using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionaryEntry
{
    public Vector3Int key;
    public Plant value;

    public SerializableDictionaryEntry(Vector3Int key, Plant value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
public class SerializableDictionary
{
    public List<SerializableDictionaryEntry> entries = new List<SerializableDictionaryEntry>();

    public void FromDictionary(Dictionary<Vector3Int, Plant> dictionary)
    {
        entries.Clear();
        foreach (var kvp in dictionary)
        {
            entries.Add(new SerializableDictionaryEntry(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<Vector3Int, Plant> ToDictionary()
    {
        Dictionary<Vector3Int, Plant> dictionary = new Dictionary<Vector3Int, Plant>();
        foreach (var entry in entries)
        {
            dictionary[entry.key] = entry.value;
        }
        return dictionary;
    }
}
