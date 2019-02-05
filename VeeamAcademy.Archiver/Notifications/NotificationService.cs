using System;

namespace VeeamAcademy.Archiver.Notifications
{
    public class NotificationService
    {
        #region ReadingChanged

        public event EventHandler<ReadingChangedEventArgs> ReadingChanged;

        public void RaiseReadingChanged(ReadingChangedEventArgs e)
        {
            OnReadingChanged(e);
        }

        protected virtual void OnReadingChanged(ReadingChangedEventArgs e)
        {
            ReadingChanged?.Invoke(this, e);
        }

        #endregion

        #region ProcessingChanged

        public event EventHandler<ProcessingChangedEventArgs> ProcessingChanged;

        public void RaiseProcessingChanged(ProcessingChangedEventArgs e)
        {
            OnProcessingChanged(e);
        }

        protected virtual void OnProcessingChanged(ProcessingChangedEventArgs e)
        {
            ProcessingChanged?.Invoke(this, e);
        }

        #endregion

        #region WritingChanged

        public event EventHandler<WritingChangedEventArgs> WritingChanged;

        public void RaiseWritingChanged(WritingChangedEventArgs e)
        {
            OnWritingChanged(e);
        }

        protected virtual void OnWritingChanged(WritingChangedEventArgs e)
        {
            WritingChanged?.Invoke(this, e);
        }

        #endregion


        #region ExceptionOccured

        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccured;

        public void RaiseExceptionOccured(ExceptionOccuredEventArgs e)
        {
            OnExceptionOccured(e);
        }

        protected virtual void OnExceptionOccured(ExceptionOccuredEventArgs e)
        {
            ExceptionOccured?.Invoke(this, e);
        }

        #endregion

    }

}
