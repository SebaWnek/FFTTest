using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT
{
    /// <summary>
    /// Calculates omega values for FFT
    /// </summary>
    internal static class Omegas
    {

        #region FFT

        /// <summary>
        /// Returns omega values for given array length 
        /// </summary>
        /// <param name="n">Array length</param>
        /// <returns>Calculated omega values</returns>
        /// <exception cref="ArgumentException">Number of values must be a power of two</exception>
        private static Complex[] GenerateOmegasRow(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new Complex[] { new Complex(1, 0) },
                2 => new Complex[] { new Complex(1, 0) },
                4 => new Complex[] { new Complex(1, 0), new Complex(0, -1) },
                _ => CalculateOmegasRow(n)
            };
        }

        /// <summary>
        /// Calculates omega values for arrays of =>4 values
        /// </summary>
        /// <param name="n">Array length</param>
        /// <returns>Omega values</returns>
        private static Complex[] CalculateOmegasRow(int n)
        {
            int k = n / 4;

            Complex[] omegas = new Complex[n / 2];

            double tmp = 0;
            double tmp2 = 0;

            omegas[0].Re = 1;
            omegas[k].Im = -1;

            for (int i = 1; i < k; i++)
            {
                tmp = Math.Cos(i * 2 * Math.PI / n);
                tmp2 = -tmp;

                omegas[i].Re = tmp;
                omegas[2 * k - i].Re = tmp2;

                omegas[k - i].Im = tmp2;
                omegas[k + i].Im = tmp2;
            }

            return omegas;
        }

        /// <summary>
        /// Generates array of arrays with omega values for all recurency levels
        /// </summary>
        /// <param name="k">Number of recurency levels</param>
        /// <returns>Array of arryas with omega values</returns>
        internal static Complex[][] GenerateOmegas(int k)
        {
            List<Complex[]> tmp = new List<Complex[]>();
            for (int i = 0; i <= k; i++)
            {
                tmp.Add(GenerateOmegasRow((int)Math.Pow(2, i)));
            }
            return tmp.ToArray();
        }
        #endregion

        #region IFFT

        /// <summary>
        /// Returns omega values for given array length for inverted fft
        /// </summary>
        /// <param name="n">Array length</param>
        /// <returns>Calculated omega values</returns>
        /// <exception cref="ArgumentException">Number of values must be a power of two</exception>
        private static Complex[] GenerateOmegasRowInverted(int n)
        {
            if (!Helpers.CheckIfPowerOfTwo(n)) throw new ArgumentException("n must be power of 2");

            return n switch
            {
                1 => new Complex[] { new Complex(1, 0) },
                2 => new Complex[] { new Complex(1, 0) },
                4 => new Complex[] { new Complex(1, 0), new Complex(0, 1) },
                _ => CalculateOmegasRowInverted(n)
            };
        }

        /// <summary>
        /// Calculates omega values for arrays of =>4 values for inverted fft
        /// </summary>
        /// <param name="n">Array length</param>
        /// <returns>Omega values</returns>
        private static Complex[] CalculateOmegasRowInverted(int n)
        {
            int k = n / 4;

            Complex[] omegas = new Complex[n / 2];

            double tmp = 0;
            double tmp2 = 0;

            omegas[0].Re = 1;
            omegas[k].Im = 1;

            for (int i = 1; i < k; i++)
            {
                tmp = Math.Cos(i * 2 * Math.PI / n);
                tmp2 = -tmp;

                omegas[i].Re = tmp;
                omegas[2 * k - i].Re = tmp2;

                omegas[k - i].Im = tmp;
                omegas[k + i].Im = tmp;
            }

            return omegas;
        }

        /// <summary>
        /// Generates array of arrays with omega values for all recurency levels for inverted fft
        /// </summary>
        /// <param name="k">Number of recurency levels</param>
        /// <returns>Array of arryas with omega values</returns>
        internal static Complex[][] GenerateOmegasInverted(int k)
        {
            List<Complex[]> tmp = new List<Complex[]>();
            for (int i = 0; i <= k; i++)
            {
                tmp.Add(GenerateOmegasRowInverted((int)Math.Pow(2, i)));
            }
            return tmp.ToArray();
        }

        #endregion
    }
}
