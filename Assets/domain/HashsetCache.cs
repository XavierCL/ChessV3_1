using System;
using System.Diagnostics;

[DebuggerDisplay("{occupancy}/{capacity}")]
public class HashsetCache<TKey, TValue> where TValue : class
{
  private readonly Entry[] entries;
  private readonly int capacity;
  private int occupancy;
  private int cacheHit;
  private int cacheMiss;
  private int replaceSet;
  private class Entry
  {
    public readonly TKey key;
    public readonly TValue value;

    public Entry(TKey key, TValue value)
    {
      this.key = key;
      this.value = value;
    }
  }

  public HashsetCache(int maxCapacity)
  {
    capacity = GetLargestPrimeUnder(maxCapacity);
    entries = new Entry[capacity];
    occupancy = 0;
    cacheHit = 0;
    cacheMiss = 0;
    replaceSet = 0;
  }

  public void Set(TKey key, TValue value)
  {
    var keyIndex = (uint)key.GetHashCode() % entries.Length;
    var oldEntry = entries[keyIndex];
    if (oldEntry == null)
    {
      ++occupancy;
    }
    else if (!Equals(oldEntry.key, key))
    {
      ++replaceSet;
    }
    entries[keyIndex] = new Entry(key, value);
  }

  public TValue Get(TKey key)
  {
    var keyIndex = (uint)key.GetHashCode() % entries.Length;
    var entry = entries[keyIndex];

    if (entry == null || !Equals(key, entry.key))
    {
      ++cacheMiss;
      return null;
    }

    ++cacheHit;
    return entry.value;
  }

  public override string ToString()
  {
    return $"{occupancy}/{capacity} - Hits: {cacheHit} - Miss: {cacheMiss} - Replaces: {replaceSet}";
  }

  private int GetLargestPrimeUnder(int maxCapacity)
  {
    if (maxCapacity % 2 == 0) --maxCapacity;
    if (maxCapacity < 1) return 1;
    for (var testCapacity = maxCapacity; testCapacity > 1; --testCapacity)
    {
      if (IsPrime(testCapacity)) return testCapacity;
    }

    return 1;
  }

  private bool IsPrime(int value)
  {
    if (value % 2 == 0) return false;
    var sqrtValue = (int)Math.Sqrt(value);

    for (var divider = 3; divider <= sqrtValue; divider += 2)
    {
      if (value % divider == 0) return false;
    }

    return true;
  }
}