using System;

namespace VeeamAcademy.Archiver.Notifications
{
    public class ExceptionOccuredEventArgs:EventArgs
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}