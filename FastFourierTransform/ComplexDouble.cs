using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public struct ComplexDouble
    {
        private double real;
        private double imaginary;
        public double Re { get => real; set => real = value; }
        public double Im { get => imaginary; set => imaginary = value; }

        public ComplexDouble(double re, double im)
        {
            real = re;
            imaginary = im;
        }
        public ComplexDouble(double re)
        {
            real = re;
            imaginary = 0;
        }


        public static implicit operator ComplexDouble(double x) => new ComplexDouble(x);
        public static explicit operator double(ComplexDouble x) => x.real;
        public static bool operator ==(ComplexDouble x, ComplexDouble y) => x.real == y.real && x.imaginary == y.imaginary;
        public static bool operator !=(ComplexDouble x, ComplexDouble y) => x.real != y.real || x.imaginary != y.imaginary;
        public static bool operator >(ComplexDouble x, ComplexDouble y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator <(ComplexDouble x, ComplexDouble y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator >=(ComplexDouble x, ComplexDouble y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator <=(ComplexDouble x, ComplexDouble y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static ComplexDouble operator +(ComplexDouble x) => x;
        public static ComplexDouble operator -(ComplexDouble x) => new ComplexDouble(-x.real, -x.imaginary);
        public static ComplexDouble operator +(ComplexDouble x, ComplexDouble y) => new ComplexDouble(x.real + y.real, x.imaginary + y.imaginary);
        public static ComplexDouble operator -(ComplexDouble x, ComplexDouble y) => new ComplexDouble(x.real - y.real, x.imaginary - y.imaginary);
        public static ComplexDouble operator *(ComplexDouble x, ComplexDouble y) => new ComplexDouble(x.real * y.real - x.imaginary * y.imaginary, x.real * y.imaginary - x.imaginary * y.real);
        public static ComplexDouble operator /(ComplexDouble x, ComplexDouble y) => x * y.Reciprocal();
        public static ComplexDouble operator +(ComplexDouble x, double y) => new ComplexDouble(x.real + y, x.imaginary);
        public static ComplexDouble operator -(ComplexDouble x, double y) => new ComplexDouble(x.real - y, x.imaginary);
        public static ComplexDouble operator *(ComplexDouble x, double y) => new ComplexDouble(x.real * y, x.imaginary * y);
        public static ComplexDouble operator /(ComplexDouble x, double y) => new ComplexDouble(x.real / y, x.imaginary / y);
        public static ComplexDouble operator ^(ComplexDouble x, int y) => x.Pow(y);

        public ComplexDouble Reciprocal() => new ComplexDouble(real / (real * real + imaginary * imaginary), -imaginary / (real * real + imaginary * imaginary));
        public ComplexDouble Conjugate() => new ComplexDouble(real, -imaginary);

        public override string ToString()
        {
            return $"{real} + {imaginary}i";
        }

        public ComplexDouble TimesMinusI() => new ComplexDouble(imaginary, -real);
        public ComplexDouble TimesI() => new ComplexDouble(-imaginary, real);

        public ComplexDouble Pow(int exponent)
        {
            if (exponent == 0) return new ComplexDouble(1, 0);
            if (exponent == 1) return this;
            ComplexDouble result = new ComplexDouble(real, imaginary);
            for (int i = 1; i < exponent; i++)
            {
                result *= this;
            }
            return result;
        }
        public float Abs() => (float)Math.Sqrt(real * real + imaginary * imaginary);
        public float Phase() => (float)Math.Atan2(real, imaginary);

        public override bool Equals(object obj)
        {
            if (obj is ComplexDouble) return this == (ComplexDouble)obj;
            else return false;

        }

        public override int GetHashCode()
        {
            return (real + imaginary).GetHashCode();
        }
    }
}
