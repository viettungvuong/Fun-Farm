using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [Serializable]
    public class SerializableDictionaryEntry
    {
        public TKey key;
        public TValue value;

        public SerializableDictionaryEntry(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public List<SerializableDictionaryEntry> entries = new List<SerializableDictionaryEntry>();

    public void FromDictionary(Dictionary<TKey, TValue> dictionary)
    {
        entries.Clear();
        foreach (var kvp in dictionary)
        {
            entries.Add(new SerializableDictionaryEntry(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        foreach (var entry in entries)
        {
            dictionary[entry.key] = entry.value;
        }
        return dictionary;
    }
}
