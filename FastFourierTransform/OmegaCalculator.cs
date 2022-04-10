﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class OmegaCalculator
    {

#region FFT
        public static ComplexFloat[] CalculateOmegasRowBasic(int n)
        {
            //e^(i*x) = cos x + i sin x
            float exponent = (float)(2 * Math.PI / n);
            ComplexFloat omega = new ComplexFloat((float)Math.Cos(exponent), (float)Math.Sin(exponent));

            ComplexFloat[] omegas = new ComplexFloat[n];

            for (int i = 0; i < n; i++)
            {
                omegas[i] = omega ^ i;
            }

            return omegas;
        }

        public static ComplexFloat[] CalculateOmegasRowOptimised(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new ComplexFloat[] { new ComplexFloat(1, 0) },
                2 => new ComplexFloat[] { new ComplexFloat(1, 0) },
                4 => new ComplexFloat[] { new ComplexFloat(1, 0), new ComplexFloat(0, -1) },
                _ => CalculateLargeOmegas(n)
            };
        }

        public static ComplexDouble[] CalculateOmegasRowOptimisedDouble(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new ComplexDouble[] { new ComplexDouble(1, 0) },
                2 => new ComplexDouble[] { new ComplexDouble(1, 0) },
                4 => new ComplexDouble[] { new ComplexDouble(1, 0), new ComplexDouble(0, -1) },
                _ => CalculateLargeOmegasDouble(n)
            };
        }

        private static ComplexFloat[] CalculateLargeOmegas(int n)
        {
            int k = n / 4;

            ComplexFloat[] omegas = new ComplexFloat[n / 2];

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

        private static ComplexDouble[] CalculateLargeOmegasDouble(int n)
        {
            int k = n / 4;

            ComplexDouble[] omegas = new ComplexDouble[n / 2];

            double tmp = 0;
            double tmp2 = 0;

            omegas[0].Re = 1;
            omegas[k].Im = -1;
            //omegas[2 * k].Re = -1;
            //omegas[3 * k].Im = 1;

            for (int i = 1; i < k; i++)
            {
                tmp = Math.Cos(i * 2 * Math.PI / n);
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

        internal static ComplexDouble[][] GenerateOmegasDouble(int k)
        {
            List<ComplexDouble[]> tmp = new List<ComplexDouble[]>();
            for (int i = 0; i <= k; i++)
            {
                tmp.Add(CalculateOmegasRowOptimisedDouble((int)Math.Pow(2, i)));
            }
            return tmp.ToArray();
        }

        public static ComplexFloat[][] GenerateOmegas(int k)
        {
            List<ComplexFloat[]> tmp = new List<ComplexFloat[]>();
            for (int i = 0; i <= k; i++)
            {
                tmp.Add(CalculateOmegasRowOptimised((int)Math.Pow(2, i)));
            }
            return tmp.ToArray();
        }
        #endregion

        #region IFFT
        public static ComplexFloat[] CalculateOmegasRowOptimisedInverted(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new ComplexFloat[] { new ComplexFloat(1, 0) },
                2 => new ComplexFloat[] { new ComplexFloat(1, 0) },
                4 => new ComplexFloat[] { new ComplexFloat(1, 0), new ComplexFloat(0, 1) },
                _ => CalculateLargeOmegasInverted(n)
            };
        }

        private static ComplexFloat[] CalculateLargeOmegasInverted(int n)
        {
            int k = n / 4;

            ComplexFloat[] omegas = new ComplexFloat[n / 2];

            float tmp = 0;
            float tmp2 = 0;

            omegas[0].Re = 1;
            omegas[k].Im = 1;
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

                omegas[k - i].Im = tmp;
                omegas[k + i].Im = tmp;
                //omegas[3 * k - i].Im = tmp;
                //omegas[3 * k + i].Im = tmp;
            }

            return omegas;
        }

        public static ComplexFloat[][] GenerateOmegasInverted(int k)
        {
            List<ComplexFloat[]> tmp = new List<ComplexFloat[]>();
            for (int i = 0; i <= k; i++)
            {
                tmp.Add(CalculateOmegasRowOptimisedInverted((int)Math.Pow(2, i)));
            }
            return tmp.ToArray();
        }

        internal static ComplexDouble[][] GenerateOmegasInvertedDouble(int k)
        {
            List<ComplexDouble[]> tmp = new List<ComplexDouble[]>();
            for (int i = 0; i <= k; i++)
            {
                tmp.Add(CalculateOmegasRowOptimisedInvertedDouble((int)Math.Pow(2, i)));
            }
            return tmp.ToArray();
        }

        private static ComplexDouble[] CalculateOmegasRowOptimisedInvertedDouble(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new ComplexDouble[] { new ComplexDouble(1, 0) },
                2 => new ComplexDouble[] { new ComplexDouble(1, 0) },
                4 => new ComplexDouble[] { new ComplexDouble(1, 0), new ComplexDouble(0, 1) },
                _ => CalculateLargeOmegasInvertedDouble(n)
            };
        }

        private static ComplexDouble[] CalculateLargeOmegasInvertedDouble(int n)
        {
            int k = n / 4;

            ComplexDouble[] omegas = new ComplexDouble[n / 2];

            double tmp = 0;
            double tmp2 = 0;

            omegas[0].Re = 1;
            omegas[k].Im = 1;
            //omegas[2 * k].Re = -1;
            //omegas[3 * k].Im = 1;

            for (int i = 1; i < k; i++)
            {
                tmp = Math.Cos(i * 2 * Math.PI / n);
                tmp2 = -tmp;

                omegas[i].Re = tmp;
                omegas[2 * k - i].Re = tmp2;
                //omegas[2 * k + i].Re = tmp2;
                //omegas[n - i].Re = tmp;

                omegas[k - i].Im = tmp;
                omegas[k + i].Im = tmp;
                //omegas[3 * k - i].Im = tmp;
                //omegas[3 * k + i].Im = tmp;
            }

            return omegas;
        }

        #endregion
    }
}
