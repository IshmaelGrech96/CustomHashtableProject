using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomHashTableProject
{
    public struct Bucket<K, V>
    {
        internal K Key { get; set; }
        internal V Value { get; set; }
        internal bool IsDeleted { get; set; } = false;
    }
}
