using System;
using System.Collections.Generic;

namespace XVNML2U
{
    public sealed class ElementMediaLibrary<T>
    {
        public SortedDictionary<int, string> IdToNameMap = new();
        public SortedDictionary<string, T> values = new();

        public T this[int id]
        {
            get
            {
                return values[IdToNameMap[id]];
            }
        }

        public T this[string name]
        {
            get
            {
                return values[name];
            }
        }

        public void Add(int id, string name, T value)
        {
            IdToNameMap.Add(id, name);
            values.Add(name, value);
        }

        public void Remove(int id, string name)
        {
            IdToNameMap.Remove(id);
            values.Remove(name);
        }

        public bool ContainsID(int id)
        {
            return IdToNameMap.ContainsKey(id);
        }

        public bool ContainsName(string name)
        {
            return IdToNameMap.ContainsValue(name) && values.ContainsKey(name);
        }

        internal string GetStringKey(int id)
        {
            if (IdToNameMap.ContainsKey(id) == false) return string.Empty;
            return IdToNameMap[id];
        }
    }
}