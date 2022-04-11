using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public class FFTParallelOptimisedDoubleExtra
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
            ComplexDouble[] result = FFTRecursive(ref complexDoubles, k);
            return result;
        }

        public static ComplexDouble[] FFTRecursive(ref ComplexDouble[] input, int depth)
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

            ComplexDouble[] ye = FFTRecursive(ref pe, depth - 1);
            ComplexDouble[] yo = FFTRecursive(ref po, depth - 1);

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
            ComplexDouble[] tmp = FFTRecursive(ref complexDoubles, k);
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
                    double[] tmpRow;
                    ComplexDouble[] cplxTmp;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        GetRow(ref input, columns, k, out tmpRow);
                        cplxTmp = FFT(tmpRow, false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = cplxTmp[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();


            ComplexDouble[,] result2;
            Transpose(ref result, out result2);

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexDouble[] tmpRow; 
                    ComplexDouble[] cplxRow;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        GetRow(ref result2, columns, k, out tmpRow);
                        cplxRow = FFT(tmpRow, false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = cplxRow[j];
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
                    ComplexDouble[] cplxTmp;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        GetRow(ref input, columns, k, out tmpRow);
                        cplxTmp = FFT(tmpRow, false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = cplxTmp[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            ComplexDouble[,] result2;
            Transpose(ref result, out result2);

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexDouble[] tmpRow;
                    ComplexDouble[] cplxRow;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        GetRow(ref result2, columns, k, out tmpRow);
                        cplxRow = FFT(tmpRow, false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = cplxRow[j];
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
                    ComplexDouble[] cplxTmp;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        GetRow(ref input, columns, k, out tmpRow);
                        cplxTmp = IFFT(tmpRow, false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = cplxTmp[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            ComplexDouble[,] result2;
            Transpose(ref result, out result2);

            for (int i = 0; i < threadCount; i++)
            {
                int tmp = i;
                threads[i] = new Thread(() =>
                {
                    ComplexDouble[] tmpRow;
                    ComplexDouble[] cplxRow;

                    for (int k = rowParts * tmp; k < rowParts * (tmp + 1); k++)
                    {
                        GetRow(ref result2, columns, k, out tmpRow);
                        cplxRow = IFFT(tmpRow, false);
                        for (int j = 0; j < columns; j++)
                        {
                            result[k, j] = cplxRow[j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (Thread t in threads) t.Join();

            return result.Convert();
        }

        public static unsafe ComplexDouble[,] Transpose(ref ComplexDouble[,] input, out ComplexDouble[,] result)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            result = new ComplexDouble[cols, rows];

            fixed (ComplexDouble* iPtr = input, rPtr = result)
            {
                byte* inPtr = (byte*)iPtr;
                byte* resPtr = (byte*)rPtr;
                Vector128<byte> tmp;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        tmp = Sse3.LoadVector128(inPtr + (i*cols + j) * 16);
                        Sse3.Store(resPtr + (j * rows + i) * 16, tmp);
                    }
                } 
            }

            return result;
        }

        public static unsafe ComplexDouble[,] TransposeBasic(ComplexDouble[,] input)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            ComplexDouble[,] result = new ComplexDouble[cols, rows];

            fixed (ComplexDouble* iPtr = input, rPtr = result)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[j, i] = input[i, j];
                    }
                }
            }

            return result;
        }

        private static unsafe double[] GetRow(ref double[,] input, int length, int rowNumber, out double[] result)
        {
            result = new double[length];
            int rowShift = rowNumber * length * sizeof(double);
            fixed (double* rowPtr = input, resPtr = result)
            {
                byte* row = (byte*)rowPtr;
                byte* res = (byte*)resPtr;

                row += rowShift;

                for(int i = 0; i < length * sizeof(double); i += 256)
                {
                    Avx2.Store(res + i      , Avx2.LoadVector256(row + i      ));
                    Avx2.Store(res + i + 32 , Avx2.LoadVector256(row + i + 32 ));
                    Avx2.Store(res + i + 64 , Avx2.LoadVector256(row + i + 64 ));
                    Avx2.Store(res + i + 96 , Avx2.LoadVector256(row + i + 96 ));
                    Avx2.Store(res + i + 128, Avx2.LoadVector256(row + i + 128));
                    Avx2.Store(res + i + 160, Avx2.LoadVector256(row + i + 160));
                    Avx2.Store(res + i + 192, Avx2.LoadVector256(row + i + 192));
                    Avx2.Store(res + i + 224, Avx2.LoadVector256(row + i + 224));
                }
            }
            return result;
        }

        private static unsafe ComplexDouble[] GetRow(ref ComplexDouble[,] input, int length, int rowNumber, out ComplexDouble[] result)
        {
            result = new ComplexDouble[length];
            int rowShift = rowNumber * length * sizeof(ComplexDouble);
            fixed (ComplexDouble* rowPtr = input, resPtr = result)
            {
                byte* row = (byte*)rowPtr;
                byte* res = (byte*)resPtr;

                row += rowShift;

                for (int i = 0; i < length * sizeof(ComplexDouble); i += 512)
                {
                    Avx2.Store(res + i      , Avx2.LoadVector256(row + i      ));
                    Avx2.Store(res + i +  32, Avx2.LoadVector256(row + i +  32));
                    Avx2.Store(res + i +  64, Avx2.LoadVector256(row + i +  64));
                    Avx2.Store(res + i +  96, Avx2.LoadVector256(row + i +  96));
                    Avx2.Store(res + i + 128, Avx2.LoadVector256(row + i + 128));
                    Avx2.Store(res + i + 160, Avx2.LoadVector256(row + i + 160));
                    Avx2.Store(res + i + 192, Avx2.LoadVector256(row + i + 192));
                    Avx2.Store(res + i + 224, Avx2.LoadVector256(row + i + 224));

                    Avx2.Store(res + i       + 256, Avx2.LoadVector256(row + i       + 256));
                    Avx2.Store(res + i +  32 + 256, Avx2.LoadVector256(row + i +  32 + 256));
                    Avx2.Store(res + i +  64 + 256, Avx2.LoadVector256(row + i +  64 + 256));
                    Avx2.Store(res + i +  96 + 256, Avx2.LoadVector256(row + i +  96 + 256));
                    Avx2.Store(res + i + 128 + 256, Avx2.LoadVector256(row + i + 128 + 256));
                    Avx2.Store(res + i + 160 + 256, Avx2.LoadVector256(row + i + 160 + 256));
                    Avx2.Store(res + i + 192 + 256, Avx2.LoadVector256(row + i + 192 + 256));
                    Avx2.Store(res + i + 224 + 256, Avx2.LoadVector256(row + i + 224 + 256));
                }
            }
            return result;
        }
    }
}
