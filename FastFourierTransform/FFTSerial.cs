using System;

namespace FastFourierTransform
{
    public static class FFTSerial
    {

        public static ComplexFloat CalculateOmegaS(int length, bool inverse)
        {
            float l = (float)length;
            //e^(i*x) = cos x + i sin x
            float exponent = -2 * (float)Math.PI / l;
            if (inverse) exponent *= -1;
            return new ComplexFloat((float)Math.Cos(exponent), (float)Math.Sin(exponent));
        }

        public static ComplexDouble CalculateOmegaD(int length, bool inverse)
        {
            //e^(i*x) = cos x + i sin x
            double exponent = 2 * Math.PI / length; 
            
            if (!inverse)
            {
                return new ComplexDouble(Math.Cos(exponent), Math.Sin(exponent));
            }
            else
            {
                //(1/n) e^ ((-2 pi i)/n)
                exponent *= -1;

                return new ComplexDouble(Math.Cos(exponent) * (1 / length), Math.Sin(exponent) * (1 / length));
            }
        }
        public static ComplexFloat[] FFTBase(ComplexFloat[] input, bool inverse)
        {
            int n = input.Length;
            //Check if array length is power of 2
            if (!Helpers.CheckIfPowerOfTwo(n))
            {
                throw new ArgumentException("Array length must e a power of 2!");
            }
            //If only one element - returns
            if (n == 1) return input;

            ComplexFloat omega = CalculateOmegaS(n, inverse);   
            //Dividing into two arrays
            (ComplexFloat[] pe, ComplexFloat[] po) = DivideArrayS(input);

            ComplexFloat[] ye = FFTBase(pe, inverse);
            ComplexFloat[] yo = FFTBase(po, inverse);

            ComplexFloat[] y = new ComplexFloat[n];
            int halfn = n / 2;
            for (int i = 0; i < halfn; i++)
            {
                y[i] = ye[i] + omega.Pow(i) * yo[i];
                y[i + n / 2] = ye[i] - omega.Pow(i) * yo[i];
            }
            return y;


        }

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

        public static ComplexFloat[] FFT(float[] input)
        {
            return FFTBase(input.Convert(), false);
        }
        public static ComplexFloat[] FFT(ComplexFloat[] input)
        {
            return FFTBase(input, false);
        }

        public static ComplexFloat[] IFFT(ComplexFloat[] input)
        {
            int length = input.Length;
            ComplexFloat[] tmp = FFTBase(input, true);
            for(int i = 0; i < length; i++)
            {
                tmp[i] /= length;
            }
            return tmp;
        }

        public static ComplexDouble[] FFT(double[] input)
        {
            throw new NotImplementedException();
        }

        public static double[] IFFT(ComplexDouble[] input)
        {
            throw new NotImplementedException();
        }

        public static ComplexFloat[,] FFT(float[,] input)
        {
            int rows = input.GetLength(0);
            int columns = input.GetLength(1);
            ComplexFloat[,] result = new ComplexFloat[rows, columns];
            ComplexFloat[] tmpRow = new ComplexFloat[columns];
            ComplexFloat[] tmpCol = new ComplexFloat[rows];
            for (int i = 0; i < rows; i++)
            {
                tmpRow = FFT(input.GetRow(i));
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = tmpRow[j];
                }
            }
            for (int i = 0; i < columns; i++)
            {
                tmpCol = FFT(result.GetCol(i));
                for (int j = 0; j < rows; j++)
                {
                    result[j, i] = tmpCol[j];
                }
            }
            return result;
        }

        public static float[,] IFFT(ComplexFloat[,] input)
        {
            int rows = input.GetLength(0);
            int columns = input.GetLength(1);
            ComplexFloat[,] result = new ComplexFloat[rows, columns];
            ComplexFloat[] tmpRow = new ComplexFloat[columns];
            ComplexFloat[] tmpCol = new ComplexFloat[rows];
            for (int i = 0; i < rows; i++)
            {
                tmpRow = IFFT(input.GetRow(i));
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = tmpRow[j];
                }
            }
            for (int i = 0; i < columns; i++)
            {
                tmpCol = IFFT(result.GetCol(i));
                for (int j = 0; j < rows; j++)
                {
                    result[j, i] = tmpCol[j];
                }
            }
            return result.Convert();
        }

        public static ComplexDouble[,] FFT(double[,] input)
        {
            throw new NotImplementedException();
        }

        public static double[,] IFFT(ComplexDouble[,] input)
        {
            throw new NotImplementedException();
        }

        public static float[,] Shift(float[,] input)
        {
            throw new NotImplementedException();
        }

        public static double[,] Shift(double[,] input)
        {
            throw new NotImplementedException();
        }

        public static ComplexFloat[,] Shift(ComplexFloat[,] input)
        {
            throw new NotImplementedException();
        }

        public static ComplexDouble[,] Shift(ComplexDouble[,] input)
        {
            throw new NotImplementedException();
        }
    }
}
