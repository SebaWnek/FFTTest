using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class FFTOptimisedKernelsDouble
    {

        public static ComplexDouble[] Kernel1(ComplexDouble[] input, ref ComplexDouble[][] omegas)
        {
            return input;
        }
        public static unsafe ComplexDouble[] Kernel8(ComplexDouble[] i, ref ComplexDouble[][] omegas)
        {
            ComplexDouble[] tmp = new ComplexDouble[4];
            ComplexDouble[] result = new ComplexDouble[8];

            result[0] = result[4] = i[0] + i[2] + i[4] + i[6];
            result[1] = result[5] = i[0] - i[4] + (i[2] - i[6]).TimesMinusI();
            result[2] = result[6] = i[0] + i[4] - i[2] - i[6];
            result[3] = result[7] = i[0] - i[4] - (i[2] - i[6]).TimesMinusI();

            tmp[0] = i[1] + i[3] + i[5] + i[7];
            tmp[1] = (i[1] - i[5] + (i[3] - i[7]).TimesMinusI()) * omegas[3][1];
            tmp[2] = (i[1] - i[3] + i[5] - i[7]).TimesMinusI();
            tmp[3] = (i[1] - i[5] - (i[3] - i[7]).TimesMinusI()) * omegas[3][3];

            if (Avx2.IsSupported)
            {
                fixed (ComplexDouble* pI = i, pRes = result, pTmp = tmp)
                {
                    double* pIfl = (double*)pI;
                    double* pResfl = (double*)pRes;
                    double* pTmpfl = (double*)pTmp;

                    Avx2.Store(pResfl + 0, Avx2.Add(Avx2.LoadVector256(pResfl + 0), Avx2.LoadVector256(pTmpfl)));
                    Avx2.Store(pResfl + 4, Avx2.Add(Avx2.LoadVector256(pResfl + 4), Avx2.LoadVector256(pTmpfl + 4)));
                    Avx2.Store(pResfl + 8, Avx2.Subtract(Avx2.LoadVector256(pResfl + 8), Avx2.LoadVector256(pTmpfl)));
                    Avx2.Store(pResfl + 12, Avx2.Subtract(Avx2.LoadVector256(pResfl + 12), Avx2.LoadVector256(pTmpfl + 4)));
                }
            }

            else
            {
                result[0] += tmp[0];
                result[1] += tmp[1];
                result[2] += tmp[2];
                result[3] += tmp[3];
                result[4] -= tmp[0];
                result[5] -= tmp[1];
                result[6] -= tmp[2];
                result[7] -= tmp[3]; 
            }

            return result;
        }


        public static ComplexDouble[] Kernel16(double[] i, ref ComplexDouble[][] omegas)
        {
            ComplexDouble[] result = new ComplexDouble[16];
            ComplexDouble[] tmp3 = new ComplexDouble[8];

            ComplexDouble ami = i[0] - i[8];
            ComplexDouble api = i[0] + i[8];
            ComplexDouble fmn = i[5] - i[13];
            ComplexDouble fpn = i[5] + i[13];

            double tmp = i[1] + i[3] + i[5] + i[7] + i[9] + i[11] + i[13] + i[15];
            tmp3[1] = omegas[4][1] * (i[1] - i[9] + (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI());
            tmp3[2] = omegas[4][2] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() + i[1] - fpn + i[9]);
            tmp3[3] = omegas[4][3] * (omegas[3][3] * (i[11] - i[3] + (i[7] - i[15]).TimesMinusI()) - i[1] + i[9] + (fmn).TimesMinusI());
            tmp3[4] = (i[1] - i[3] + fpn - i[7] + i[9] - i[11] - i[15]).TimesMinusI();
            tmp3[5] = (i[1] - i[9] - (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI()) * omegas[4][5];
            tmp3[6] = omegas[4][6] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() - i[1] + fpn - i[9]);
            tmp3[7] = omegas[4][7] * ((i[11] - i[3] + (i[7] - i[15]).TimesMinusI()).TimesMinusI() + i[1] - i[9] - (fmn).TimesMinusI());

            result[0] = result[8] = api + i[2] + i[4] + i[6] + i[10] + i[12] + i[14];
            result[1] = result[9] = ami + (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            result[2] = result[10] = api - i[4] - i[12] + (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            result[3] = result[11] = ami - (i[4] - i[12]).TimesMinusI() - (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()) * omegas[3][3];
            result[4] = result[12] = api - i[2] + i[4] - i[6] - i[10] + i[12] - i[14];
            result[5] = result[13] = ami - (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            result[6] = result[14] = api - i[4] - i[12] - (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            result[7] = result[15] = ami - (i[4] - i[12]).TimesMinusI() + (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()).TimesMinusI();

            result[0] += tmp;
            result[8] -= tmp;

            result[1] += tmp3[1];
            result[9] -= tmp3[1];

            result[2] += tmp3[2];
            result[10] -= tmp3[2];

            result[3] -= tmp3[3];
            result[11] += tmp3[3];

            result[4] += tmp3[4];
            result[12] -= tmp3[4];

            result[5] += tmp3[5];
            result[13] -= tmp3[5];

            result[6] -= tmp3[6];
            result[14] += tmp3[6];

            result[7] += tmp3[7];
            result[15] -= tmp3[7];

            return result;
        }


        public static ComplexDouble[] Kernel16(ComplexDouble[] i, ref ComplexDouble[][] omegas)
        {
            ComplexDouble[] result = new ComplexDouble[16];
            ComplexDouble[] tmp3 = new ComplexDouble[8];

            ComplexDouble ami = i[0] - i[8];
            ComplexDouble api = i[0] + i[8];
            ComplexDouble fmn = i[5] - i[13];
            ComplexDouble fpn = i[5] + i[13];

            tmp3[0] = i[1] + i[3] + i[5] + i[7] + i[9] + i[11] + i[13] + i[15];
            tmp3[1] = omegas[4][1] * (i[1] - i[9] + (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI());
            tmp3[2] = omegas[4][2] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() + i[1] - fpn + i[9]);
            tmp3[3] = omegas[4][3] * (omegas[3][3] * (i[11] - i[3] + (i[7] - i[15]).TimesMinusI()) - i[1] + i[9] + (fmn).TimesMinusI());
            tmp3[4] = (i[1] - i[3] + fpn - i[7] + i[9] - i[11] - i[15]).TimesMinusI();
            tmp3[5] = (i[1] - i[9] - (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI()) * omegas[4][5];
            tmp3[6] = omegas[4][6] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() - i[1] + fpn - i[9]);
            tmp3[7] = omegas[4][7] * ((i[11] - i[3] + (i[7] - i[15]).TimesMinusI()).TimesMinusI() + i[1] - i[9] - (fmn).TimesMinusI());

            result[0] = result[8] = api + i[2] + i[4] + i[6] + i[10] + i[12] + i[14];
            result[1] = result[9] = ami + (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            result[2] = result[10] = api - i[4] - i[12] + (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            result[3] = result[11] = ami - (i[4] - i[12]).TimesMinusI() - (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()) * omegas[3][3];
            result[4] = result[12] = api - i[2] + i[4] - i[6] - i[10] + i[12] - i[14];
            result[5] = result[13] = ami - (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            result[6] = result[14] = api - i[4] - i[12] - (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            result[7] = result[15] = ami - (i[4] - i[12]).TimesMinusI() + (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()).TimesMinusI();

            result[0] += tmp3[0];
            result[8] -= tmp3[0];

            result[1] += tmp3[1];
            result[9] -= tmp3[1];

            result[2] += tmp3[2];
            result[10] -= tmp3[2];

            result[3] -= tmp3[3];
            result[11] += tmp3[3];

            result[4] += tmp3[4];
            result[12] -= tmp3[4];

            result[5] += tmp3[5];
            result[13] -= tmp3[5];

            result[6] -= tmp3[6];
            result[14] += tmp3[6];

            result[7] += tmp3[7];
            result[15] -= tmp3[7];

            return result;
        }
    }
}
