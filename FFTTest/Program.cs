using FastFourierTransform;
using System;
using System.Diagnostics;

namespace FFTTest
{
    class Program
    {
        static Random rnd = new Random();
        static Stopwatch stopwatch = new Stopwatch();
        static long t1, t2, t3, t4, t5, t6;
        static void Main(string[] args)
        {
            unsafe
            {
                int size = sizeof(ComplexFloat);
                Console.WriteLine(size);
            }
            int x = 1024, y = 1024;
            float[,] test = FillRandom(x, y);

            //Print(test);
            Console.WriteLine();
            stopwatch.Start();
            ComplexFloat[,] result = FFTSerial.FFT(test);
            stopwatch.Stop();
            t1 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t1 + "ms");
            //Print(result);
            Console.WriteLine();
            stopwatch.Restart();
            float[,] result2 = FFTSerial.IFFT(result);
            stopwatch.Stop();
            t2 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t2 + "ms");
            //Print(result2);

            float[,] compare = Compare(test, result2);

            //Print(compare);


            stopwatch.Restart();
            ComplexFloat[,] resultPar = FFTParallel.FFT(test);
            stopwatch.Stop();
            t3 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t3 + "ms");
            stopwatch.Restart();
            float[,] resultPar2 = FFTParallel.IFFT(resultPar);
            stopwatch.Stop();
            float[,] comparePar = Compare(test, resultPar2);
            t4 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t4 +"ms");

            stopwatch.Restart();
            ComplexFloat[,] resultAvx = FFTParallelAvx.FFT(test);
            stopwatch.Stop();
            t5 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t5 + "ms");

            stopwatch.Restart();
            float[,] resultAvx2 = FFTParallelAvx.IFFT(resultAvx);
            stopwatch.Stop();
            t6 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t6+"ms");

            float[,] compareParAvx = Compare(test, resultAvx2);
            Console.WriteLine();
            Console.WriteLine("Sum of difference:");
            float s1 = Sum(compare);
            Console.WriteLine("Serial: " + s1 + "\nMean: " + s1/(x*y));
            float s2 = Sum(comparePar);
            Console.WriteLine("Parallel: " + s2 + "\nMean: " + s2 / (x * y));
            float s3 = Sum(compareParAvx);
            Console.WriteLine("Parallel Optimised: " + s3 + "\nMean: " + s3 / (x * y));

            Console.WriteLine();
            //float[,] fftCompare = Compare(result, resultPar);
            //float[,] ifftCompare = Compare(result2, resultPar2);
            //Console.WriteLine("Difference between methods:");
            //float s3 = Sum(fftCompare);
            //Console.WriteLine("FFT: " + s3 + "\nMean: " + s3/(x*y));
            //float s4 = Sum(ifftCompare);
            //Console.WriteLine("IFFT: " + s4 + "\nMean: " + s4/(x*y));
            Console.WriteLine();
            Console.WriteLine("Speedup Parallel: " + (t1 + t2)/(float)(t3+t4));
            Console.WriteLine("Speedup Parallel + AVX: " + (t1 + t2) / (float)(t5+t6));

            //Console.WriteLine("\n-------------------------------------------------------------\n");

            //float[] test2 = { 1f, 2f, 4f, 6f, 3f, 1f, 3f, 8f, 3f, 1f, 0f, 5f, 2f, 7f, 5f, 3f };

            //Print(test2);
            //Console.WriteLine("\n");

            //ComplexFloat[] result21 = FFTSerial.FFT(test2);

            //Print(result21, true);
            //Console.WriteLine("\n");

            //float[] result22 = FFTSerial.IFFT(result21).Convert();

            //Print(result22);
            //Console.WriteLine("\n");
        }

        private static float Sum(float[,] compare)
        {
            int rows = compare.GetLength(0);
            int cols = compare.GetLength(1);
            float result = 0f;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result += Math.Abs(compare[i, j]);
                }
            }
            return result;
        }

        private static float[,] FillRandom(int v1, int v2)
        {
            float[,] result = new float[v1, v2];
            for (int i = 0; i < v1; i++)
            {
                for (int j = 0; j < v2; j++)
                {
                    result[i, j] = rnd.Next(256);
                }
            }
            return result;
        }

        private static float[,] Compare(float[,] test, float[,] result2)
        {
            int rows = test.GetLength(0);
            int cols = test.GetLength(1);
            float[,] result = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = test[i, j] - result2[i, j];
                }
            }
            return result;
        }
        private static float[,] Compare(ComplexFloat[,] test, ComplexFloat[,] result2)
        {
            int rows = test.GetLength(0);
            int cols = test.GetLength(1);
            float[,] result = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = (test[i, j] - result2[i, j]).Abs();
                }
            }
            return result;
        }

        private static void Print<T>(T[] result, bool vertical = false)
        {

            for (int j = 0; j < result.Length; j++)
            {
                if (vertical)
                {
                    Console.WriteLine(result[j]);
                }
                else
                {
                    Console.Write($"({result[j]}) ");
                }
            }
        }

        private static void Print<T>(T[,] result, bool vertical = false)
        {
            Console.WriteLine();
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (vertical)
                    {
                        Console.WriteLine(result[i, j]);
                    }
                    else
                    {
                        Console.Write($"({result[i, j]}) ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
