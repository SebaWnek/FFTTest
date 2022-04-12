using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFT
{
    /// <summary>
    /// Helper methods needed for FFT calculations and allowing some additional functionality on complex number arrays
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Shifts array by half, used to move brightest part of FFT'd image from cornes to middle
        /// </summary>
        /// <typeparam name="T">Any type containing data</typeparam>
        /// <param name="input">Array to be shifted</param>
        /// <returns>Shifted 2d array</returns>
        public static T[,] ShiftToMiddle<T>(T[,] input)
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

        /// <summary>
        /// Checks if array sizes are powers of 2, as used FFT algorythm requires that
        /// </summary>
        /// <param name="data">2d array to be checked</param>
        /// <returns>Bool with confirmation or denial that tested array's size is power of 2 in both directions</returns>
        internal static bool CheckIfPowerOfTwo(int data)
        {
            if (data == 1) return true;
            double power = 2;
            while (power <= data)
            {
                if (power == data) return true;
                power *= 2;
            }
            return false;
        }

        /// <summary>
        /// Converts byte data containing ARGB bitmap image into 2d array of complex numbers. 
        /// Averages all 3 color channels into grayscale, discards alpha channel. 
        /// </summary>
        /// <param name="data">Image data, array containing all the pixels in 8 bits per channel, 4 channels</param>
        /// <param name="width">Image width</param>
        /// <returns>2d array of complex numbers containing averaged RGB data</returns>
        public static Complex[,] ImportFromArgbBitmap(byte[] data, int width)
        {
            int heigth = data.Length / (4 * width);
            Complex[,] result = new Complex[heigth, width];
            int position;
            for (int i = 0; i < heigth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    position = i * width * 4 + 4 * j;
                    result[i, j] = (data[position] + data[position + 1] + data[position + 2]) / 3;
                }
            }

            return result;
        }

        /// <summary>
        /// Extension method for multiplying floating point value by imaginary -i
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>Output complex value</returns>
        public static Complex TimesMinusI(this double x) => new Complex(0, -x);
    }
}
