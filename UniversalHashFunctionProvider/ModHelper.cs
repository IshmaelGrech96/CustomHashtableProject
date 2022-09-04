using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalHashFunctionProvider
{
    static class ModHelper
    {
        internal static int Mod(this int a, int b)
        {
            if (a < 0)
                return b + (a % b);
            else
                return a % b;
        }

        internal static int Mod(this long a, int b)
        {
            if (a < 0)
                return b + (int)(a % b);
            else
                return (int)(a % b);
        }

        internal static long Mod(this long a, long b)
        {
            if (a < 0)
                return b + (a % b);
            else
                return a % b;
        }
    }
}
