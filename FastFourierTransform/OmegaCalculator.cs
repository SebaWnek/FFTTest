using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class OmegaCalculator
    {
        public static ComplexFloat[] CalculateOmegasBasic(int n)
        {
            //e^(i*x) = cos x + i sin x
            float exponent = (float)(2 * Math.PI / n); 
            ComplexFloat omega = new ComplexFloat((float)Math.Cos(exponent), (float)Math.Sin(exponent));

            ComplexFloat[] omegas = new ComplexFloat[n];

            for(int i = 0; i < n; i++)
            {
                omegas[i] = omega ^ i;
            }

            return omegas;
        }

        public static ComplexFloat[] CalculateOmegasOptimised(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new ComplexFloat[] { new ComplexFloat(1, 0) },
                2 => new ComplexFloat[] { new ComplexFloat(1, 0) },
                4 => new ComplexFloat[] { new ComplexFloat(1, 0), new ComplexFloat(0, -1)},
                _ => CalculateLargeOmegas(n)
            };
        }

        private static ComplexFloat[] CalculateLargeOmegas(int n)
        {
            int k = n / 4;

            ComplexFloat[] omegas = new ComplexFloat[n/2];

            float tmp = 0;
            float tmp2 = 0;

            omegas[0].Re = 1;
            omegas[k].Im = -1;
            //omegas[2 * k].Re = -1;
            //omegas[3 * k].Im = 1;

            for (int i = 1; i < k; i++)
            {
                tmp = (float)Math.Cos(i * 2 * (float)Math.PI / n);
                tmp2 = -tmp;

                omegas[i].Re = tmp;
                omegas[2 * k - i].Re = tmp2;
                //omegas[2 * k + i].Re = tmp2;
                //omegas[n - i].Re = tmp;

                omegas[k - i].Im = tmp2;
                omegas[k + i].Im = tmp2;
                //omegas[3 * k - i].Im = tmp;
                //omegas[3 * k + i].Im = tmp;
            }

            return omegas;
        }
    }
}
