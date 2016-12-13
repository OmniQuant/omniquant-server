using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FastQuant
{
    [DataContract]
    public class AccountDataFieldList : ICollection
    {
        [DataMember]
        private readonly Dictionary<string, Dictionary<string, object>> _fields = new Dictionary<string, Dictionary<string, object>>();

        public int Count => _fields.Values.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public object this[string name, string currency]
        {
            get
            {
                Dictionary<string, object> logger;
                if (!_fields.TryGetValue(name, out logger))
                    return null;
                object value;
                logger.TryGetValue(currency, out value);
                return value;
            }
            internal set
            {
                Add(name, currency, value);
            }
        }

        public object this[string name] => this[name, string.Empty];

        internal AccountDataFieldList()
        {
        }

        /// <summary>
        /// Update if the key is already there.
        /// </summary>
        public void Add(string name, string currency, object value)
        {
            Dictionary<string, object> logger;
            if (!_fields.TryGetValue(name, out logger))
            {
                logger = new Dictionary<string, object>();
                _fields.Add(name, logger);
            }
            logger.Add(currency, value);
        }

        public void Add(string name, object value) => Add(name, string.Empty, value);

        public void Clear() => _fields.Clear();

        public AccountDataField[] ToArray() => _fields.SelectMany(name => name.Value, (logger, field) => new AccountDataField(logger.Key, field.Key, field.Value)).ToArray();

        public void CopyTo(Array array, int index) => ToArray().CopyTo(array, index);

        public IEnumerator GetEnumerator() => ToArray().GetEnumerator();
    }
}