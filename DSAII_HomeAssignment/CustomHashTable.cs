using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalHashFunctionProvider;

namespace CustomHashTableProject
{
    public class CustomHashTable<K, V> : ICustomHashTable<K, V>
        where K : IEquatable<K>
        where V : IEquatable<V>
    {

        private const int INITIAL_LENGTH = 8;
        private const double MAX_FILL_FACTOR = 0.7;

        public int Size { get; set; } = 0;
        public int CollisionCount { get; set; } = 0;

        static IHashFunctionProvider provider = new CarterWegmanHashFunctionProvider();
        public IHashFunction HashFunction { get; set; } = provider.GetHashFunction(INITIAL_LENGTH);

        //Add an array of type Bucket to store the data
        Bucket<K, V>[] bucket = new Bucket<K, V>[INITIAL_LENGTH];

        public int Count()
        {
            return Size;
        }

        public bool Delete(K key)
        {
            int pos = HashFunction.Hash(key);

            if (bucket[pos].Value == null)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                if (bucket[pos].Key.Equals(key))
                {
                    if (bucket[pos].IsDeleted)
                    {
                        throw new KeyNotFoundException();
                    }
                    else
                    {
                        bucket[pos].IsDeleted = true;
                        Size--;
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetCollisions()
        {
            return CollisionCount;
        }

        public double GetLoadFactor()
        {
            return ((double)Size / (double)bucket.Length);
        }

        public bool Insert(K key, V value)
        {
            //hashFunction = provider.GetHashFunction(bucket.Length);
            int pos = HashFunction.Hash(key);

            if (bucket[pos].Value == null || bucket[pos].IsDeleted)
            {
                bucket[pos] = new Bucket<K, V>() { Key = key, Value = value, IsDeleted = false };
                // successfully added the new item, increment size
                Size++;

                return true;
            }
            else if (bucket[pos].Key.Equals(key))
            {
                // duplicate key found
                throw new Exception("Duplicate key ");
            }
            else if (bucket[pos].Value != null)
            {
                bool rehashRequired = true;
                if (rehashRequired)
                {
                    bool rehashSuccessful = false;
                    while (!rehashSuccessful)
                    {
                        rehashSuccessful = true;
                        if (GetLoadFactor() > MAX_FILL_FACTOR || CollisionCount > 2)
                        {
                            // double the hashtable size
                            Bucket<K, V>[] newBucket = new Bucket<K, V>[bucket.Length * 2];
                            Bucket<K, V>[] oldBucket = bucket;
                            bucket = newBucket;
                            // select a new hash function at random from the universal family of hash functions
                            HashFunction = provider.GetHashFunction(newBucket.Length);
                            // Set the collisionCount to 0
                            CollisionCount = 0;
                            foreach (Bucket<K, V> oldBuck in oldBucket)
                            {
                                if (oldBuck.Value != null && !oldBuck.IsDeleted)
                                {

                                    Insert(oldBuck.Key, oldBuck.Value);
                                    Size--;
                                }
                            }

                        }
                        else
                        {
                            Bucket<K, V>[] rehashedBucket = new Bucket<K, V>[bucket.Length];
                            Bucket<K, V>[] oldBucket = bucket;
                            bucket = rehashedBucket;
                            // selecting a new hash function at random from the universal family of hash functions
                            HashFunction = provider.GetHashFunction(oldBucket.Length);
                            foreach (Bucket<K, V> oldBuck in oldBucket)
                            {
                                if (oldBuck.Value != null)
                                {
                                    Insert(oldBuck.Key, oldBuck.Value);
                                    Size--;
                                }
                            }

                            // increment the collisionCount
                            CollisionCount++;
                            rehashSuccessful = false;
                            continue;
                        }
                    }
                }
                Insert(key, value);

            }

            //throw new Exception("This code should be unreachable! No available space!");
            return false;
        }

        public V Search(K key)
        {
            int pos = HashFunction.Hash(key);
            if (bucket[pos].Key.Equals(key) && !bucket[pos].IsDeleted)
            {
                return bucket[pos].Value;
            }
            else
            {
                throw new KeyNotFoundException("Key not found");
            }
        }



        public bool Update(K key, V newValue)
        {
            int pos = HashFunction.Hash(key);

            if (bucket[pos].Key.Equals(key))
            {
                bucket[pos].Value = newValue;
                return true;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
