using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class FFTOptimisedKernels
    {

        public static ComplexFloat[] Kernel1(ComplexFloat[] input, ref ComplexFloat[][] omegas)
        {
            return input;
        }

        public static ComplexDouble[] Kernel1(ComplexDouble[] input, ref ComplexDouble[][] omegas)
        {
            return input;
        }

        //public static ComplexFloat[] Kernel2(ComplexFloat[] input)
        //{

        //}
        //public static ComplexFloat[] Kernel4(ComplexFloat[] input)
        //{

        //}

        public static unsafe ComplexFloat[] Kernel8(ComplexFloat[] i, ref ComplexFloat[][] omegas)
        {
            ComplexFloat[] tmp = new ComplexFloat[4];
            ComplexFloat[] result = new ComplexFloat[8];

            result[0] = result[4] = i[0] + i[2] + i[4] + i[6];
            result[1] = result[5] = i[0] - i[4] + (i[2] - i[6]).TimesMinusI();
            result[2] = result[6] = i[0] + i[4] - i[2] - i[6];
            result[3] = result[7] = i[0] - i[4] - (i[2] - i[6]).TimesMinusI();

            tmp[0] = i[1] + i[3] + i[5] + i[7];
            tmp[1] = (i[1] - i[5] + (i[3] - i[7]).TimesMinusI()) * omegas[3][1];
            tmp[2] = (i[1] - i[3] + i[5] - i[7]).TimesMinusI();
            tmp[3] = (i[1] - i[5] - (i[3] - i[7]).TimesMinusI()) * omegas[3][3];

            fixed (ComplexFloat* pI = i, pRes = result, pTmp = tmp)
            {
                float* pIfl = (float*)pI;
                float* pResfl = (float*)pRes;
                float* pTmpfl = (float*)pTmp;

                Avx2.Store(pResfl + 0, Avx2.Add(Avx2.LoadVector256(pResfl + 0), Avx2.LoadVector256(pTmpfl)));
                Avx2.Store(pResfl + 8, Avx2.Subtract(Avx2.LoadVector256(pResfl + 8), Avx2.LoadVector256(pTmpfl)));
            }

            //result[0] += tmp[0];
            //result[1] += tmp[1];
            //result[2] += tmp[2];
            //result[3] += tmp[3];
            //result[4] -= tmp[0];
            //result[5] -= tmp[1];
            //result[6] -= tmp[2];
            //result[7] -= tmp[3];

            return result;
        }

        public static ComplexFloat[] Kernel16(float[] i, ref ComplexFloat[][] omegas)
        {
            ComplexFloat[] result = new ComplexFloat[16];
            ComplexFloat[] tmp3 = new ComplexFloat[8];

            ComplexFloat ami = i[0] - i[8];
            ComplexFloat api = i[0] + i[8];
            ComplexFloat fmn = i[5] - i[13];
            ComplexFloat fpn = i[5] + i[13];

            float tmp = i[1] + i[3] + i[5] + i[7] + i[9] + i[11] + i[13] + i[15];
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

        public static ComplexFloat[] Kernel16(ComplexFloat[] i, ref ComplexFloat[][] omegas)
        {
            ComplexFloat[] result = new ComplexFloat[16];
            ComplexFloat[] tmp3 = new ComplexFloat[8];

            ComplexFloat ami = i[0] - i[8];
            ComplexFloat api = i[0] + i[8];
            ComplexFloat fmn = i[5] - i[13];
            ComplexFloat fpn = i[5] + i[13];

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

        public static unsafe ComplexFloat[] Kernel32(ComplexFloat[] i, ref ComplexFloat[][] omegas)
        {
            ComplexFloat[] result = new ComplexFloat[32];
            ComplexFloat[] tmp = new ComplexFloat[48];

            ComplexFloat ami = i[0] - i[8];
            ComplexFloat api = i[0] + i[8];
            ComplexFloat fmn = i[5] - i[13];
            ComplexFloat fpn = i[5] + i[13];

            ComplexFloat xami = i[16] - i[24];
            ComplexFloat xapi = i[16] + i[24];
            ComplexFloat xfmn = i[21] - i[29];
            ComplexFloat xfpn = i[21] + i[29];

            tmp[0] = api + i[2] + i[4] + i[6] + i[10] + i[12] + i[14];
            tmp[1] = ami + (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            tmp[2] = api - i[4] - i[12] + (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            tmp[3] = ami - (i[4] - i[12]).TimesMinusI() - (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()) * omegas[3][3];
            tmp[4] = api - i[2] + i[4] - i[6] - i[10] + i[12] - i[14];
            tmp[5] = ami - (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            tmp[6] = api - i[4] - i[12] - (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            tmp[7] = ami - (i[4] - i[12]).TimesMinusI() + (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()).TimesMinusI();

            tmp[8] = i[1] + i[3] + fpn + i[7] + i[9] + i[11] + i[15];
            tmp[9] = omegas[4][1] * (i[1] - i[9] + (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI());
            tmp[10] = omegas[4][2] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() + i[1] - fpn + i[9]);
            tmp[11] = omegas[4][3] * (omegas[3][3] * (i[11] - i[3] + (i[7] - i[15]).TimesMinusI()) - i[1] + i[9] + (fmn).TimesMinusI());
            tmp[12] = (i[1] - i[3] + fpn - i[7] + i[9] - i[11] - i[15]).TimesMinusI();
            tmp[13] = (i[1] - i[9] - (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI()) * omegas[4][5];
            tmp[14] = omegas[4][6] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() - i[1] + fpn - i[9]);
            tmp[15] = omegas[4][7] * ((i[11] - i[3] + (i[7] - i[15]).TimesMinusI()).TimesMinusI() + i[1] - i[9] - (fmn).TimesMinusI());

            tmp[16] = xapi + i[18] + i[20] + i[22] + i[28] + i[26] + i[30];
            tmp[17] = xami + (i[18] - i[26] + (i[22] - i[30]).TimesMinusI()) * omegas[3][1] + (i[20] - i[28]).TimesMinusI();
            tmp[18] = xapi - i[20] - i[28] + (i[18] - i[22] + i[26] - i[30]).TimesMinusI();
            tmp[19] = xami - (i[20] - i[28]).TimesMinusI() - (i[26] - i[18] + (i[22] - i[30]).TimesMinusI()) * omegas[3][3];
            tmp[20] = xapi - i[28] + i[20] - i[22] - i[26] + i[28] - i[30];
            tmp[21] = xami - (i[28] - i[26] + (i[22] - i[30]).TimesMinusI()) * omegas[3][1] + (i[20] - i[22]).TimesMinusI();
            tmp[22] = xapi - i[20] - i[28] - (i[18] - i[22] + i[26] - i[30]).TimesMinusI();
            tmp[23] = xami - (i[20] - i[28]).TimesMinusI() + (i[26] - i[18] + (i[22] - i[30]).TimesMinusI()).TimesMinusI();

            tmp[24] = i[17] + i[19] + xfpn + i[23] + i[25] + i[27] + i[31];
            tmp[25] = omegas[4][1] * (i[17] - i[25] + (i[19] - i[27] + (i[23] - i[31]).TimesMinusI()) * omegas[3][1] + (xfmn).TimesMinusI());
            tmp[26] = omegas[4][2] * ((i[19] - i[23] + i[27] - i[31]).TimesMinusI() + i[17] - xfpn + i[25]);
            tmp[27] = omegas[4][3] * (omegas[3][3] * (i[27] - i[19] + (i[23] - i[31]).TimesMinusI()) - i[17] + i[25] + (xfmn).TimesMinusI());
            tmp[28] = (i[17] - i[19] + xfpn - i[23] + i[25] - i[27] - i[31]).TimesMinusI();
            tmp[29] = (i[17] - i[25] - (i[19] - i[27] + (i[23] - i[31]).TimesMinusI()) * omegas[3][1] + (xfmn).TimesMinusI()) * omegas[4][5];
            tmp[30] = omegas[4][6] * ((i[19] - i[23] + i[27] - i[31]).TimesMinusI() - i[17] + xfpn - i[25]);
            tmp[31] = omegas[4][7] * ((i[27] - i[19] + (i[23] - i[31]).TimesMinusI()).TimesMinusI() + i[17] - i[25] - (xfmn).TimesMinusI());


            result[0] = tmp[0] + tmp[8];
            result[1] = tmp[1] + tmp[9];
            result[2] = tmp[2] + tmp[10];
            result[3] = tmp[3] + tmp[11];
            result[4] = tmp[4] + tmp[12];
            result[5] = tmp[5] + tmp[13];
            result[6] = tmp[6] + tmp[14];
            result[7] = tmp[7] + tmp[15];

            result[8] = tmp[0] - tmp[8];
            result[9] = tmp[1] - tmp[9];
            result[10] = tmp[2] - tmp[10];
            result[11] = tmp[3] - tmp[11];
            result[12] = tmp[4] - tmp[12];
            result[13] = tmp[5] - tmp[13];
            result[14] = tmp[6] - tmp[14];
            result[15] = tmp[7] - tmp[15];

            result[16] = tmp[16] + tmp[24];
            result[17] = tmp[17] + tmp[25];
            result[18] = tmp[18] + tmp[26];
            result[19] = tmp[19] + tmp[27];
            result[20] = tmp[20] + tmp[28];
            result[21] = tmp[21] + tmp[29];
            result[22] = tmp[22] + tmp[30];
            result[23] = tmp[23] + tmp[31];

            result[24] = tmp[16] - tmp[24];
            result[25] = tmp[17] - tmp[25];
            result[26] = tmp[18] - tmp[26];
            result[27] = tmp[19] - tmp[27];
            result[28] = tmp[20] - tmp[28];
            result[29] = tmp[21] - tmp[29];
            result[30] = tmp[22] - tmp[30];
            result[31] = tmp[23] - tmp[31];



            tmp[0] = result[0] + result[8];
            tmp[1] = result[1] + result[9];
            tmp[2] = result[2] + result[10];
            tmp[3] = result[3] + result[11];
            tmp[4] = result[4] + result[12];
            tmp[5] = result[5] + result[13];
            tmp[6] = result[6] + result[14];
            tmp[7] = result[7] + result[15];

            tmp[8] = result[0] - result[8];
            tmp[9] = result[1] - result[9];
            tmp[10] = result[2] - result[10];
            tmp[11] = result[3] - result[11];
            tmp[12] = result[4] - result[12];
            tmp[13] = result[5] - result[13];
            tmp[14] = result[6] - result[14];
            tmp[15] = result[7] - result[15];

            tmp[16] = result[16] + result[24];
            tmp[17] = result[17] + result[25];
            tmp[18] = result[18] + result[26];
            tmp[19] = result[19] + result[27];
            tmp[20] = result[20] + result[28];
            tmp[21] = result[21] + result[29];
            tmp[22] = result[22] + result[30];
            tmp[23] = result[23] + result[31];

            tmp[24] = result[16] - result[24];
            tmp[25] = result[17] - result[25];
            tmp[26] = result[18] - result[26];
            tmp[27] = result[19] - result[27];
            tmp[28] = result[20] - result[28];
            tmp[29] = result[21] - result[29];
            tmp[30] = result[22] - result[30];
            tmp[31] = result[23] - result[31];

            tmp[32] = tmp[16];
            tmp[33] = tmp[17] * omegas[5][1];
            tmp[34] = tmp[18] * omegas[5][2];
            tmp[35] = tmp[19] * omegas[5][3];
            tmp[36] = tmp[20] * omegas[5][4];
            tmp[37] = tmp[21] * omegas[5][5];
            tmp[38] = tmp[22] * omegas[5][6];
            tmp[39] = tmp[23] * omegas[5][7];

            tmp[40] = tmp[24].TimesMinusI();
            tmp[41] = tmp[25] * omegas[5][9];
            tmp[42] = tmp[26] * omegas[5][10];
            tmp[43] = tmp[27] * omegas[5][11];
            tmp[44] = tmp[28] * omegas[5][12];
            tmp[45] = tmp[29] * omegas[5][13];
            tmp[46] = tmp[30] * omegas[5][14];
            tmp[47] = tmp[31] * omegas[5][15];




            result[0] = tmp[0] + tmp[16];
            result[1] = tmp[1] + tmp[17];
            result[2] = tmp[2] + tmp[18];
            result[3] = tmp[3] + tmp[19];
            result[4] = tmp[4] + tmp[20];
            result[5] = tmp[5] + tmp[21];
            result[6] = tmp[6] + tmp[22];
            result[7] = tmp[7] + tmp[23];

            result[8] = tmp[8] + tmp[24];
            result[9] = tmp[9] + tmp[25];
            result[10] = tmp[10] + tmp[26];
            result[11] = tmp[11] + tmp[27];
            result[12] = tmp[12] + tmp[28];
            result[13] = tmp[13] + tmp[29];
            result[14] = tmp[14] + tmp[30];
            result[15] = tmp[15] + tmp[31];

            result[16] = tmp[0] + tmp[32];
            result[17] = tmp[1] + tmp[33];
            result[18] = tmp[2] + tmp[34];
            result[19] = tmp[3] + tmp[35];
            result[20] = tmp[4] + tmp[36];
            result[21] = tmp[5] + tmp[37];
            result[22] = tmp[6] + tmp[38];
            result[23] = tmp[7] + tmp[39];

            result[24] = tmp[8] + tmp[40];
            result[25] = tmp[9] + tmp[41];
            result[26] = tmp[10] + tmp[42];
            result[27] = tmp[11] + tmp[43];
            result[28] = tmp[12] + tmp[44];
            result[29] = tmp[13] + tmp[45];
            result[30] = tmp[14] + tmp[46];
            result[31] = tmp[15] + tmp[47];

            return result;
        }

        static byte imm8bShuffle = 0b10110001;
        static byte imm8aImShuffle = 0b10100000;
        static byte imm8aReShuffle = 0b11110101;
        public static unsafe ComplexFloat[] Kernel32avx(ComplexFloat[] i, ref ComplexFloat[][] omegas)
        {
            ComplexFloat[] result = new ComplexFloat[32];
            ComplexFloat[] tmp = new ComplexFloat[48];

            ComplexFloat ami = i[0] - i[8];
            ComplexFloat api = i[0] + i[8];
            ComplexFloat fmn = i[5] - i[13];
            ComplexFloat fpn = i[5] + i[13];

            ComplexFloat xami = i[16] - i[24];
            ComplexFloat xapi = i[16] + i[24];
            ComplexFloat xfmn = i[21] - i[29];
            ComplexFloat xfpn = i[21] + i[29];

            tmp[0] = api + i[2] + i[4] + i[6] + i[10] + i[12] + i[14];
            tmp[1] = ami + (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            tmp[2] = api - i[4] - i[12] + (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            tmp[3] = ami - (i[4] - i[12]).TimesMinusI() - (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()) * omegas[3][3];
            tmp[4] = api - i[2] + i[4] - i[6] - i[10] + i[12] - i[14];
            tmp[5] = ami - (i[2] - i[10] + (i[6] - i[14]).TimesMinusI()) * omegas[3][1] + (i[4] - i[12]).TimesMinusI();
            tmp[6] = api - i[4] - i[12] - (i[2] - i[6] + i[10] - i[14]).TimesMinusI();
            tmp[7] = ami - (i[4] - i[12]).TimesMinusI() + (i[10] - i[2] + (i[6] - i[14]).TimesMinusI()).TimesMinusI();

            tmp[8] = i[1] + i[3] + fpn + i[7] + i[9] + i[11] + i[15];
            tmp[9] = omegas[4][1] * (i[1] - i[9] + (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI());
            tmp[10] = omegas[4][2] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() + i[1] - fpn + i[9]);
            tmp[11] = omegas[4][3] * (omegas[3][3] * (i[11] - i[3] + (i[7] - i[15]).TimesMinusI()) - i[1] + i[9] + (fmn).TimesMinusI());
            tmp[12] = (i[1] - i[3] + fpn - i[7] + i[9] - i[11] - i[15]).TimesMinusI();
            tmp[13] = (i[1] - i[9] - (i[3] - i[11] + (i[7] - i[15]).TimesMinusI()) * omegas[3][1] + (fmn).TimesMinusI()) * omegas[4][5];
            tmp[14] = omegas[4][6] * ((i[3] - i[7] + i[11] - i[15]).TimesMinusI() - i[1] + fpn - i[9]);
            tmp[15] = omegas[4][7] * ((i[11] - i[3] + (i[7] - i[15]).TimesMinusI()).TimesMinusI() + i[1] - i[9] - (fmn).TimesMinusI());

            tmp[16] = xapi + i[18] + i[20] + i[22] + i[28] + i[26] + i[30];
            tmp[17] = xami + (i[18] - i[26] + (i[22] - i[30]).TimesMinusI()) * omegas[3][1] + (i[20] - i[28]).TimesMinusI();
            tmp[18] = xapi - i[20] - i[28] + (i[18] - i[22] + i[26] - i[30]).TimesMinusI();
            tmp[19] = xami - (i[20] - i[28]).TimesMinusI() - (i[26] - i[18] + (i[22] - i[30]).TimesMinusI()) * omegas[3][3];
            tmp[20] = xapi - i[28] + i[20] - i[22] - i[26] + i[28] - i[30];
            tmp[21] = xami - (i[28] - i[26] + (i[22] - i[30]).TimesMinusI()) * omegas[3][1] + (i[20] - i[22]).TimesMinusI();
            tmp[22] = xapi - i[20] - i[28] - (i[18] - i[22] + i[26] - i[30]).TimesMinusI();
            tmp[23] = xami - (i[20] - i[28]).TimesMinusI() + (i[26] - i[18] + (i[22] - i[30]).TimesMinusI()).TimesMinusI();

            tmp[24] = i[17] + i[19] + xfpn + i[23] + i[25] + i[27] + i[31];
            tmp[25] = omegas[4][1] * (i[17] - i[25] + (i[19] - i[27] + (i[23] - i[31]).TimesMinusI()) * omegas[3][1] + (xfmn).TimesMinusI());
            tmp[26] = omegas[4][2] * ((i[19] - i[23] + i[27] - i[31]).TimesMinusI() + i[17] - xfpn + i[25]);
            tmp[27] = omegas[4][3] * (omegas[3][3] * (i[27] - i[19] + (i[23] - i[31]).TimesMinusI()) - i[17] + i[25] + (xfmn).TimesMinusI());
            tmp[28] = (i[17] - i[19] + xfpn - i[23] + i[25] - i[27] - i[31]).TimesMinusI();
            tmp[29] = (i[17] - i[25] - (i[19] - i[27] + (i[23] - i[31]).TimesMinusI()) * omegas[3][1] + (xfmn).TimesMinusI()) * omegas[4][5];
            tmp[30] = omegas[4][6] * ((i[19] - i[23] + i[27] - i[31]).TimesMinusI() - i[17] + xfpn - i[25]);
            tmp[31] = omegas[4][7] * ((i[27] - i[19] + (i[23] - i[31]).TimesMinusI()).TimesMinusI() + i[17] - i[25] - (xfmn).TimesMinusI());


            //32 complex floats = 64 floats
            //Divided into 4 parts A, B, C, D = each containing 8 complex floats, so 16 floats
            //AVX takes 8 floats at once, so will calculate in halves of those parts
            //Tmp will ocntain 6 octets

            fixed (ComplexFloat* entry = result, om5 = omegas[5], t = tmp)
            {
                Vector256<float> a;
                Vector256<float> b;
                Vector256<float> bSwap;
                Vector256<float> aIm;
                Vector256<float> aRe;
                Vector256<float> aIM_bSwap;

                float* partA = (float*)entry;
                float* partB = partA + 16;
                float* partC = partA + 32;
                float* partD = partA + 48;
                float* omPart1 = (float*)om5;
                float* omPart2 = omPart1 + 16;
                float* tmpPart1 = (float*)t;
                float* tmpPart2 = tmpPart1 + 16;
                float* tmpPart3 = tmpPart1 + 32;
                float* tmpPart4 = tmpPart1 + 48;
                float* tmpPart5 = tmpPart1 + 64;
                float* tmpPart6 = tmpPart1 + 80;

                //Summing up result

                Avx2.Store(partA, Avx2.Add(Avx2.LoadVector256(tmpPart1), Avx2.LoadVector256(tmpPart2)));
                Avx2.Store(partA + 8, Avx2.Add(Avx2.LoadVector256(tmpPart1 + 8), Avx2.LoadVector256(tmpPart2 + 8)));
                Avx2.Store(partB, Avx2.Subtract(Avx2.LoadVector256(tmpPart1), Avx2.LoadVector256(tmpPart2)));
                Avx2.Store(partB + 8, Avx2.Subtract(Avx2.LoadVector256(tmpPart1 + 8), Avx2.LoadVector256(tmpPart2 + 8)));

                Avx2.Store(partC, Avx2.Add(Avx2.LoadVector256(tmpPart3), Avx2.LoadVector256(tmpPart4)));
                Avx2.Store(partC + 8, Avx2.Add(Avx2.LoadVector256(tmpPart3 + 8), Avx2.LoadVector256(tmpPart4 + 8)));
                Avx2.Store(partD, Avx2.Subtract(Avx2.LoadVector256(tmpPart3), Avx2.LoadVector256(tmpPart4)));
                Avx2.Store(partD + 8, Avx2.Subtract(Avx2.LoadVector256(tmpPart3 + 8), Avx2.LoadVector256(tmpPart4 + 8)));




                //-------------------------------------------------------------------------------------------------------------

                //First part of each 8 complex part

                //Tmp[0] = A + B
                Avx2.Store(tmpPart1, Avx2.Add(Avx2.LoadVector256(partA), Avx2.LoadVector256(partB)));
                //Tmp[1] = A - B
                Avx2.Store(tmpPart2, Avx2.Subtract(Avx2.LoadVector256(partA), Avx2.LoadVector256(partB)));
                //Tmp[2] = C + D
                Avx2.Store(tmpPart3, Avx2.Add(Avx2.LoadVector256(partC), Avx2.LoadVector256(partD)));
                //Tmp[3] = C - D
                Avx2.Store(tmpPart4, Avx2.Subtract(Avx2.LoadVector256(partC), Avx2.LoadVector256(partD)));

                //Complex multiplication based on: https://www.researchgate.net/figure/Vectorized-complex-multiplication-using-AVX-2_fig2_337532904

                //Tmp[4] = omega * (C+D)
                a = Avx2.LoadVector256(tmpPart3);
                b = Avx2.LoadVector256(omPart1);
                bSwap = Avx2.Shuffle(b, b, imm8bShuffle);
                aIm = Avx2.Shuffle(a, a, imm8aImShuffle);
                aRe = Avx2.Shuffle(a, a, imm8aReShuffle);
                aIM_bSwap = Avx.Multiply(aIm, bSwap);
                Avx2.Store(tmpPart5, Fma.MultiplyAddSubtract(aRe, b, aIM_bSwap));

                //Tmp[4] = omega * (C-D)
                a = Avx2.LoadVector256(tmpPart4);
                b = Avx2.LoadVector256(omPart2);
                bSwap = Avx2.Shuffle(b, b, imm8bShuffle);
                aIm = Avx2.Shuffle(a, a, imm8aImShuffle);
                aRe = Avx2.Shuffle(a, a, imm8aReShuffle);
                aIM_bSwap = Avx.Multiply(aIm, bSwap);
                Avx2.Store(tmpPart6, Fma.MultiplyAddSubtract(aRe, b, aIM_bSwap));

                //(A+B) + (C+D)
                Avx2.Store(partA, Avx.Add(Avx.LoadVector256(tmpPart1), Avx.LoadVector256(tmpPart3)));
                //(A-B) + (C-D)
                Avx2.Store(partB, Avx.Add(Avx.LoadVector256(tmpPart2), Avx.LoadVector256(tmpPart4)));
                //(A+B) + omega*(C+D)
                Avx2.Store(partC, Avx.Add(Avx.LoadVector256(tmpPart1), Avx.LoadVector256(tmpPart5)));
                //(A-B) + omega*(C-D)
                Avx2.Store(partD, Avx.Add(Avx.LoadVector256(tmpPart2), Avx.LoadVector256(tmpPart6)));

                //--------------------------------------------------------------------------------------------------------------

                //Second part of each 8 complex part

                //Tmp[0] = A + B
                Avx2.Store(tmpPart1, Avx2.Add(Avx2.LoadVector256(partA + 8), Avx2.LoadVector256(partB + 8)));
                //Tmp[1] = A - B
                Avx2.Store(tmpPart2, Avx2.Subtract(Avx2.LoadVector256(partA + 8), Avx2.LoadVector256(partB + 8)));
                //Tmp[2] = C + D
                Avx2.Store(tmpPart3, Avx2.Add(Avx2.LoadVector256(partC + 8), Avx2.LoadVector256(partD + 8)));
                //Tmp[2] = C - D
                Avx2.Store(tmpPart4, Avx2.Subtract(Avx2.LoadVector256(partC + 8), Avx2.LoadVector256(partD + 8)));

                //Complex multiplication based on: https://www.researchgate.net/figure/Vectorized-complex-multiplication-using-AVX-2_fig2_337532904

                //Tmp[4] = omega * (C+D)
                a = Avx2.LoadVector256(tmpPart3);
                b = Avx2.LoadVector256(omPart1 + 8);
                bSwap = Avx2.Shuffle(b, b, imm8bShuffle);
                aIm = Avx2.Shuffle(a, a, imm8aImShuffle);
                aRe = Avx2.Shuffle(a, a, imm8aReShuffle);
                aIM_bSwap = Avx.Multiply(aIm, bSwap);
                Avx2.Store(tmpPart5, Fma.MultiplyAddSubtract(aRe, b, aIM_bSwap));

                //Tmp[4] = omega * (C-D)
                a = Avx2.LoadVector256(tmpPart4);
                b = Avx2.LoadVector256(omPart2 + 8);
                bSwap = Avx2.Shuffle(b, b, imm8bShuffle);
                aIm = Avx2.Shuffle(a, a, imm8aImShuffle);
                aRe = Avx2.Shuffle(a, a, imm8aReShuffle);
                aIM_bSwap = Avx.Multiply(aIm, bSwap);
                Avx2.Store(tmpPart6, Fma.MultiplyAddSubtract(aRe, b, aIM_bSwap));

                //(A+B) + (C+D)
                Avx2.Store(partA + 8, Avx.Add(Avx.LoadVector256(tmpPart1), Avx.LoadVector256(tmpPart3)));
                //(A-B) + (C-D)
                Avx2.Store(partB + 8, Avx.Add(Avx.LoadVector256(tmpPart2), Avx.LoadVector256(tmpPart4)));
                //(A+B) + omega*(C+D)         
                Avx2.Store(partC + 8, Avx.Add(Avx.LoadVector256(tmpPart1), Avx.LoadVector256(tmpPart5)));
                //(A-B) + omega*(C-D)
                Avx2.Store(partD + 8, Avx.Add(Avx.LoadVector256(tmpPart2), Avx.LoadVector256(tmpPart6)));
            }
            return result;

            //ComplexFloat[] result = new ComplexFloat[32];
            //ArraySegment<ComplexFloat> partA = new ArraySegment<ComplexFloat>(i, 0, 16);
            //ArraySegment<ComplexFloat> partB = new ArraySegment<ComplexFloat>(i, 16, 16);
            //Kernel16(partA.ToArray(), ref omegas).CopyTo(result, 0);
            //Kernel16(partA.ToArray(), ref omegas).CopyTo(result, 16);
            //return result;
        }
    }
}
