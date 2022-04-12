using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFT
{
    public struct Complex
    {
        private double real;
        private double imaginary;
        public double Re { get => real; set => real = value; }
        public double Im { get => imaginary; set => imaginary = value; }

        public Complex(double re, double im)
        {
            real = re;
            imaginary = im;
        }

        public Complex(double re)
        {
            real = re;
            imaginary = 0;
        }


        public static implicit operator Complex(double x) => new Complex(x);
        //public static implicit operator Complex(ComplexFloat x) => new ComplexDouble(x.Re, x.Im);
        public static explicit operator double(Complex x) => x.real;

        public static bool operator ==(Complex x, Complex y) => x.real == y.real && x.imaginary == y.imaginary;
        public static bool operator !=(Complex x, Complex y) => x.real != y.real || x.imaginary != y.imaginary;
        public static bool operator >(Complex x, Complex y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator <(Complex x, Complex y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator >=(Complex x, Complex y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator <=(Complex x, Complex y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static Complex operator +(Complex x) => x;
        public static Complex operator +(Complex x, Complex y) => new Complex(x.real + y.real, x.imaginary + y.imaginary);
        public static Complex operator +(Complex x, double y) => new Complex(x.real + y, x.imaginary);
        public static Complex operator -(Complex x) => new Complex(-x.real, -x.imaginary);
        public static Complex operator -(Complex x, Complex y) => new Complex(x.real - y.real, x.imaginary - y.imaginary);
        public static Complex operator -(Complex x, double y) => new Complex(x.real - y, x.imaginary);
        public static Complex operator *(Complex x, Complex y) => new Complex(x.real * y.real - x.imaginary * y.imaginary, x.real * y.imaginary + x.imaginary * y.real);
        public static Complex operator *(Complex x, double y) => new Complex(x.real * y, x.imaginary * y);
        public static Complex operator /(Complex x, Complex y) => x * y.Reciprocal();
        public static Complex operator /(Complex x, double y) => new Complex(x.real / y, x.imaginary / y);
        public static Complex operator ^(Complex x, int y) => x.Pow(y);

        public Complex Reciprocal() => new Complex(real / (real * real + imaginary * imaginary), -imaginary / (real * real + imaginary * imaginary));
        public Complex Conjugate() => new Complex(real, -imaginary);


        public Complex TimesMinusI() => new Complex(imaginary, -real);

        public Complex TimesI() => new Complex(-imaginary, real);
        

        public Complex Pow(int exponent)
        {
            if (exponent == 0) return new Complex(1, 0);
            if (exponent == 1) return this;
            Complex result = new Complex(real, imaginary);
            for (int i = 1; i < exponent; i++)
            {
                result *= this;
            }
            return result;
        }
        public float Abs() => (float)Math.Sqrt(real * real + imaginary * imaginary);
        public float Phase() => (float)Math.Atan2(real, imaginary);

        public override bool Equals(object? obj)
        {
            if (obj is Complex) return this == (Complex)obj;
            else return false;

        }

        public override string ToString()
        {
            return $"{real} + {imaginary}i";
        }

        public override int GetHashCode()
        {
            return (real + imaginary).GetHashCode();
        }

        //public static Complex[,] FromSingle(ComplexFloat[,] single)
        //{
        //    int rows = single.GetLength(0);
        //    int cols = single.GetLength(1);

        //    Complex[,] result = new Complex[rows, cols];

        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < cols; j++)
        //        {
        //            result[i, j] = (Complex)single[i, j];
        //        }
        //    }

        //    return result;
        //}
    }
}
