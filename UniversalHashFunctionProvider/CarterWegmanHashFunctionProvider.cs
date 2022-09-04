using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalHashFunctionProvider
{
    public class CarterWegmanHashFunctionProvider : IHashFunctionProvider
    {
        private static readonly object o = new object();
        private static Random r;

        public CarterWegmanHashFunctionProvider()
        {
            if (r == null)
            {
                lock (o)
                {
                    if (r == null)
                    {
                        r = new Random();
                    }
                }
            }
        }

        public CarterWegmanHashFunctionProvider(int seed)
        {
            if (r == null)
            {
                lock (o)
                {
                    if (r == null)
                    {
                        r = new Random(seed);
                    }
                    else
                    {
                        throw new InvalidOperationException("Multiple initialisations with a seed are forbidden as they can lead to a race-condition");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Multiple initialisations with a seed are forbidden as they can lead to a race-condition");
            }
        }

        public IHashFunction GetHashFunction()
        {
            int A = r.Next(1, int.MaxValue);
            int B = r.Next();

            return new CarterWegmanHashFunction(A, B);
        }

        public IHashFunction GetHashFunction(int M)
        {
            int A = r.Next(1, int.MaxValue);
            int B = r.Next();

            return new CarterWegmanHashFunction(A, B, M);
        }
    }
}
