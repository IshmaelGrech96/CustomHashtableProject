using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalHashFunctionProvider;

namespace CustomHashTableProject
{
    public class CustomHashTableThreadSafe<K, V>
                where K : IEquatable<K>
                where V : IEquatable<V>
    {
        protected static CustomHashTable<K, V> HashTable { get; set; }

        internal ReaderWriterLockSlim ReaderWriterLockSlim { get; set; }

        private const int INITIAL_LENGTH = 8;
        private const double MAX_FILL_FACTOR = 0.7;

        public int Size { get; set; } = 0;
        public int CollisionCount { get; set; } = 0;

        static IHashFunctionProvider provider = new CarterWegmanHashFunctionProvider();
        public IHashFunction HashFunction { get; set; } = provider.GetHashFunction(INITIAL_LENGTH);

        //Add an array of type Bucket to store the data
        BucketThreadSafe<K, V>[] bucket = new BucketThreadSafe<K, V>[INITIAL_LENGTH];

        public CustomHashTableThreadSafe()
        {

            ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public double GetLoadFactor()
        {
            return ((double)Size / (double)bucket.Length);
        }

        public bool Insert(K key, V value)
        {
            // Obtain an upgradeable read lock
            ReaderWriterLockSlim.EnterUpgradeableReadLock();
            int pos = SearchBucketPosition(key, true);
            try
            {

                if (bucket[pos] == null || bucket[pos].IsDeleted)
                {
                    BucketThreadSafe<K, V> newBucket = new BucketThreadSafe<K, V>() { Key = key, Value = value, IsDeleted = false };
                    BucketThreadSafe<K, V> oldBucket = Interlocked.CompareExchange(ref bucket[pos], newBucket, bucket[pos]);
                    if (!Object.ReferenceEquals(oldBucket, bucket[pos]))
                    {
                        Size++;
                        return true;  // Insert is successful!
                    }
                    else
                    {
                        return false; // Insert did not occur
                    }

                }
                else if (bucket[pos].Key.Equals(key))
                {
                    // duplicate key found
                    throw new Exception("Duplicate key ");
                }
                else if (bucket[pos] != null)
                {
                    ReaderWriterLockSlim.EnterWriteLock();
                    try
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
                                    BucketThreadSafe<K, V>[] newBucket = new BucketThreadSafe<K, V>[bucket.Length * 2];
                                    BucketThreadSafe<K, V>[] oldBucket = bucket;
                                    bucket = newBucket;
                                    // select a new hash function at random from the universal family of hash functions
                                    HashFunction = provider.GetHashFunction(newBucket.Length);
                                    // Set the collisionCount to 0
                                    CollisionCount = 0;
                                    foreach (BucketThreadSafe<K, V> oldBuck in oldBucket)
                                    {
                                        if (oldBuck != null && !oldBuck.IsDeleted)
                                        {

                                            Insert(oldBuck.Key, oldBuck.Value);
                                            Size--;
                                        }
                                    }
                                }
                                else
                                {
                                    // rehash the elements of the hashtable, keeping the same size                         
                                    BucketThreadSafe<K, V>[] rehashedBucket = new BucketThreadSafe<K, V>[bucket.Length];
                                    BucketThreadSafe<K, V>[] oldBucket = bucket;
                                    bucket = rehashedBucket;
                                    // selecting a new hash function at random from the universal family of hash functions
                                    HashFunction = provider.GetHashFunction(oldBucket.Length);
                                    foreach (BucketThreadSafe<K, V> oldBuck in oldBucket)
                                    {
                                        if (oldBuck != null)
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
                    catch (Exception ex){}
                    finally
                    {
                        ReaderWriterLockSlim.ExitWriteLock();
                    }


                }
            }
            finally
            {
                ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
            return false;

        }

        public V Search(K key)
        {
            ReaderWriterLockSlim.EnterReadLock();
            try
            {

                //var value = HashTable.Search(key);
                int pos = HashFunction.Hash(key);
                if (bucket[pos] != null)
                {
                    if (bucket[pos].Key.Equals(key) && !bucket[pos].IsDeleted)
                    {
                        return bucket[pos].Value;
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }

            }
            catch (LockRecursionException lre)
            {
                Console.WriteLine("\n{0}: {1}",
                    lre.GetType().Name, lre.Message);
            }
            finally
            {
                ReaderWriterLockSlim.ExitReadLock();
            }

            return default;

        }

        public bool Update(K key, V value)
        {
            ReaderWriterLockSlim.EnterReadLock();

            try
            {
                //var value = HashTable.Search(key);

                int pos = SearchBucketPosition(key);

                BucketThreadSafe<K, V> newBucket = new BucketThreadSafe<K, V> { Key = key, Value = value };
                BucketThreadSafe<K, V> oldBucket = Interlocked.CompareExchange(ref bucket[pos], newBucket, bucket[pos]);
                if (Object.ReferenceEquals(oldBucket, bucket[pos]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (LockRecursionException lre)
            {
                Console.WriteLine("\n{0}: {1}",
                    lre.GetType().Name, lre.Message);
                return false;
            }
            finally
            {
                ReaderWriterLockSlim.ExitReadLock();
            }


        }

        public bool Delete(K key)
        {
            ReaderWriterLockSlim.EnterReadLock();

            try
            {
                //Validations are done in SearchBucketPosition
                int pos = SearchBucketPosition(key);
                //bucket[pos].IsDeleted = true;
                Size--;

                BucketThreadSafe<K, V> newBucket = new BucketThreadSafe<K, V> { Key = bucket[pos].Key, Value = bucket[pos].Value, IsDeleted = true };
                BucketThreadSafe<K, V> oldBucket = Interlocked.CompareExchange(ref bucket[pos], newBucket, bucket[pos]);
                if (Object.ReferenceEquals(oldBucket, bucket[pos]))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (LockRecursionException lre)
            {
                Console.WriteLine("\n{0}: {1}",
                    lre.GetType().Name, lre.Message);
                return false;
            }
            finally
            {
                ReaderWriterLockSlim.ExitReadLock();
            }


        }

        private int SearchBucketPosition(K key, bool insert = false)
        {

            // From the key, find the position of the entry using the GetHashCode
            int pos = HashFunction.Hash(key);

            //If using search for insert return position only
            if (insert)
            {
                return pos;
            }
            // empty space, we can end the search
            if (bucket[pos] == null)
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
                        return pos;
                    }
                }
            }

            return default;
        }

    }
}
