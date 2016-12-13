using System.Collections;
using System.Collections.Generic;

namespace FastQuant
{
    public class AltIdList : IEnumerable<AltId>
    {
        private readonly IdArray<AltId> array = new IdArray<AltId>();

        private readonly List<AltId> list = new List<AltId>();

        public void Add(AltId id)
        {
            this.array[id.ProviderId] = id;
            this.list.Add(id);
        }

        public void Clear()
        {
            this.array.Clear();
            this.list.Clear();
        }

        public AltId Get(byte providerId) => this.array[providerId];

        public IEnumerator<AltId> GetEnumerator() => this.list.GetEnumerator();

        public void Remove(AltId id)
        {
            this.array.Remove(id.ProviderId);
            this.list.Remove(id);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => this.list.Count;

        public AltId this[int i] => this.list[i];
    }
}