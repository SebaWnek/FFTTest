using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public struct ComplexFloat
    {
        private float real;
        private float imaginary;
        public float Re { get => real; set => real = value; }
        public float Im { get => imaginary; set => imaginary = value; }

        public ComplexFloat(float re, float im)
        {
            real = re;
            imaginary = im;
        }

        public ComplexFloat(float re)
        {
            real = re;
            imaginary = 0;
        }


        public static implicit operator ComplexFloat(float x) => new ComplexFloat(x);
        public static explicit operator float(ComplexFloat x) => x.real;
        public static explicit operator ComplexFloat(ComplexDouble x) => new ComplexFloat((float)x.Re, (float)x.Im);

        public static bool operator ==(ComplexFloat x, ComplexFloat y) => x.real == y.real && x.imaginary == y.imaginary;
        public static bool operator !=(ComplexFloat x, ComplexFloat y) => x.real != y.real || x.imaginary != y.imaginary;
        public static bool operator >(ComplexFloat x, ComplexFloat y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator <(ComplexFloat x, ComplexFloat y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator >=(ComplexFloat x, ComplexFloat y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static bool operator <=(ComplexFloat x, ComplexFloat y) => throw new InvalidOperationException("Operation not vlid for complex numbers");
        public static ComplexFloat operator +(ComplexFloat x) => x;
        public static ComplexFloat operator -(ComplexFloat x) => new ComplexFloat(-x.real, -x.imaginary);
        public static ComplexFloat operator +(ComplexFloat x, ComplexFloat y) => new ComplexFloat(x.real + y.real, x.imaginary + y.imaginary);
        public static ComplexFloat operator -(ComplexFloat x, ComplexFloat y) => new ComplexFloat(x.real - y.real, x.imaginary - y.imaginary);
        public static ComplexFloat operator +(ComplexFloat x, float y) => new ComplexFloat(x.real + y, x.imaginary);
        public static ComplexFloat operator -(ComplexFloat x, float y) => new ComplexFloat(x.real - y, x.imaginary);
        public static ComplexFloat operator *(ComplexFloat x, ComplexFloat y) => new ComplexFloat(x.real * y.real - x.imaginary * y.imaginary, x.real * y.imaginary + x.imaginary * y.real);
        public static ComplexFloat operator *(ComplexFloat x, float y) => new ComplexFloat(x.real * y, x.imaginary * y);
        public static ComplexFloat operator /(ComplexFloat x, ComplexFloat y) => x * y.Reciprocal();
        public static ComplexFloat operator /(ComplexFloat x, float y) => new ComplexFloat(x.real / y, x.imaginary / y);
        public static ComplexFloat operator ^(ComplexFloat x, int y) => x.Pow(y);

        public ComplexFloat Reciprocal() => new ComplexFloat(real / (real * real + imaginary * imaginary), -imaginary / (real * real + imaginary * imaginary));
        public ComplexFloat Conjugate() => new ComplexFloat(real, -imaginary);

        public ComplexFloat TimesMinusI() => new ComplexFloat(imaginary, -real);
        public ComplexFloat TimesI() => new ComplexFloat(-imaginary, real);

        public ComplexFloat Pow(int exponent)
        {
            if (exponent == 0) return new ComplexFloat(1, 0);
            if (exponent == 1) return this;
            ComplexFloat result = new ComplexFloat(real, imaginary);
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
            if (obj is ComplexFloat) return this == (ComplexFloat)obj;
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
    }
}
