using System;

namespace VeeamAcademy.Archiver.Notifications
{
    public class ReadingChangedEventArgs : EventArgs
    {
        public long StreamPosition { get; set; }
        public long StreamLength { get; set; }
        public int ChunkId { get; set; }
        public string Message { get; set; }
    }
}