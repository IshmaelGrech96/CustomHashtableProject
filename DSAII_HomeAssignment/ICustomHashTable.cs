using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHashTableProject
{
    public interface ICustomHashTable<K,V>
    {
        bool Insert(K key, V value);
        bool Update(K key, V newValue);
        V Search(K key);
        bool Delete(K key);
        int GetCollisions();
        int Count();
        double GetLoadFactor();
    }
}
