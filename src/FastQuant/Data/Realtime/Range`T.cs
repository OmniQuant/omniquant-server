using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastQuant.Data.Realtime
{
    public class Range<T>
    {
        public T Min { get; set; }

        public T Max { get; set; }

        internal Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString() => $"{Min} .. {Max}";

        internal string ToSingleString() => $"{Min};{Max}";

        internal void FromSingleString(string value)
        {
            try
            {
                var array = value.Split(';');
                Min = (T)Convert.ChangeType(array[0], typeof(T));
                Max = (T)Convert.ChangeType(array[1], typeof(T));
            }
            catch
            {
                // ignored
            }
        }
    }
}
