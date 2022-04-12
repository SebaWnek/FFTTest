using FastFourierTransform;
using FFT;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FFTTest
{
    class Program
    {
        static Random rnd = new Random();
        static Stopwatch stopwatch = new Stopwatch();
        static long t1, t2, t3, t4, t5, t6;
        static void Main(string[] args)
        {
            //TestKernels();
            //TestAVX();
            //TestFFT();
            //ComplexFloat[] input = { 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2 };

            //float[] inputFloat = {   1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2,
            //                         1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2 };


            #region fft
            //double t1 = 0, t2 = 0, sum1 = 0, sum2 = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    int size = (int)Math.Pow(2, 11);
            //    Console.WriteLine("Size: " + size);
            //    Console.WriteLine();
            //    float[,] inputFloat = new float[size, size];
            //    double[,] inputDouble = new double[size, size];
            //    inputFloat = FillRandom(size, size);
            //    inputDouble = FillRandomDouble(size, size);
            //    //ComplexFloat[,] input = inputFloat.Convert();
            //    ComplexFloat[,] result;
            //    ComplexDouble[,] resultDouble;
            //    float[,] resultBack;
            //    double[,] resultBackDouble;

            //    Console.WriteLine("Float:");
            //    sum1 = PrintSum(inputFloat);
            //    stopwatch.Restart();
            //    result = FFTParallelOptimised.FFT(inputFloat);
            //    stopwatch.Stop();
            //    //Print(result, true);
            //    Console.WriteLine();
            //    Console.WriteLine(t1 = stopwatch.ElapsedMilliseconds);
            //    //time2 += stopwatch.ElapsedMilliseconds;
            //    PrintSum(result);
            //    stopwatch.Restart();
            //    resultBack = FFTParallelOptimised.IFFT(result);
            //    stopwatch.Stop();
            //    Console.WriteLine();
            //    Console.WriteLine(t2 = stopwatch.ElapsedMilliseconds);
            //    sum2 = PrintSum(resultBack);
            //    Console.WriteLine();
            //    Console.WriteLine("Result: " + sum1 / sum2);
            //    Console.WriteLine();

            //    Console.WriteLine("Double:");
            //    sum1 = PrintSum(inputDouble);
            //    stopwatch.Restart();
            //    resultDouble = FFTParallelOptimised.FFT(inputDouble);
            //    stopwatch.Stop();
            //    //Print(result, true);
            //    Console.WriteLine();
            //    Console.WriteLine(t1 = stopwatch.ElapsedMilliseconds);
            //    //time2 += stopwatch.ElapsedMilliseconds;
            //    PrintSum(resultDouble);
            //    stopwatch.Restart();
            //    resultBackDouble = FFTParallelOptimised.IFFT(resultDouble);
            //    stopwatch.Stop();
            //    Console.WriteLine();
            //    Console.WriteLine(t2 = stopwatch.ElapsedMilliseconds);
            //    sum2 = PrintSum(resultBackDouble);
            //    Console.WriteLine();
            //    Console.WriteLine("Result: " + sum1 / sum2);
            //    Console.WriteLine();
            //    Console.WriteLine("---------------------------------------------------");
            //    Console.WriteLine();

            //stopwatch.Restart();
            //result = Helpers.ImportFromRGB(inputFloat);
            //stopwatch.Stop();
            //Console.WriteLine("Basic: " + stopwatch.ElapsedMilliseconds);
            //t1 += stopwatch.ElapsedTicks;
            //result = null;

            //stopwatch.Restart();
            //result = Helpers.ImportFromRGBThreads(inputFloat);
            //stopwatch.Stop();
            //Console.WriteLine("Threads: " + stopwatch.ElapsedMilliseconds);
            //t2 += stopwatch.ElapsedTicks;
            //result = null;

            //stopwatch.Restart();
            //result = Helpers.ImportFromRGBParallel(inputFloat);
            //stopwatch.Stop();
            //Console.WriteLine("Parallel: " + stopwatch.ElapsedMilliseconds);
            //t3 += stopwatch.ElapsedTicks;
            //result = null;

            //    Console.WriteLine();
            //}
            //Console.WriteLine($"Basic: {t1}\nThreads: {t2} {t1 / t2}x\nParallel: {t3} {t1 / t3}x");
            #endregion

            TestFloat();
            Thread.Sleep(1000);
            TestDouble();
            Thread.Sleep(1000);
            TestDoubleExtra();

            //TestTranspose();



        }

        private static void TestTranspose()
        {
            ComplexDouble[,] test = FillRandomDouble(2048, 2048).Convert();

            ComplexDouble[,] result;

            stopwatch.Start();

            for (int i = 0; i < 100; i++)
            {
                FFTParallelOptimisedDoubleExtra.Transpose(ref test, out result); 
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);  

            //Print(result);

            stopwatch.Restart();

            for (int i = 0; i < 100; i++)
            {
                result = FFTParallelOptimisedDoubleExtra.TransposeBasic(test); 
            }

            stopwatch.Stop();
            //Print(result);

            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        private static void TestFloat()
        {
            byte[] data = GetBytes().GetAwaiter().GetResult();
            int width = (int)Math.Sqrt(data.Length/4);

            ComplexFloat[,] result = FastFourierTransform.Helpers.ImportFromRGB(data, width);

            stopwatch.Restart();

            ComplexFloat[,] fft = FFTParallelOptimisedSingle.FFT(result);
            float[,] ifft = FFTParallelOptimisedSingle.IFFT(fft);

            stopwatch.Stop();
            t1 = stopwatch.ElapsedMilliseconds;

            byte[] fftBytes = FloatToByte(FastFourierTransform.Helpers.ShiftMiddle( fft.Log(2)));
            byte[] bytes = FloatToByte(ifft);

            double sum1 = PrintSum(result);
            double sum2 = PrintSum(ifft);

            Console.WriteLine("Result: " + sum1 / sum2);

            //Print(fftBytes);
            //Print(ifft);

            PrintImage(width, fftBytes, "img1fft.bmp");
            PrintImage(width, bytes, "img1returned.bmp");
            Console.WriteLine(t1);
        }



        private static void TestDouble()
        {
            byte[] data = GetBytes().GetAwaiter().GetResult();
            int width = (int)Math.Sqrt(data.Length / 4);

            Complex[,] result = FFT.Helpers.ImportFromArgbBitmap(data, width);

            stopwatch.Restart();
            
            Complex[,] fft = FFT.MainMethods.FFT(result);
            double[,] ifft = FFT.MainMethods.IFFT(fft);

            stopwatch.Stop();
            t1 = stopwatch.ElapsedMilliseconds;

            byte[] fftBytes = DoubleToByte(FFT.Helpers.ShiftToMiddle(fft.FitTo8Bits(2)));
            byte[] bytes = DoubleToByte(ifft);

            double sum1 = PrintSum(result);
            double sum2 = PrintSum(ifft);

            Console.WriteLine("Result: " + sum1 / sum2);

            //Print(fftBytes);
            //Print(ifft);


            PrintImage(width, fftBytes, "img2fft.bmp");
            PrintImage(width, bytes, "img2returned.bmp"); 
            Console.WriteLine(t1);
        }

        private static void TestDoubleExtra()
        {
            byte[] data = GetBytes().GetAwaiter().GetResult();
            int width = (int)Math.Sqrt(data.Length / 4);

            ComplexDouble[,] result = FastFourierTransform.Helpers.ImportFromRGBDouble(data, width);

            stopwatch.Restart();

            ComplexDouble[,] fft = FFTParallelOptimisedDoubleExtra.FFT(result);
            double[,] ifft = FFTParallelOptimisedDoubleExtra.IFFT(fft);

            stopwatch.Stop();
            t1 = stopwatch.ElapsedMilliseconds;

            byte[] fftBytes = DoubleToByte(FastFourierTransform.Helpers.ShiftMiddle(fft.Log(2)));
            byte[] bytes = DoubleToByte(ifft);

            double sum1 = PrintSum(result);
            double sum2 = PrintSum(ifft);

            Console.WriteLine("Result: " + sum1 / sum2);

            //Print(fftBytes);
            //Print(ifft);


            PrintImage(width, fftBytes, "img3fft.bmp");
            PrintImage(width, bytes, "img3returned.bmp");
            Console.WriteLine(t1);
        }

        private static void PrintImage(int width, byte[] bytes, string fileName)
        {
            Bitmap bmp = new Bitmap(width, width, PixelFormat.Format32bppArgb);
            Rectangle rectangle = new Rectangle(0, 0, width, width);
            BitmapData bitmapData = bmp.LockBits(rectangle, ImageLockMode.WriteOnly, bmp.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            bmp.UnlockBits(bitmapData);
            bmp.Save(fileName, ImageFormat.Bmp);
        }

        private static double[,] FillRandomDouble(int v1, int v2)
        {
            double[,] result = new double[v1, v2];
            for (int i = 0; i < v1; i++)
            {
                for (int j = 0; j < v2; j++)
                {
                    result[i, j] = rnd.Next(256);
                }
            }
            return result;
        }

        private static byte[] FloatToByte(float[,] ifft)
        {
            byte[] result = new byte[ifft.Length * 4];
            byte tmp2;
            int tmp;
            for (int i = 0; i < ifft.GetLength(0); i++)
            {
                for (int j = 0; j < ifft.GetLength(1); j++)
                {
                    tmp = (int)Math.Round(ifft[i, j]);
                    tmp2 = (byte)(tmp < 0 ? 0 : tmp > 255 ? 255 : tmp);
                    result[4 * i * ifft.GetLength(0) + 4 * j] = tmp2;
                    result[4 * i * ifft.GetLength(0) + 4 * j + 1] = tmp2;
                    result[4 * i * ifft.GetLength(0) + 4 * j + 2] = tmp2;
                    result[4 * i * ifft.GetLength(0) + 4 * j + 3] = 255;
                }
            }
            return result;
        }

        private static byte[] DoubleToByte(double[,] ifft)
        {
            byte[] result = new byte[ifft.Length * 4];
            byte tmp2;
            int tmp;
            for (int i = 0; i < ifft.GetLength(0); i++)
            {
                for (int j = 0; j < ifft.GetLength(1); j++)
                {
                    tmp = (int)Math.Round(ifft[i, j]);
                    tmp2 = (byte)(tmp < 0 ? 0 : tmp > 255 ? 255 : tmp);
                    result[4 * i * ifft.GetLength(0) + 4 * j] = tmp2;
                    result[4 * i * ifft.GetLength(0) + 4 * j + 1] = tmp2;
                    result[4 * i * ifft.GetLength(0) + 4 * j + 2] = tmp2;
                    result[4 * i * ifft.GetLength(0) + 4 * j + 3] = 255;
                }
            }
            return result;
        }

        private static async Task<byte[]> GetBytes()
        {
            byte[] array;
            Bitmap image;

            HttpClient client = new HttpClient();

            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/fft.jpg")));
            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/cartest.jpg")));
            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/horse.jpg")));
            image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/nastia.jpg")));
            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/suknia.jpg")));
            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/nastiabig.jpg")));
            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/cameraman.jpg")));
            //image = new Bitmap(Image.FromStream(await client.GetStreamAsync("https://wnekofoty.pl/asia.jpg")));

            client.Dispose();
            Rectangle rectanle = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData data = image.LockBits(rectanle, ImageLockMode.ReadWrite, image.PixelFormat);
            int bytes = Math.Abs(data.Stride) * image.Height;
            array = new byte[bytes];
            IntPtr ptr = data.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(ptr, array, 0, bytes);
            return array;
        }

        private static double PrintSum(float[,] result)
        {
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            double tmpSum = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tmpSum += result[i, j];
                }
            }

            Console.WriteLine(tmpSum);
            return tmpSum;
        }

        private static double PrintSum(double[,] result)
        {
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            double tmpSum = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tmpSum += result[i, j];
                }
            }

            Console.WriteLine(tmpSum);
            return tmpSum;
        }

        private static double PrintSum(ComplexFloat[,] result)
        {
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            double tmpSum = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tmpSum += result[i, j].Abs();
                }
            }

            Console.WriteLine(tmpSum);
            return tmpSum;
        }

        private static double PrintSum(ComplexDouble[,] result)
        {
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            double tmpSum = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tmpSum += result[i, j].Abs();
                }
            }

            Console.WriteLine(tmpSum);
            return tmpSum;
        }
        private static double PrintSum(Complex[,] result)
        {
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            double tmpSum = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tmpSum += result[i, j].Abs();
                }
            }

            Console.WriteLine(tmpSum);
            return tmpSum;
        }

        private static void TestAVX()
        {
            float[] test = new float[] { 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2, 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2 };
            float[] tmp = new float[] { 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2 };
            float[] tmp2 = new float[] { 1, 2, 3, 4, 4, 3, 2, 1 };
            ComplexFloat[] resultAvx = FFTParallelOptimisedSingle.FFT(test);
            Print(resultAvx, true);
        }

        private static void TestKernels()
        {
            float[] tmp = new float[] { 1, 2, 3, 4, 4, 3, 2, 1, 0, 6, 4, 3, 1, 11, 14, 2 };
            ComplexFloat[] test = tmp.Convert();

            FFTParallelOptimisedSingle.omegas = OmegaCalculatorSingle.GenerateOmegas(4);
            stopwatch.Start();
            Print(FFTOptimisedKernelsSingle.Kernel16(tmp, ref FFTParallelOptimisedSingle.omegas), true);
            for (int i = 0; i < 1000; i++)
            {
                FFTOptimisedKernelsSingle.Kernel16(tmp, ref FFTParallelOptimisedSingle.omegas);
            }
            stopwatch.Stop();
            t1 = stopwatch.ElapsedTicks;

            Console.WriteLine();
            stopwatch.Restart();
            Print(FFTParallelOptimisedSingle.FFT(tmp, false), true);
            for (int i = 0; i < 1000; i++)
            {
                FFTParallelOptimisedSingle.FFT(tmp, false);
            }
            stopwatch.Stop();
            t2 = stopwatch.ElapsedTicks;
            Console.WriteLine();
            Console.WriteLine(t1 + "   " + t2 + "   " + t1 / (float)t2);
        }

        private static void TestOmegas()
        {
            Print(OmegaCalculatorSingle.CalculateOmegasRowBasic(32), true);
            Console.WriteLine();
            Print(OmegaCalculatorSingle.CalculateOmegasRowOptimised(32), true);
            for (int n = 2; n < 12; n++)
            {
                int k = (int)Math.Pow(2, n);
                stopwatch.Restart();
                for (int i = 0; i < 1000; i++)
                {
                    OmegaCalculatorSingle.CalculateOmegasRowBasic(k);
                }
                stopwatch.Stop();
                t1 = stopwatch.ElapsedMilliseconds;
                t2 = stopwatch.ElapsedTicks;
                stopwatch.Restart();
                for (int i = 0; i < 1000; i++)
                {
                    OmegaCalculatorSingle.CalculateOmegasRowOptimised(k);
                }
                stopwatch.Stop();
                t3 = stopwatch.ElapsedMilliseconds;
                t4 = stopwatch.ElapsedTicks;
                Console.WriteLine($"{k,4} {t1,6} {t2,10} {t3,6} {t4,10} {t2 / t4}");
                //Console.WriteLine($"n = {k}\nBasic: {t1}ms\nOptimised: {t3}ms\nSpeedup: {t2 / t4}x"); 
            }
        }

        private static void TestFFT()
        {
            unsafe
            {
                int size = sizeof(ComplexFloat);
                Console.WriteLine(size);
            }
            int x = 1, y = (int)Math.Pow(2, 16);
            float[,] test = FillRandom(x, y);
            //float[,] test = { { 5f, 3f, 2f, 1f, 5f, 3f, 2f, 1f, 5f, 3f, 2f, 1f, 5f, 3f, 2f, 1f } };

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
            Console.WriteLine(t4 + "ms");

            stopwatch.Restart();
            ComplexFloat[,] resultAvx = FFTParallelOptimisedSingle.FFT(test);
            stopwatch.Stop();
            t5 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t5 + "ms");

            stopwatch.Restart();
            float[,] resultAvx2 = FFTParallelOptimisedSingle.IFFT(resultAvx);
            stopwatch.Stop();
            t6 = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(t6 + "ms");

            float[,] compareParAvx = Compare(test, resultAvx2);
            Console.WriteLine();
            Console.WriteLine("Sum of difference:");
            float s1 = Sum(compare);
            Console.WriteLine("Serial: " + s1 + "\nMean: " + s1 / (x * y));
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
            Console.WriteLine("Speedup Parallel, only FFT: " + t1 / (float)t3);
            Console.WriteLine("Speedup Parallel Optimised, only FFT: " + t1 / (float)t5);
            Console.WriteLine("Speedup Parallel: " + (t1 + t2) / (float)(t3 + t4));
            Console.WriteLine("Speedup Parallel Optimised: " + (t1 + t2) / (float)(t5 + t6));

            //Print(test);
            //Console.WriteLine();

            //Print(result);
            //Print(resultAvx);

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
        private static float[,,] FillRandom(int v1, int v2, int v3)
        {
            float[,,] result = new float[v1, v2, v3];
            for (int i = 0; i < v1; i++)
            {
                for (int j = 0; j < v2; j++)
                {
                    for (int k = 0; k < v3; k++)
                    {
                        result[i, j, k] = rnd.Next(256);
                    }
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
        private static void Print2<T>(T[] result, bool vertical = false)
        {

            for (int j = 0; j < result.Length; j++)
            {
                if (vertical)
                {
                    Console.WriteLine(result[j]);
                }
                else
                {
                    Console.Write($"{result[j]}, ");
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
