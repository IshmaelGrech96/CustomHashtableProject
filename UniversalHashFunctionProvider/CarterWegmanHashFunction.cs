using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalHashFunctionProvider
{
    public class CarterWegmanHashFunction : IHashFunction
    {
        /// <summary>
        /// Initialisation of an instance of the CarterWegmanHashFunction.
        /// </summary>
        /// <param name="A">Multiplicative parameter, cannot be 0</param>
        /// <param name="B"></param>
        /// <param name="P">A prime number > M.  The value isn't checked for primality, so it's important that the caller verifies that P is prime, otherwise, a sub-optimal hash function may be produced</param>
        /// <param name="M">The size of the array. int.MaxValue if not passed as a parameter - if the value of M is not passed as a parameter, the hash value has to be mapped to the table by the user</param>
        internal CarterWegmanHashFunction(int A, int B, int M = int.MaxValue, long P = 87178291199)
        {
            if (A == 0)
            {
                throw new ArgumentException("The value of the multiplicative parameter cannot be 0!", "A");
            }

            if (P < M)
            {
                throw new ArgumentException("The value of P must be larger than M!", "P");
            }

            this.A = A;
            this.B = B;
            this.P = P;
            this.M = M;
        }

        /// <summary>
        /// Multiplicative Parameter
        /// </summary>
        internal int A { get; set; }

        /// <summary>
        /// Additive Parameter
        /// </summary>
        internal int B { get; set; }

        /// <summary>
        /// Prime number > M
        /// </summary>
        internal long P { get; set; }

        /// <summary>
        /// Array size, if available
        /// </summary>
        internal int M { get; set; } = int.MaxValue;

        public int Hash(object o)
        {
            return 
                (Math.BigMul(A, o.GetHashCode()) + B).Mod(P).Mod(M);
        }

        public int HashString(string x)
        {
            int h = x.GetHashCode();
            for(int i = 0; i < x.Length;i++)
            {
                 h = (Math.BigMul(h,A) + x[i]).Mod(P).Mod(M);
            }

            return h;
        }
    }
}
