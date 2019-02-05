using System;
using System.Threading;

namespace VeeamAcademy.Archiver.Utils
{
    internal static class ThreadUtils
    {
        public static Thread StartParametrizedThread<T>(T parameter, Action<T> action,
            ThreadPriority priority)
        {
            var t = new Thread(() => action.Invoke(parameter));
            t.Start();
            return t;
        }

        public static Thread StartBackgroundParametrizedThread<T>(T parameter, Action<T> action,
            ThreadPriority priority)
        {
            var t = new Thread(() => action.Invoke(parameter)) {IsBackground = true, Priority = priority};
            t.Start();
            return t;
        }
        public static Thread StartBackgroundParametrizedThread<T>(T parameter, Action<T> action)
        {
            var t = new Thread(() => action.Invoke(parameter)) {IsBackground = true};
            t.Start();
            return t;
        }
    }
}