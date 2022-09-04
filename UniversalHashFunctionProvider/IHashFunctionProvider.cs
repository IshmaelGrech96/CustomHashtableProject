using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalHashFunctionProvider
{
    /// <summary>
    /// Factory that can be used to generate hash functions
    /// </summary>
    public interface IHashFunctionProvider
    {
        /// <summary>
        /// Generates a new hash function and returns it
        /// </summary>
        /// <returns>Returns a hash function</returns>
        IHashFunction GetHashFunction();

        /// <summary>
        /// Generates a new hash function and returns it.  The hash function will map the objects to a hashtable of size M.
        /// </summary>
        /// <param name="M">The size of the hashtable that the hash function will map to</param>
        /// <returns>The hash function that is able to map to a table of size M</returns>
        IHashFunction GetHashFunction(int M);
    }
}
