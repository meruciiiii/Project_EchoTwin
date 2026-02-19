
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiDictionary<TKey, TValue>
{
    private Dictionary<TKey, TValue> keyToValue = new Dictionary<TKey, TValue>();
    private Dictionary<TValue, TKey> valueToKey = new Dictionary<TValue, TKey>();
    public void Add(TKey key, TValue value)
    {
        if (keyToValue.ContainsKey(key))
        {
            Debug.LogWarning($"Key {key} already exists, overwriting");
            Remove(key);
        }
        if (valueToKey.ContainsKey(value))
        {
            Debug.LogWarning($"Value {value} already exists, removing old key");
            Remove(valueToKey[value]);
        }
        keyToValue[key] = value;
        valueToKey[value] = key;
    }
    public bool TryGetValue(TKey key, out TValue value)
        => keyToValue.TryGetValue(key, out value);
    public bool TryGetKey(TValue value, out TKey key)
        => valueToKey.TryGetValue(value, out key);
    public void Remove(TKey key)
    {
        if (keyToValue.TryGetValue(key, out TValue value))
        {
            keyToValue.Remove(key);
            valueToKey.Remove(value);
        }
    }
    public void Clear()
    {
        keyToValue.Clear();
        valueToKey.Clear();
    }
    public int Count => keyToValue.Count;
}
