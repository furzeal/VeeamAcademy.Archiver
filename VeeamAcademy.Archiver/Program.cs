using System;
using System.IO;
using VeeamAcademy.Archiver.Notifications;
using VeeamAcademy.Archiver.Settings;

namespace VeeamAcademy.Archiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var sourceFilepath = Path.Combine(currentDir, "2018 11 24 Запись+экрана.wmv");
            var processedFilepath = Path.Combine(currentDir, "2018 11 24 Запись+экрана.wmv.gz");
            var settings = new ArchivationSettings();

            var notificationService = new NotificationService();

            notificationService.ReadingChanged += _readingChanged;
            notificationService.ProcessingChanged += _processingChanged;
            notificationService.WritingChanged += _writingChanged;
            notificationService.ExceptionOccured += _exceptionOccured;

            using (var archiver = new ArchivationService(settings, notificationService))
                archiver.Execute(sourceFilepath, processedFilepath);

#if DEBUG
            Console.ReadKey();
#endif
        }


        private static void _readingChanged(object sender, ReadingChangedEventArgs e)
        {
            Console.WriteLine(
                $"{e.Message} (Chunk index: {e.ChunkId}, Progress: {e.StreamPosition} / {e.StreamLength})");
        }

        private static void _processingChanged(object sender, ProcessingChangedEventArgs e)
        {
            Console.WriteLine(
                $"{e.Message} (Chunk index: {e.Chunk.Id}, Processed {e.Chunk.BufferBytes.Length} to {e.Chunk.ProcessedBytes.Length} bytes by {e.ThreadName})");
        }

        private static void _writingChanged(object sender, WritingChangedEventArgs e)
        {
            Console.WriteLine(
                $"{e.Message} (Chunk index: {e.Chunk.Id}, {e.Chunk.ProcessedBytes.Length} bytes written)");
        }

        private static void _exceptionOccured(object sender, ExceptionOccuredEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Exception.Message);
        }
    }
}