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
    }
}
