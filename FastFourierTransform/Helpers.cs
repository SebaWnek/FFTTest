using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class Helpers
    {
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
                    array[i, j] = (data[i, j, 0] + data[i, j, 1] + data[i, j, 2])/3;
                }
            }

            return array;
        }

        public static float[,,] ExportToRGB(ComplexFloat[,] data, int width = 0, int heigth = 0)
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
        }
    }
}
