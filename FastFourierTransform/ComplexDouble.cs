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

        public ComplexDouble Reciprocal() => new ComplexDouble(real / (real * real + imaginary * imaginary), -imaginary / (real * real + imaginary * imaginary));
        public ComplexDouble Conjugate() => new ComplexDouble(real, -imaginary);

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
