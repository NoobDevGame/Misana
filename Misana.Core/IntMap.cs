using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

/*
The MIT License (MIT)

Copyright (c) .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

namespace Misana.Core
{
    internal static class HashHelpers
    {
        // This is the maximum prime smaller than Array.MaxArrayLength
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;
        // Table of prime numbers to use as hash table sizes. 
        // A typical resize algorithm would pick the smallest prime number in this array
        // that is larger than twice the previous capacity. 
        // Suppose our Hashtable currently has capacity x and enough elements are added 
        // such that a resize needs to occur. Resizing first computes 2x then finds the 
        // first prime in the table greater than 2x, i.e. if primes are ordered 
        // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n. 
        // Doubling is important for preserving the asymptotic complexity of the 
        // hashtable operations such as add.  Having a prime guarantees that double 
        // hashing does not lead to infinite loops.  IE, your hash function will be 
        // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
        public static readonly int[] primes = {
            3,
            7,
            11,
            17,
            23,
            29,
            37,
            47,
            59,
            71,
            89,
            107,
            131,
            163,
            197,
            239,
            293,
            353,
            431,
            521,
            631,
            761,
            919,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369,
            8639249,
            10367101,
            12440537,
            14928671,
            17914409,
            21497293,
            25796759,
            30956117,
            37147349,
            44576837,
            53492207,
            64190669,
            77028803,
            92434613,
            110921543,
            133105859,
            159727031,
            191672443,
            230006941,
            276008387,
            331210079,
            397452101,
            476942527,
            572331049,
            686797261,
            824156741,
            988988137,
            1186785773,
            1424142949,
            1708971541,
            2050765853,
            MaxPrimeArrayLength
        };

        public static int GetPrime(int min)
        {
            for (var i = 0; i < primes.Length; i++)
            {
                var prime = primes[i];
                if (prime >= min)
                    return prime;
            }

            return min;
        }

        // Returns size of hashtable to grow to.
        public static int ExpandPrime(int oldSize)
        {
            var newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint) newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                Debug.Assert(MaxPrimeArrayLength == GetPrime(MaxPrimeArrayLength), "Invalid MaxPrimeArrayLength");
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }
    }

    public class IntMap<TValue>
    {
        private int[] _buckets;
        private int _count;
        private Entry[] _entries;
        private int _freeCount;

        private int _freeList;

        public IntMap(int capacity)
        {
            Initialize(capacity);
        }

        
        public int Count => _count - _freeCount;
        public List<int> Keys => _entries.Where((t,i) => t.hashCode >= 0 && i < Count).Select(t => t.key).ToList();
        public List<TValue> Values => Count == 0 ? new List<TValue>() : _entries.Where((t, i) => t.hashCode >= 0 && i < Count).Select(t => t.value).ToList();

        public TValue this[int key]
        {
            get
            {
                var i = FindEntry(key);
                if (i >= 0)
                    return _entries[i].value;
                throw new KeyNotFoundException();
            }
            set { Insert(key, value, false); }
        }

        public void Add(int key, TValue value)
        {
            Insert(key, value, true);
        }

        public void Clear()
        {
            if (_count > 0)
            {
                for (var i = 0; i < _buckets.Length; i++)
                    _buckets[i] = -1;
                Array.Clear(_entries, 0, _count);
                _freeList = -1;
                _count = 0;
                _freeCount = 0;
            }
        }

        public bool ContainsKey(int key)
        {
            return FindEntry(key) >= 0;
        }

        private int FindEntry(int key)
        {
            var hashCode = key & 0x7FFFFFFF;
            for (var i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && _entries[i].key == key)
                    return i;
            }
            return -1;
        }

        private void Initialize(int capacity)
        {
            var size = HashHelpers.GetPrime(capacity);
            _buckets = new int[size];
            for (var i = 0; i < _buckets.Length; i++)
                _buckets[i] = -1;
            _entries = new Entry[size];
            _freeList = -1;
        }

        private void Insert(int key, TValue value, bool add)
        {
            var hashCode = key & 0x7FFFFFFF;
            var targetBucket = hashCode % _buckets.Length;

            for (var i = _buckets[targetBucket]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && _entries[i].key == key)
                {
                    if (add)
                    {
                        throw new ArgumentException("Duplicate Key");
                    }
                    _entries[i].value = value;
                    return;
                }
            }

            int index;

            if (_freeCount > 0)
            {
                index = _freeList;
                _freeList = _entries[index].next;
                _freeCount--;
            }
            else
            {
                if (_count == _entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % _buckets.Length;
                }
                index = _count;
                _count++;
            }

            _entries[index].hashCode = hashCode;
            _entries[index].next = _buckets[targetBucket];
            _entries[index].key = key;
            _entries[index].value = value;
            _buckets[targetBucket] = index;
        }

        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(_count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            Debug.Assert(newSize >= _entries.Length);
            var newBuckets = new int[newSize];
            for (var i = 0; i < newBuckets.Length; i++)
                newBuckets[i] = -1;

            var newEntries = new Entry[newSize];
            Array.Copy(_entries, 0, newEntries, 0, _count);

            if (forceNewHashCodes)
            {
                for (var i = 0; i < _count; i++)
                {
                    if (newEntries[i].hashCode != -1)
                    {
                        newEntries[i].hashCode = newEntries[i].key & 0x7FFFFFFF;
                    }
                }
            }

            for (var i = 0; i < _count; i++)
            {
                if (newEntries[i].hashCode >= 0)
                {
                    var bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            _buckets = newBuckets;
            _entries = newEntries;
        }

        public bool Remove(int key)
        {
            if (_buckets != null)
            {
                var hashCode = key & 0x7FFFFFFF;
                var bucket = hashCode % _buckets.Length;
                var last = -1;
                for (var i = _buckets[bucket]; i >= 0; last = i, i = _entries[i].next)
                {
                    if (_entries[i].hashCode == hashCode && _entries[i].key == key)
                    {
                        if (last < 0)
                        {
                            _buckets[bucket] = _entries[i].next;
                        }
                        else
                        {
                            _entries[last].next = _entries[i].next;
                        }
                        _entries[i].hashCode = -1;
                        _entries[i].next = _freeList;
                        _entries[i].key = 0;
                        _entries[i].value = default(TValue);
                        _freeList = i;
                        _freeCount++;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryGetValue(int key, out TValue value)
        {
            var i = FindEntry(key);
            if (i >= 0)
            {
                value = _entries[i].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        private struct Entry
        {
            public int hashCode;
            public int next;
            public int key;
            public TValue value;
        }
    }
}