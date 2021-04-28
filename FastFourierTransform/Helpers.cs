using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class Helpers
    {
        public static bool CheckIfPowerOfTwo(int n)
        {
            if (n == 1) return true;
            double power = 2;
            while (power <= n)
            {
                if (power == n) return true;
                power *= 2;
            }
            return false;
        }
    }
}
