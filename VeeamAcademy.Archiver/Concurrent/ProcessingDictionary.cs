using System.Collections.Generic;

namespace VeeamAcademy.Archiver.Concurrent
{
    public sealed class ProcessingDictionary<TKey, TValue>
    {
        private readonly object _locker = new object();
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                lock (_locker)
                    return _dictionary[key];
            }
            set
            {
                lock (_locker)
                    _dictionary[key] = value;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_locker)
                return _dictionary.TryGetValue(key, out value);
        }
    }
}