using System;

namespace VeeamAcademy.Archiver.Notifications
{
    public class ProcessingChangedEventArgs : EventArgs
    {
        public Chunk Chunk { get; set; }
        public string Message { get; set; }
        public string ThreadName { get; set; }
    }
}