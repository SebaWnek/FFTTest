using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public class FFTParallelOptimisedDouble
    {
        public static ComplexDouble[][] omegasDouble;

        public static ComplexDouble CalculateOmegaD(int length, bool inverse)
        {
            //e^(i*x) = cos x + i sin x
            double l = length;
            //e^(i*x) = cos x + i sin x
            double exponent = -2 * Math.PI / l;
            if (inverse) exponent *= -1;
            return new ComplexDouble(Math.Cos(exponent), Math.Sin(exponent));
        }
        public static (ComplexDouble[], ComplexDouble[]) DivideArrayD(ComplexDouble[] input)
        {
            ComplexDouble[] Pe = new ComplexDouble[input.Length / 2];
            ComplexDouble[] Po = new ComplexDouble[input.Length / 2];
            for (int i = 0; i < input.Length / 2; i++)
            {
                Pe[i] = input[i * 2];
                Po[i] = input[i * 2 + 1];
            }
            return (Pe, Po);
        }

        public static ComplexDouble[] FFT(double[] input, bool newOmegas = true)
        {
            return FFT(input.Convert(), newOmegas);
        }

        private static ComplexDouble[] FFT(ComplexDouble[] complexDoubles, bool newOmegas = true)
        {
            int n = complexDoubles.Length;
            //if (!Helpers.CheckIfPowerOfTwo(n))
            //{
            //    throw new ArgumentException("Array length must e a power of 2!");
            //}
            int k = (int)Math.Log2(n);
            if (newOmegas)
            {
                omegasDouble = OmegaCalculatorDouble.GenerateOmegasDouble(k);
            }
            ComplexDouble[] result = FFTRecursive(complexDoubles, k);
            return result;
        }

        public static ComplexDouble[] FFTRecursive(ComplexDouble[] input, int depth)
        {
            int n = input.Length;
            //Check if array length is power of 2


            //if (n == 32) return FFTOptimisedKernels.Kernel32(input, ref omegas);
            //if (n == 16) return FFTOptimisedKernelsDouble.Kernel16(input, ref omegasDouble);
            if (n == 8) return FFTOptimisedKernelsDouble.Kernel8(input, ref omegasDouble);

            //If only one element - returns
            if (n == 1) return input;

            //Dividing into two arrays
            (ComplexDouble[] pe, ComplexDouble[] po) = DivideArrayD(input);

            ComplexDouble[] ye = FFTRecursive(pe, depth - 1);
            ComplexDouble[] yo = FFTRecursive(po, depth - 1);

            ComplexDouble[] y = new ComplexDouble[n];
            int halfn = n / 2;
            for (int i = 0; i < halfn; i++)
            {
                y[i] = ye[i] + omegasDouble[depth][i] * yo[i];
                y[i + n / 2] = ye[i] - omegasDouble[depth][i] * yo[i];
            }
            return y;
        }

        public static ComplexDouble[] IFFT(ComplexDouble[] complexDoubles, bool newOmegas = true)
        {
            int n = complexDoubles.Length;
            //}
            int k = (int)Math.Log2(n);
            if (newOmegas)
            {
                omegasDouble = OmegaCalculatorDouble.GenerateOmegasInvertedDouble(k);
            }
            ComplexDouble[] tmp = FFTRecursive(complexDoubles, k);
            ComplexDouble[] tmp2 = new ComplexDouble[n];
            for (int i = 0; i < n; i++)
            {
                tmp2[i] = tmp[i] / n;
            }
            return tmp2;
        }

        public static ComplexDouble[,] FFT(double[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegasDouble = OmegaCalculatorDouble.GenerateOmegasDouble(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexDouble[,] result = new ComplexDouble[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexDouble[] tmpRow;

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
                    ComplexDouble[] tmpCol;
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

        public static ComplexDouble[,] FFT(ComplexDouble[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegasDouble = OmegaCalculatorDouble.GenerateOmegasDouble(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexDouble[,] result = new ComplexDouble[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexDouble[] tmpRow;

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
                    ComplexDouble[] tmpCol;
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

        public static double[,] IFFT(ComplexDouble[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegasDouble = OmegaCalculatorDouble.GenerateOmegasDouble(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexDouble[,] result = new ComplexDouble[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexDouble[] tmpRow;

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
                    ComplexDouble[] tmpCol;
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

            return result.Convert();
        }
    }
}
