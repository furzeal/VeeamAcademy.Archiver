using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using VeeamAcademy.Archiver.Concurrent;
using VeeamAcademy.Archiver.Interfaces;
using VeeamAcademy.Archiver.Notifications;
using VeeamAcademy.Archiver.Utils;

namespace VeeamAcademy.Archiver
{
    public class ArchivationService : IDisposable
    {
        private readonly IConfiguration _settings;
        private readonly NotificationService _notificationService;
        private ProcessingQueue<Chunk> _processingQueue;

        private readonly ProcessingDictionary<int, Chunk>
            _processingDictionary = new ProcessingDictionary<int, Chunk>();

        //private readonly object _locker = new object();
        private long _lastChunkIndex;

        public ArchivationService(IConfiguration settings, NotificationService notificationService)
        {
            _settings = settings;
            _notificationService = notificationService;
        }

        public void Execute(string sourceFilepath, string processedFilepath)
        {
            var poolSize = _settings.MaxThreadCount - 3 > 1 ? _settings.MaxThreadCount - 3 : 1;

            _processingQueue = new ProcessingQueue<Chunk>(poolSize, _processTask);
            _lastChunkIndex = new FileInfo(sourceFilepath).Length / _settings.BufferSize;

            // Start IO threads
            var readerThread =
                ThreadUtils.StartBackgroundParametrizedThread(sourceFilepath, _readFile);
            var writerThread =
                ThreadUtils.StartBackgroundParametrizedThread(processedFilepath, _writeToFile);

            readerThread.Join();
            writerThread.Join();
        }

        private void _readFile(string sourceFilepath)
        {
            try
            {
                using (var reader = new FileStream(sourceFilepath, FileMode.Open))
                {
                    var index = 0;
                    var length = reader.Length;
                    var bufferSize = _settings.BufferSize;

                    while (reader.Position < length)
                    {
                        var chunk = new Chunk {Id = index};

                        var size = length - reader.Position > bufferSize ? bufferSize : length - reader.Position;
                        chunk.BufferBytes = new byte[size];

                        reader.Read(chunk.BufferBytes, 0, (int) size);

                        _processingQueue.EnqueueProductForProcessing(chunk);
                        index++;

                        _notificationService.RaiseReadingChanged(new ReadingChangedEventArgs
                        {
                            ChunkId = chunk.Id,
                            Message = "Reading file",
                            StreamPosition = reader.Position,
                            StreamLength = reader.Length
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _notificationService.RaiseExceptionOccured(new ExceptionOccuredEventArgs
                {
                    Message = "An error has been occured during reading",
                    Exception = e
                });
            }
        }

        private void _processTask(Chunk chunk)
        {
            try
            {
                if (chunk is null)
                    return;

                using (var ms = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(ms, CompressionMode.Compress))
                        zipStream.Write(chunk.BufferBytes, 0, chunk.BufferBytes.Length);

                    chunk.ProcessedBytes = ms.ToArray();

                    _processingDictionary[chunk.Id] = chunk;

                    _notificationService.RaiseProcessingChanged(new ProcessingChangedEventArgs()
                    {
                        Chunk = chunk,
                        Message = "Compressing file",
                        ThreadName = Thread.CurrentThread.Name
                    });
                }
            }
            catch (Exception e)
            {
                _notificationService.RaiseExceptionOccured(new ExceptionOccuredEventArgs
                {
                    Message = "An error has been occured during processing",
                    Exception = e
                });
            }
        }

        private void _writeToFile(string processedFilepath)
        {
            try
            {
                using (var writer = new FileStream(processedFilepath, FileMode.Append))
                {
                    //throw new Exception();
                    var index = 0;

                    while (index <= _lastChunkIndex)
                    {
                        _processingDictionary.TryGetValue(index, out var chunk);

                        if (chunk != null)
                        {
                            writer.Write(chunk.ProcessedBytes, 0, chunk.ProcessedBytes.Length);


                            _notificationService.RaiseWritingChanged(new WritingChangedEventArgs()
                            {
                                Chunk = chunk,
                                Message = "Writing to file",
                            });

                            // Clear buffer
                            _processingDictionary[index] = null;

                            index++;
                        }
                        //else
                        //    Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e)
            {
                _notificationService.RaiseExceptionOccured(new ExceptionOccuredEventArgs
                {
                    Message = "An error has been occured during writing",
                    Exception = e
                });
            }
        }

        #region IDisposable

        private bool _disposed = false;

        // ReSharper disable once FlagArgument
        protected virtual void Dispose(bool isDisposing)
        {
            if (!_disposed)
            {
                if (isDisposing)
                {
                    _processingQueue?.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}