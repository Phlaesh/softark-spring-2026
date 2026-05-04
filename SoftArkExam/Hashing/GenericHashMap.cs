using System;
using System.Collections.Generic;
using System.Text;
public interface Map<K, V>
{
    public V Get(K key);
    public V Put(K key, V value);
    public V Remove(K key);
    public bool IsEmpty();
    public int Size();
}
public class GenericHashMap<K, V> : Map<K, V>
{
    private class Entry<TK, TV>
    {
        public TK Key { get; set; }
        public TV Value { get; set; }

        public Entry(TK key, TV value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"[{Key}, {Value}]";
        }
    }

    private LinkedList<Entry<K, V>>[] buckets;
    private int size;
    private int capacity;
    private const double LOAD_FACTOR = 0.75;

    public GenericHashMap(int initialCapacity = 16)
    {
        capacity = initialCapacity;
        buckets = new LinkedList<Entry<K, V>>[capacity];
        size = 0;
    }

    public V Get(K key)
    {
        if (TryGetValue(key, out V value))
        {
            return value;
        }
        return default(V); // or throw an exception, as you prefer
    }

    public V Put(K key, V value)
    {
        int bucketIndex = GetBucketIndex(key);
        V oldValue = default(V);
        if (buckets[bucketIndex] == null)
        {
            buckets[bucketIndex] = new LinkedList<Entry<K, V>>();
        }

        LinkedListNode<Entry<K, V>> current = buckets[bucketIndex].First;
        while (current != null)
        {
            if (current.Value.Key.Equals(key))
            {
                oldValue = current.Value.Value;
                current.Value.Value = value;
                return oldValue;
            }
            current = current.Next;
        }

        buckets[bucketIndex].AddLast(new Entry<K, V>(key, value));
        size++;

        if ((double)size / capacity > LOAD_FACTOR)
        {
            Resize();
        }
        return oldValue;
    }

    public V Remove(K key)
    {
        int bucketIndex = GetBucketIndex(key);
        V removedValue = default(V);

        if (buckets[bucketIndex] != null)
        {
            LinkedListNode<Entry<K, V>> current = buckets[bucketIndex].First;
            while (current != null)
            {
                if (current.Value.Key.Equals(key))
                {
                    removedValue = current.Value.Value;
                    buckets[bucketIndex].Remove(current); // not efficient...
                    size--;
                    return removedValue;
                }
                current = current.Next;
            }
        }
        return removedValue;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    public int Size()
    {
        return size;
    }

    private bool TryGetValue(K key, out V value)
    {
        int bucketIndex = GetBucketIndex(key);

        if (buckets[bucketIndex] != null)
        {
            foreach (var entry in buckets[bucketIndex])
            {
                if (entry.Key.Equals(key))
                {
                    value = entry.Value;
                    return true;
                }
            }
        }

        value = default(V);
        return false;
    }

    private int GetBucketIndex(K key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int hashCode = key.GetHashCode();
        return Math.Abs(hashCode % capacity);
    }

    private void Resize()
    {
        int newCapacity = capacity * 2;
        LinkedList<Entry<K, V>>[] newBuckets = new LinkedList<Entry<K, V>>[newCapacity];

        foreach (var bucket in buckets)
        {
            if (bucket != null)
            {
                foreach (var entry in bucket)
                {
                    int newBucketIndex = Math.Abs(entry.Key.GetHashCode() % newCapacity);

                    if (newBuckets[newBucketIndex] == null)
                    {
                        newBuckets[newBucketIndex] = new LinkedList<Entry<K, V>>();
                    }

                    newBuckets[newBucketIndex].AddLast(entry);
                }
            }
        }

        buckets = newBuckets;
        capacity = newCapacity;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        bool first = true;

        for (int i = 0; i < capacity; i++)
        {
            if (buckets[i] != null)
            {
                foreach (Entry<K, V> entry in buckets[i])
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(entry.ToString());
                    first = false;
                }
            }
        }
        sb.Append("}");
        return sb.ToString();
    }
}