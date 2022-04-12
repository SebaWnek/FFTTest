using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFT
{
    /// <summary>
    /// Extension methods needed for FFT calculations and allowing some additional functionality on complex number arrays
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns array containing selected row of 2d array
        /// </summary>
        /// <typeparam name="T">Any data type needed</typeparam>
        /// <param name="input">Input 2d array</param>
        /// <param name="row">Row number</param>
        /// <returns>Selected row</returns>
        internal static T[] GetRow<T>(this T[,] input, int row)
        {
            int cols = input.GetLength(1);
            return Enumerable.Range(0, cols).Select(x => input[row, x]).ToArray();
        }

        /// <summary>
        /// Returns array containing selected column of 2d array
        /// </summary>
        /// <typeparam name="T">Any data type needed</typeparam>
        /// <param name="input">Input 2d array</param>
        /// <param name="col">Column number</param>
        /// <returns>Selected column</returns>
        internal static T[] GetCol<T>(this T[,] input, int col)
        {
            int rows = input.GetLength(0);
            return Enumerable.Range(0, rows).Select(x => input[x, col]).ToArray();

        }

        /// <summary>
        /// Converts floating point 1d array into complex numbers 1d array keeping it's values as real part
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>Complex array with same values as input</returns>
        public static Complex[] ConvertType(this double[] x)
        {
            Complex[] result = new Complex[x.Length];
            for (int i = 0; i < x.Length; i++) result[i] = x[i];
            return result;
        }

        /// <summary>
        /// Converts complex numbers 1d array to floating point real numbers 1d array keeping only real parts
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>Floating point array containing real parts of input array</returns>
        public static double[] ConvertType(this Complex[] input)
        {
            double[] result = new double[input.Length];
            for (int i = 0; i < input.Length; i++) result[i] = input[i].Re;
            return result;
        }

        /// <summary>
        /// Converts complex numbers 2d array to floating point real numbers 2d array keeping only real parts
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>Floating point array containing real parts of input array</returns>
        public static double[,] ConvertType(this Complex[,] input)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            double[,] result = new double[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = (double)input[i, j];
                }
            });
            return result;
        }

        /// <summary>
        /// Converts floating point 2d array into complex numbers 2d array keeping it's values as real part
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>Complex array with same values as input</returns>
        public static Complex[,] ConvertType(this double[,] input)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            Complex[,] result = new Complex[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = input[i, j];
                }
            });
            return result;
        }

        /// <summary>
        /// Calculates array of absolute values of complex numbers array
        /// </summary>
        /// <param name="input">Input 2d array of complex numbers</param>
        /// <returns>Calculated floating point 2d array</returns>
        public static double[,] Abs(this Complex[,] input)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            double[,] result = new double[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = input[i, j].Abs();
                }
            });
            return result;
        }

        /// <summary>
        /// Transforms 2d complex numbers array into floating point values from range 0-255 to be used to generate bitmap representation.
        /// Uses absolute values of complex numbers and logarithm to flatten range.
        /// </summary>
        /// <param name="input">Input complex 2d array</param>
        /// <param name="base">Logarithm base</param>
        /// <returns>Calculated floating point 2d array</returns>
        public static double[,] FitTo8Bits(this Complex[,] input, double @base = 2)
        {
            double tmpMax = 0;
            double max = 255;

            int rows = input.GetLength(0);
            int cols = input.GetLength(1);

            double[,] result = new double[rows, cols];

            for(int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = Math.Log(input[i, j].Abs(), @base);
                    if (result[i, j] > tmpMax) tmpMax = result[i, j];
                }
            };

            double divider = tmpMax / max;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] /= divider;
                }
            };

            return result;
        }
    }
}
