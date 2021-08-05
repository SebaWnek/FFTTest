using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class Extensions
    {
        

        public static T[] GetRow<T>(this T[,] input, int row)
        {
            int cols = input.GetLength(1);
            return Enumerable.Range(0, cols).Select(x => input[row, x]).ToArray();
        }

        public static T[] GetCol<T>(this T[,] input, int col)
        {
            int rows = input.GetLength(0);
            return Enumerable.Range(0, rows).Select(x => input[x, col]).ToArray();

        }

        public static ComplexFloat[] Convert(this float[] x)
        {
            ComplexFloat[] result = new ComplexFloat[x.Length];
            for (int i = 0; i < x.Length; i++) result[i] = x[i];
            return result;
        }
        public static float[] Convert(this ComplexFloat[] x)
        {
            float[] result = new float[x.Length];
            for (int i = 0; i < x.Length; i++) result[i] = x[i].Re;
            return result;
        }

        public static float[,] Convert(this ComplexFloat[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            float[,] result = new float[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = (float)x[i, j];
                }
            });
            return result;
        }

        public static ComplexFloat[,] Convert(this float[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            ComplexFloat[,] result = new ComplexFloat[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = x[i, j];
                }
            });
            return result;
        }

        public static ComplexDouble[] Convert(this double[] x)
        {
            ComplexDouble[] result = new ComplexDouble[x.Length];
            for (int i = 0; i < x.Length; i++) result[i] = x[i];
            return result;
        }
        public static double[] Convert(this ComplexDouble[] x)
        {
            double[] result = new double[x.Length];
            for (int i = 0; i < x.Length; i++) result[i] = x[i].Re;
            return result;
        }

        public static double[,] Convert(this ComplexDouble[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] result = new double[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = (double)x[i, j];
                }
            });
            return result;
        }

        public static ComplexDouble[,] Convert(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            ComplexDouble[,] result = new ComplexDouble[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = x[i, j];
                }
            });
            return result;
        }

        public static double[,] Abs(this ComplexDouble[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] result = new double[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = x[i, j].Abs();
                }
            });
            return result;
        }

        public static float[,] Abs(this ComplexFloat[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            float[,] result = new float[rows, cols];
            Parallel.For(0, rows, (i) =>
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = x[i, j].Abs();
                }
            });
            return result;
        }
        public static double[,] Log(this ComplexDouble[,] x, double b = 2)
        {
            double tmpMax = 0;

            double max = 255;
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] result = new double[rows, cols];
            for(int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = Math.Log(x[i, j].Abs(), b);
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

        public static float[,] Log(this ComplexFloat[,] x, double b = 2)
        {
            float tmpMax = 0;
            float max = 255;
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            float[,] result = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = (float)Math.Log(x[i, j].Abs(), b);
                    if (result[i, j] > tmpMax) tmpMax = result[i, j];
                }
            };

            float divider = tmpMax / max;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] /= divider;
                }
            };

            return result;
        }

        public static ComplexFloat TimesMinusI(this float x)
        {
            return new ComplexFloat(0, -x);
        }

        public static ComplexDouble TimesMinusI(this double x)
        {
            return new ComplexDouble(0, -x);
        }
    }
}
