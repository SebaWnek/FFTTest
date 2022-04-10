using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class Helpers
    {
        public static T[,] ShiftMiddle<T>(T[,] input)
        {
            int h = input.GetLength(0), w = input.GetLength(1);

            T[,] output = new T[h, w];

            for (int i = 0; i < h / 2; i++)
            {
                for (int j = 0; j < w / 2; j++)
                {
                    output[i,j] = input[i + h/2, j+h/2];
                }
            }

            for (int i = 0; i < h / 2; i++)
            {
                for (int j = w / 2; j < w; j++)
                {
                    output[i,j] = input[i+h/2, j-h/2];
                }
            }

            for (int i = h / 2; i < h; i++)
            {
                for (int j = 0; j < w / 2; j++)
                {
                    output[i,j] = input[i - h / 2, j+h/2];
                }
            }

            for (int i = h / 2; i < h; i++)
            {
                for (int j = w / 2; j < w; j++)
                {
                    output[i, j] = input[i - h / 2, j - w / 2];
                }
            }

            return output;
        }

        public static bool CheckIfPowerOfTwo(int n)
        {
            if (n == 1) return true;
            double power = 2;
            while (power <= n)
            {
                if (power == n) return true;
                power *= 2;
            }
            return false;
        }

        public static ComplexFloat[,] ImportFromRGB(byte[] data, int width)
        {
            int heigth = data.Length / (4 * width);
            ComplexFloat[,] result = new ComplexFloat[heigth, width];
            int position;
            for (int i = 0; i < heigth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    position = i * width * 4 + 4 * j;
                    result[i, j] = ((float)data[position] + data[position + 1] + data[position + 2]) / 3;
                }
            }

            return result;
        }

        public static ComplexDouble[,] ImportFromRGBDouble(byte[] data, int width)
        {
            int heigth = data.Length / (4 * width);
            ComplexDouble[,] result = new ComplexDouble[heigth, width];
            int position;
            for (int i = 0; i < heigth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    position = i * width * 4 + 4 * j;
                    result[i, j] = ((float)data[position] + data[position + 1] + data[position + 2]) / 3;
                }
            }

            return result;
        }

        public static ComplexFloat[,] ImportFromRGB(float[,,] data)
        {
            int h = data.GetLength(0);
            int w = data.GetLength(1);
            double logH = Math.Log2(h);
            double logW = Math.Log2(w);
            double roundLogH = Math.Ceiling(logH);
            double roundLogW = Math.Ceiling(logW);
            int newH = (int)Math.Pow(2, roundLogH);
            int newW = (int)Math.Pow(2, roundLogW);

            ComplexFloat[,] array = new ComplexFloat[h, w];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    array[i, j] = (data[i, j, 0] + data[i, j, 1] + data[i, j, 2]) / 3;
                }
            }

            return array;
        }

        public static ComplexFloat[,] ImportFromRGBParallel(float[,,] data)
        {
            int h = data.GetLength(0);
            int w = data.GetLength(1);
            double logH = Math.Log2(h);
            double logW = Math.Log2(w);
            double roundLogH = Math.Ceiling(logH);
            double roundLogW = Math.Ceiling(logW);
            int newH = (int)Math.Pow(2, roundLogH);
            int newW = (int)Math.Pow(2, roundLogW);

            ComplexFloat[,] array = new ComplexFloat[h, w];
            Parallel.For(0, h, (i) =>
            {
                for (int j = 0; j < w; j++)
                {
                    array[i, j] = (data[i, j, 0] + data[i, j, 1] + data[i, j, 2]) / 3;
                }
            });

            return array;
        }

        public static ComplexFloat[,] ImportFromRGBThreads(float[,,] data)
        {
            int h = data.GetLength(0);
            int w = data.GetLength(1);
            double logH = Math.Log2(h);
            double logW = Math.Log2(w);
            double roundLogH = Math.Ceiling(logH);
            double roundLogW = Math.Ceiling(logW);
            int newH = (int)Math.Pow(2, roundLogH);
            int newW = (int)Math.Pow(2, roundLogW);

            ComplexFloat[,] array = new ComplexFloat[h, w];
            int threadCount = Environment.ProcessorCount;
            int portion = h / threadCount;
            Thread[] threads = new Thread[threadCount];
            for (int k = 0; k < threadCount; k++)
            {
                int tmp = k;
                threads[k] = new Thread(() =>
                {
                    for (int i = portion * tmp; i < portion * (tmp + 1); i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            array[i, j] = (data[i, j, 0] + data[i, j, 1] + data[i, j, 2]) / 3;
                        }
                    }
                });
            }
            foreach (Thread t in threads)
            {
                t.Start();
                t.Join();
            }
            return array;
        }

        public static float[,,] ExportToRGB(ComplexFloat[,] data, int heigth = 0, int width = 0)
        {
            int h, w;
            if (width == 0 || heigth == 0)
            {
                h = data.GetLength(0);
                w = data.GetLength(1);
            }
            else
            {
                h = heigth;
                w = width;
            }

            float[,,] result = new float[h, w, 3];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    result[i, j, 0] = result[i, j, 1] = result[i, j, 2] = data[i, j].Abs();
                }
            }
            return result;
        }
    }
}
