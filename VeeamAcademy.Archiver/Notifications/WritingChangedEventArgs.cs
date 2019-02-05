using System;

namespace VeeamAcademy.Archiver.Notifications
{
    public class WritingChangedEventArgs : EventArgs
    {
        public Chunk Chunk { get; set; }
        public string Message { get; set; }
    }
}