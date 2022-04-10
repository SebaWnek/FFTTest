using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class FFTParallelOptimisedSingle
    {
        public static ComplexFloat[][] omegas;
        public static ComplexFloat CalculateOmegaS(int length, bool inverse)
        {
            float l = (float)length;
            //e^(i*x) = cos x + i sin x
            float exponent = -2 * (float)Math.PI / l;
            if (inverse) exponent *= -1;
            return new ComplexFloat((float)Math.Cos(exponent), (float)Math.Sin(exponent));
        }

        
        //public static ComplexFloat[] FFTBase(ComplexFloat[] input, bool inverse)
        //{
        //    int n = input.Length;
        //    //Check if array length is power of 2
        //    //if (!Helpers.CheckIfPowerOfTwo(n))
        //    //{
        //    //    throw new ArgumentException("Array length must e a power of 2!");
        //    //}

        //    //If only one element - returns

        //    if (n == 1) return input;

        //    if (n == 32) return FFTOptimisedKernels.Kernel32(input, ref omegas);

        //    ComplexFloat omega = CalculateOmegaS(n, inverse);
        //    //Dividing into two arrays
        //    (ComplexFloat[] pe, ComplexFloat[] po) = DivideArrayS(input);

        //    ComplexFloat[] ye = FFTBase(pe, inverse);
        //    ComplexFloat[] yo = FFTBase(po, inverse);

        //    ComplexFloat[] y = new ComplexFloat[n];
        //    int halfn = n / 2;
        //    for (int i = 0; i < halfn; i++)
        //    {
        //        y[i] = ye[i] + omega.Pow(i) * yo[i];
        //        y[i + n / 2] = ye[i] - omega.Pow(i) * yo[i];
        //    }
        //    return y;


        //}

        public static (ComplexFloat[], ComplexFloat[]) DivideArrayS(ComplexFloat[] input)
        {
            ComplexFloat[] Pe = new ComplexFloat[input.Length / 2];
            ComplexFloat[] Po = new ComplexFloat[input.Length / 2];
            for (int i = 0; i < input.Length / 2; i++)
            {
                Pe[i] = input[i * 2];
                Po[i] = input[i * 2 + 1];
            }
            return (Pe, Po);
        }

        public static ComplexFloat[] FFT(float[] input, bool newOmegas = true)
        {
            return FFT(input.Convert(), newOmegas);
        }

        private static ComplexFloat[] FFT(ComplexFloat[] complexFloats, bool newOmegas = true)
        {
            int n = complexFloats.Length;
            //if (!Helpers.CheckIfPowerOfTwo(n))
            //{
            //    throw new ArgumentException("Array length must e a power of 2!");
            //}
            int k = (int)Math.Log2(n);
            if (newOmegas)
            {
                omegas = OmegaCalculatorSingle.GenerateOmegas(k);
            }
            ComplexFloat[] result = FFTRecursive(complexFloats, k);
            return FFTRecursive(complexFloats, k);
        }

        public static ComplexFloat[] FFTRecursive(ComplexFloat[] input, int depth)
        {
            int n = input.Length;
            //Check if array length is power of 2


            //if (n == 32) return FFTOptimisedKernelsSingle.Kernel32(input, ref omegas);
            //if (n == 16) return FFTOptimisedKernelsSingle.Kernel16(input, ref omegas);
            if (n == 8) return FFTOptimisedKernelsSingle.Kernel8(input, ref omegas);

            //If only one element - returns
            if (n == 1) return input;

            //Dividing into two arrays
            (ComplexFloat[] pe, ComplexFloat[] po) = DivideArrayS(input);

            ComplexFloat[] ye = FFTRecursive(pe, depth - 1);
            ComplexFloat[] yo = FFTRecursive(po, depth - 1);

            ComplexFloat[] y = new ComplexFloat[n];
            int halfn = n / 2;
            for (int i = 0; i < halfn; i++)
            {
                y[i] = ye[i] + omegas[depth][i] * yo[i];
                y[i + n / 2] = ye[i] - omegas[depth][i] * yo[i];
            }
            return y;
        }

        public static ComplexFloat[] IFFT(ComplexFloat[] complexFloats, bool newOmegas = true)
        {
            int n = complexFloats.Length;
            //}
            int k = (int)Math.Log2(n);
            if (newOmegas)
            {
                omegas = OmegaCalculatorSingle.GenerateOmegasInverted(k);
            }
            ComplexFloat[] tmp = FFTRecursive(complexFloats, k);
            ComplexFloat[] tmp2 = new ComplexFloat[n];
            for(int i = 0; i < n; i++)
            {
                tmp2[i] = tmp[i] / n;
            }
            return tmp2;
        }

        public static ComplexFloat[,] FFTOld(float[,] input)
        {
            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = OmegaCalculatorSingle.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexFloat[,] result = new ComplexFloat[rows, columns];
            Parallel.For(0, rows, (i) =>
            //for(int i = 0; i < rows; i++)
            {
                ComplexFloat[] tmpRow = FFT(input.GetRow(i), false);
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = tmpRow[j];
                }
            });

            Parallel.For(0, columns, (i) =>
            {
                ComplexFloat[] tmpCol = FFT(result.GetCol(i), false);
                for (int j = 0; j < rows; j++)
                {
                    result[j, i] = tmpCol[j];
                }
            });
            return result;
        }

        public static ComplexFloat[,] FFT(float[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = OmegaCalculatorSingle.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexFloat[,] result = new ComplexFloat[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexFloat[] tmpRow;

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
                    ComplexFloat[] tmpCol;
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

        public static ComplexFloat[,] FFT(ComplexFloat[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = OmegaCalculatorSingle.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexFloat[,] result = new ComplexFloat[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexFloat[] tmpRow;

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
                    ComplexFloat[] tmpCol;
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

        public static float[,] IFFT(ComplexFloat[,] input)
        {
            int threadCount = Environment.ProcessorCount;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            int rowParts = rows / threadCount;
            int columnParts = columns / threadCount;

            int k = (int)Math.Log2(rows >= columns ? rows : columns);
            omegas = OmegaCalculatorSingle.GenerateOmegas(k);

            if (!Helpers.CheckIfPowerOfTwo(rows) || !Helpers.CheckIfPowerOfTwo(columns))
            {
                throw new ArgumentException("Array length must be a power of 2!");
            }

            ComplexFloat[,] result = new ComplexFloat[rows, columns];

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexFloat[] tmpRow;

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
                    ComplexFloat[] tmpCol;
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
