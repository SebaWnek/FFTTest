using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFT
{
    /// <summary>
    /// Main part of library, containing public classes, for:
    /// - FFT of floating point 1d and 2d arrays
    /// - FFT of complex 1d and 2d arrays
    /// - Inverse FFT of 1d and 2d arrays
    /// No IFFT of floating point numbers, as by definiton operates only on complex numbers
    /// </summary>
    public static class MainMethods
    {
        private static Complex[][] omegas = { };

        /// <summary>
        /// FFT of one-dimmensional array of floating point real numbers
        /// </summary>
        /// <param name="input">Data to be transformed</param>
        /// <param name="newOmegas">If omega values should be calculated again. 
        /// True only first time, false in subsequent calculations, as they can use previously calculated values</param>
        /// <returns>FFT of input data</returns>
        public static Complex[] FFT(double[] input, bool newOmegas = true)
        {
            return FFT(input.ConvertType(), newOmegas);
        }

        /// <summary>
        /// FFT of one-dimmensional array of complex numbers
        /// </summary>
        /// <param name="input">Data to be transformed</param>
        /// <param name="newOmegas">If omega values should be calculated again. 
        /// True only first time, false in subsequent calculations, as they can use previously calculated values</param>
        /// <returns>FFT of input data</returns>
        private static Complex[] FFT(Complex[] input, bool newOmegas = true)
        {
            int n = input.Length;
            //if (!Helpers.CheckIfPowerOfTwo(n))
            //{
            //    throw new ArgumentException("Array length must e a power of 2!");
            //}
            int k = (int)Math.Log2(n);
            if (newOmegas)
            {
                omegas = Omegas.GenerateOmegas(k);
            }
            Complex[] result = FFTRecursive(ref input, k);
            return result;
        }

        /// <summary>
        /// Inverse FFT of one-dimmensional array of complex numbers
        /// </summary>
        /// <param name="input">Data to be transformed</param>
        /// <param name="newOmegas">If omega values should be calculated again. 
        /// True only first time, false in subsequent calculations, as they can use previously calculated values</param>
        /// <returns>FFT of input data</returns>
        public static Complex[] IFFT(Complex[] input, bool newOmegas = true)
        {
            int n = input.Length;
            //}
            int k = (int)Math.Log2(n);
            if (newOmegas)
            {
                omegas = Omegas.GenerateOmegasInverted(k);
            }
            Complex[] tmp = FFTRecursive(ref input, k);
            Complex[] tmp2 = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                tmp2[i] = tmp[i] / n;
            }
            return tmp2;
        }

        /// <summary>
        /// FFT of two-dimmensional array of floating point real numbers
        /// </summary>
        /// <param name="input">Data to be transformed</param>
        /// <returns>FFT of input data</returns>
        public static Complex[,] FFT(double[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = Omegas.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            Complex[,] result = new Complex[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    Complex[] tmpRow;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        tmpRow = FFT(input.GetRow(k), false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = tmpRow[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    Complex[] tmpCol;
                    for (int k = columnParts * tmp; k < columnParts * (tmp + 1); k++)
                    {
                        tmpCol = FFT(result.GetCol(k), false);
                        for (int j = 0; j < rows; j++)
                        {
                            result[j, k] = tmpCol[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            return result;
        }

        /// <summary>
        /// FFT of two-dimmensional array of complex numbers
        /// </summary>
        /// <param name="input">Data to be transformed</param>
        /// <returns>FFT of input data</returns>
        public static Complex[,] FFT(Complex[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = Omegas.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            Complex[,] result = new Complex[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    Complex[] tmpRow;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        tmpRow = FFT(input.GetRow(k), false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = tmpRow[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    Complex[] tmpCol;
                    for (int k = columnParts * tmp; k < columnParts * (tmp + 1); k++)
                    {
                        tmpCol = FFT(result.GetCol(k), false);
                        for (int j = 0; j < rows; j++)
                        {
                            result[j, k] = tmpCol[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            return result;
        }

        /// <summary>
        /// Inverse FFT of two-dimmensional array of complex numbers
        /// </summary>
        /// <param name="input">Data to be transformed</param>
        /// <returns>FFT of input data</returns>
        public static double[,] IFFT(Complex[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = Omegas.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            Complex[,] result = new Complex[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    Complex[] tmpRow;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        tmpRow = IFFT(input.GetRow(k), false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = tmpRow[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    Complex[] tmpCol;
                    for (int k = columnParts * tmp; k < columnParts * (tmp + 1); k++)
                    {
                        tmpCol = IFFT(result.GetCol(k), false);
                        for (int j = 0; j < rows; j++)
                        {
                            result[j, k] = tmpCol[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            return result.ConvertType();
        }

        /// <summary>
        /// Main part calculating FFT using Cooley–Tukey algorithm
        /// </summary>
        /// <param name="input">Imput data</param>
        /// <param name="depth">Current recursion depth</param>
        /// <returns>FFT of input data</returns>
        private static Complex[] FFTRecursive(ref Complex[] input, int depth)
        {
            int n = input.Length;

            //Left in case I'll repair Kernel16 and Kernel32 methods, altough they didn't seemed to be faster anyway
            //if (n == 32) return FFTOptimisedKernels.Kernel32(input, ref omegas);
            //if (n == 16) return FFTOptimisedKernelsDouble.Kernel16(input, ref omegasDouble);
            if (n == 8) return Kernels.Kernel8(input, ref omegas);

            //If only one element - returns
            if (n == 1) return input;

            //Dividing into two arrays
            (Complex[] pe, Complex[] po) = DivideArray(ref input);

            Complex[] ye = FFTRecursive(ref pe, depth - 1);
            Complex[] yo = FFTRecursive(ref po, depth - 1);

            Complex[] y = new Complex[n];
            int halfn = n / 2;
            for (int i = 0; i < halfn; i++)
            {
                y[i] = ye[i] + omegas[depth][i] * yo[i];
                y[i + n / 2] = ye[i] - omegas[depth][i] * yo[i];
            }
            return y;
        }

        /// <summary>
        /// Divides array for Cooley–Tukey algorithm
        /// </summary>
        /// <param name="input">Input array</param>
        /// <returns>Tuple contaiinng two resulting arrays</returns>
        private static (Complex[], Complex[]) DivideArray(ref Complex[] input)
        {
            Complex[] Pe = new Complex[input.Length / 2];
            Complex[] Po = new Complex[input.Length / 2];
            for (int i = 0; i < input.Length / 2; i++)
            {
                Pe[i] = input[i * 2];
                Po[i] = input[i * 2 + 1];
            }
            return (Pe, Po);
        }
    }
}
