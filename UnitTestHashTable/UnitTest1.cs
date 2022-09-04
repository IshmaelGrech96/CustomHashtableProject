using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomHashTableProject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace UnitTestHashTable
{
    [TestClass]
    public class UnitTest1
    {
        private CustomHashTable<int, string> customHashTable;
        private CustomHashTableThreadSafe<int, string> threadSafeHashTable;
        Random random = new Random();
        private const int N = 5000;

        [TestInitialize]
        public void TestInitialize()
        {
            customHashTable = new CustomHashTable<int, string>();
            threadSafeHashTable = new CustomHashTableThreadSafe<int, string>();
        }

        /// <summary>
        /// Generates random tuple with a unique key and a random value
        /// </summary>
        /// <returns></returns>
        private Tuple<int, string> GenerateRandom()
        {
            string alph = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string symb = "/.,;_-";
            string nums = "123456789";
            List<string> list = new List<string> { alph, symb, nums };

            string randString = "";
            for (int x = 0; x < 10; x++)
            {
                string charact = list[random.Next(list.Count)];
                randString += charact[random.Next(charact.Length)];
            }
            Tuple<int, string> tuple = new Tuple<int, string>(Guid.NewGuid().GetHashCode(), randString);

            return tuple;
        }


        [TestMethod]
        public void TestInsertParallel()
        {
            ConcurrentDictionary<int, string> concurrentDict = new ConcurrentDictionary<int, string>();
            Parallel.For(0, 1000, i =>
            {
                bool isSuccesful = false;
                while (!isSuccesful)
                {                 
                    Tuple<int, string> tuple = GenerateRandom();
                    concurrentDict.TryAdd(tuple.Item1, tuple.Item2);
                    isSuccesful = threadSafeHashTable.Insert(tuple.Item1, tuple.Item2);
                }
            });

            foreach(var item in concurrentDict)
            {
                Assert.AreEqual(threadSafeHashTable.Search(item.Key), item.Value);
            }

        }

        [TestMethod]
        public void TestDeleteParallel()
        {
            ConcurrentDictionary<int, string> concurrentDict = new ConcurrentDictionary<int, string>();
            Parallel.For(0, 1000, i =>
            {
                bool isSuccesful = false;
                while (!isSuccesful)
                {
                    Tuple<int, string> tuple = GenerateRandom();
                    concurrentDict.TryAdd(tuple.Item1, tuple.Item2);
                    isSuccesful = threadSafeHashTable.Insert(tuple.Item1, tuple.Item2);
                    threadSafeHashTable.Delete(tuple.Item1);
                }
            });

            foreach (var item in concurrentDict)
            {
                try
                {
                    threadSafeHashTable.Search(item.Key);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(ex.Message, "The given key was not present in the dictionary.");
                }
                
            }

        }


        [TestMethod]
        public void TestUpdateParallel()
        {
            ConcurrentDictionary<int, string> concurrentDict = new ConcurrentDictionary<int, string>();
            Parallel.For(0, 1000, i =>
            {
                bool isSuccesful = false;
                while (!isSuccesful)
                {
                    Tuple<int, string> tuple = GenerateRandom();
                    concurrentDict.TryAdd(tuple.Item1, tuple.Item2);
                    isSuccesful = threadSafeHashTable.Insert(tuple.Item1, "TEST");
                    threadSafeHashTable.Update(tuple.Item1, tuple.Item2);
                }
            });

            foreach (var item in concurrentDict)
            {
                Assert.AreEqual(threadSafeHashTable.Search(item.Key), item.Value);
            }

        }

        [TestMethod]
        public void TestInsertParallelSize()
        {
            ConcurrentDictionary<int, string> concurrentDict = new ConcurrentDictionary<int, string>();
            Parallel.For(0, 1000, i =>
            {
                bool isSuccesful = false;
                while (!isSuccesful)
                {
                    Tuple<int, string> tuple = GenerateRandom();
                    concurrentDict.TryAdd(tuple.Item1, tuple.Item2);
                    isSuccesful = threadSafeHashTable.Insert(tuple.Item1, tuple.Item2);
                }
            });

            Assert.IsTrue(threadSafeHashTable.Size == concurrentDict.Count,"{0},{1}", threadSafeHashTable.Size, concurrentDict.Count);

        }


        [TestMethod]
        public void TestInsertValidSize()
        {
            customHashTable.Insert(1, "Value");
            Assert.AreEqual(1, customHashTable.Size);
        }

        [TestMethod]
        public void TestInsertValidValues()
        {
            customHashTable.Insert(2, "Value");
            Assert.AreEqual("Value", customHashTable.Search(2));
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestInsertDuplicateKey()
        {
            customHashTable.Insert(2, "Value");
            customHashTable.Insert(2, "AnotherValue");
        }

        [TestMethod]
        public void TestInsertAfterDelete()
        {
            customHashTable.Insert(2, "Value");
            customHashTable.Delete(2);
            customHashTable.Insert(2, "Value");
            Assert.AreEqual("Value", customHashTable.Search(2));
        }

        [TestMethod]
        public void TestSearchValid()
        {
            customHashTable.Insert(2, "Value");
            Assert.AreEqual("Value", customHashTable.Search(2));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestSearchInvalid()
        {
            customHashTable.Insert(2, "Value");
            customHashTable.Delete(2);
            Assert.AreEqual("Value", customHashTable.Search(2));
        }

        [TestMethod]
        public void TestUpdateValid()
        {
            customHashTable.Insert(2, "Value");
            customHashTable.Update(2, "Updated");
            Assert.AreEqual("Updated", customHashTable.Search(2));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestUpdateInvalid()
        {
            customHashTable.Update(2, "Updated");
            Assert.AreEqual("Updated", customHashTable.Search(2));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestDeleteValid()
        {
            customHashTable.Insert(2, "Value");
            customHashTable.Delete(2);
            customHashTable.Search(2);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestDeleteInvalid()
        {
            customHashTable.Delete(2);
        }


    }
}