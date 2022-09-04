using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalHashFunctionProvider
{
    /// <summary>
    /// This instance represents a hash function.  You can use it to hash an object.  The hash function will use the GetHashCode method for the object to obtain an integer from the object
    /// </summary>
    /// <remarks>
    /// Since the hash function depends on the GetHashCode to provide an integer, if two non-equal objects generate the same hash code, they will cause a collision for every instance of hash function.
    /// </remarks>
    public interface IHashFunction
    {
        /// <summary>
        /// Apply the hashfunction to the object o passed as a parameter.  Uses the GetHashCode method from the object o.
        /// </summary>
        /// <param name="o">The object to be hashed</param>
        /// <returns>The result of the hash function</returns>
        int Hash(object o);

        int HashString(string x);
    }
}
