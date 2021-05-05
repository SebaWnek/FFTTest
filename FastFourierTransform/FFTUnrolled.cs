using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FastFourierTransform
{
    public static class FFTUnrolled
    {
        static ComplexFloat[][] omegas;
        static int[] map;
        static int portion = 64; //64 numbers at once, so 128 floats, so 16x 8 float registers
        static int addressShift = portion * 2;
        static byte shuffleMaskL1 = 0b01001110;
        static byte permuteMaskL2 = 0b00000001;
        static float t = -1;
        static float f = 0;
        static Vector256<float> storePlusMaskL1 = Vector256.Create(t, t, f, f, t, t, f, f);
        static Vector256<float> storeMinusMaskL1 = Vector256.Create(f, f, t, t, f, f, t, t);
        static Vector256<float> storePlusMaskL2 = Vector256.Create(t, t, t, t, f, f, f, f);
        static Vector256<float> storeMinusMaskL2 = Vector256.Create(f, f, f, f, t, t, t, t);
        static byte imm8bShuffle = 0b10110001;
        static byte imm8aImShuffle = 0b11110101;
        static byte imm8aReShuffle = 0b10100000;
        public static ComplexFloat[] ShuffleNumbers(ComplexFloat[] input, bool newMap = true)
        {
            int n = input.Length;
            if (newMap) map = BitReverse.GenerateMap((int)Math.Log2(n));
            ComplexFloat[] result = new ComplexFloat[n];

            for (int i = 0; i < n; i++)
            {
                result[map[i]] = input[i];
            }

            return result;
        }

        public static ComplexFloat[,] ShuffleNumbers(ComplexFloat[,] input, bool newMap = true)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            int currentCol = 0;

            if (newMap) map = BitReverse.GenerateMap((int)Math.Log2(rows));
            ComplexFloat[,] result = new ComplexFloat[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                currentCol = map[i];
                for (int j = 0; j < cols; j++)
                {
                    result[j, currentCol] = input[i, j];
                }
            }

            return result;
        }

        public static unsafe ComplexFloat[] FFT(ref ComplexFloat[] input, bool generateMapOmegas = true)
        {
            int n = input.Length;
            int k = (int)Math.Log2(n);
            int runs = n / portion;
            int additRuns = runs - 6;
            int tmpShift = 0;
            int tmpJump = 0;

            Vector256<float> partA, partB, partC, partD, partE, partF, partG, partH, partI, partJ, partK, partL, partM, partN, partO, partP;
            Vector256<float> partAs, partBs, partCs, partDs, partEs, partFs, partGs, partHs, partIs, partJs, partKs, partLs, partMs, partNs, partOs, partPs;
            Vector256<float> tmpA, tmpB, tmpC, tmpD, tmpE, tmpF, tmpG, tmpH, tmpI, tmpJ, tmpK, tmpL, tmpM, tmpN, tmpO, tmpP;
            Vector256<float> omA, omB, omC, omD, omE, omF, omG, omH, omI, omJ, omK, omL, omM, omN, omO, omP;
            Vector256<float> bSwapA, aIm, aRe, aIm_bSwap, bSwapB, bSwapC, bSwapD, bSwapE, bSwapF, bSwapG, bSwapH, bSwapI, bSwapJ, bSwapK, bSwapL, bSwapM, bSwapN, bSwapO, bSwapP;

            ComplexFloat[] result = new ComplexFloat[n];
            if (generateMapOmegas)
            {
                map = BitReverse.GenerateMap(k);
                omegas = OmegaCalculator.GenerateOmegas(k);
            }

            result = ShuffleNumbers(input, true);

            fixed (ComplexFloat* inputPtr = input, resPtr = result, om3 = omegas[3], om4 = omegas[4], om5 = omegas[5], om6 = omegas[6])
            {
                float* iPtr = (float*)inputPtr;
                float* rPtr = (float*)resPtr;

                #region L1
                for (int i = 0; i < runs; i++)
                {
                    partA = Avx2.LoadVector256(i * addressShift + rPtr + 0);
                    partB = Avx2.LoadVector256(i * addressShift + rPtr + 8);
                    partC = Avx2.LoadVector256(i * addressShift + rPtr + 16);
                    partD = Avx2.LoadVector256(i * addressShift + rPtr + 24);
                    partE = Avx2.LoadVector256(i * addressShift + rPtr + 32);
                    partF = Avx2.LoadVector256(i * addressShift + rPtr + 40);
                    partG = Avx2.LoadVector256(i * addressShift + rPtr + 48);
                    partH = Avx2.LoadVector256(i * addressShift + rPtr + 56);
                    partI = Avx2.LoadVector256(i * addressShift + rPtr + 64);
                    partJ = Avx2.LoadVector256(i * addressShift + rPtr + 72);
                    partK = Avx2.LoadVector256(i * addressShift + rPtr + 80);
                    partL = Avx2.LoadVector256(i * addressShift + rPtr + 88);
                    partM = Avx2.LoadVector256(i * addressShift + rPtr + 96);
                    partN = Avx2.LoadVector256(i * addressShift + rPtr + 104);
                    partO = Avx2.LoadVector256(i * addressShift + rPtr + 112);
                    partP = Avx2.LoadVector256(i * addressShift + rPtr + 120);

                    partAs = Avx2.Shuffle(partA, partA, shuffleMaskL1);
                    partBs = Avx2.Shuffle(partB, partB, shuffleMaskL1);
                    partCs = Avx2.Shuffle(partC, partC, shuffleMaskL1);
                    partDs = Avx2.Shuffle(partD, partD, shuffleMaskL1);
                    partEs = Avx2.Shuffle(partE, partE, shuffleMaskL1);
                    partFs = Avx2.Shuffle(partF, partF, shuffleMaskL1);
                    partGs = Avx2.Shuffle(partG, partG, shuffleMaskL1);
                    partHs = Avx2.Shuffle(partH, partH, shuffleMaskL1);
                    partIs = Avx2.Shuffle(partI, partI, shuffleMaskL1);
                    partJs = Avx2.Shuffle(partJ, partJ, shuffleMaskL1);
                    partKs = Avx2.Shuffle(partK, partK, shuffleMaskL1);
                    partLs = Avx2.Shuffle(partL, partL, shuffleMaskL1);
                    partMs = Avx2.Shuffle(partM, partM, shuffleMaskL1);
                    partNs = Avx2.Shuffle(partN, partN, shuffleMaskL1);
                    partOs = Avx2.Shuffle(partO, partO, shuffleMaskL1);
                    partPs = Avx2.Shuffle(partP, partP, shuffleMaskL1);

                    tmpA = Avx2.Add(partA, partAs);
                    tmpB = Avx2.Add(partB, partBs);
                    tmpC = Avx2.Add(partC, partCs);
                    tmpD = Avx2.Add(partD, partDs);
                    tmpE = Avx2.Add(partE, partEs);
                    tmpF = Avx2.Add(partF, partFs);
                    tmpG = Avx2.Add(partG, partGs);
                    tmpH = Avx2.Add(partH, partHs);
                    tmpI = Avx2.Add(partI, partIs);
                    tmpJ = Avx2.Add(partJ, partJs);
                    tmpK = Avx2.Add(partK, partKs);
                    tmpL = Avx2.Add(partL, partLs);
                    tmpM = Avx2.Add(partM, partMs);
                    tmpN = Avx2.Add(partN, partNs);
                    tmpO = Avx2.Add(partO, partOs);
                    tmpP = Avx2.Add(partP, partPs);

                    Avx2.MaskStore(i * addressShift + rPtr + 0, storePlusMaskL1, tmpA);
                    Avx2.MaskStore(i * addressShift + rPtr + 8, storePlusMaskL1, tmpB);
                    Avx2.MaskStore(i * addressShift + rPtr + 16, storePlusMaskL1, tmpC);
                    Avx2.MaskStore(i * addressShift + rPtr + 24, storePlusMaskL1, tmpD);
                    Avx2.MaskStore(i * addressShift + rPtr + 32, storePlusMaskL1, tmpE);
                    Avx2.MaskStore(i * addressShift + rPtr + 40, storePlusMaskL1, tmpF);
                    Avx2.MaskStore(i * addressShift + rPtr + 48, storePlusMaskL1, tmpG);
                    Avx2.MaskStore(i * addressShift + rPtr + 56, storePlusMaskL1, tmpH);
                    Avx2.MaskStore(i * addressShift + rPtr + 64, storePlusMaskL1, tmpI);
                    Avx2.MaskStore(i * addressShift + rPtr + 72, storePlusMaskL1, tmpJ);
                    Avx2.MaskStore(i * addressShift + rPtr + 80, storePlusMaskL1, tmpK);
                    Avx2.MaskStore(i * addressShift + rPtr + 88, storePlusMaskL1, tmpL);
                    Avx2.MaskStore(i * addressShift + rPtr + 96, storePlusMaskL1, tmpM);
                    Avx2.MaskStore(i * addressShift + rPtr + 104, storePlusMaskL1, tmpN);
                    Avx2.MaskStore(i * addressShift + rPtr + 112, storePlusMaskL1, tmpO);
                    Avx2.MaskStore(i * addressShift + rPtr + 120, storePlusMaskL1, tmpP);

                    tmpA = Avx2.Subtract(partAs, partA);
                    tmpB = Avx2.Subtract(partBs, partB);
                    tmpC = Avx2.Subtract(partCs, partC);
                    tmpD = Avx2.Subtract(partDs, partD);
                    tmpE = Avx2.Subtract(partEs, partE);
                    tmpF = Avx2.Subtract(partFs, partF);
                    tmpG = Avx2.Subtract(partGs, partG);
                    tmpH = Avx2.Subtract(partHs, partH);
                    tmpI = Avx2.Subtract(partIs, partI);
                    tmpJ = Avx2.Subtract(partJs, partJ);
                    tmpK = Avx2.Subtract(partKs, partK);
                    tmpL = Avx2.Subtract(partLs, partL);
                    tmpM = Avx2.Subtract(partMs, partM);
                    tmpN = Avx2.Subtract(partNs, partN);
                    tmpO = Avx2.Subtract(partOs, partO);
                    tmpP = Avx2.Subtract(partPs, partP);

                    Avx2.MaskStore(i * addressShift + rPtr + 0, storeMinusMaskL1, tmpA);
                    Avx2.MaskStore(i * addressShift + rPtr + 8, storeMinusMaskL1, tmpB);
                    Avx2.MaskStore(i * addressShift + rPtr + 16, storeMinusMaskL1, tmpC);
                    Avx2.MaskStore(i * addressShift + rPtr + 24, storeMinusMaskL1, tmpD);
                    Avx2.MaskStore(i * addressShift + rPtr + 32, storeMinusMaskL1, tmpE);
                    Avx2.MaskStore(i * addressShift + rPtr + 40, storeMinusMaskL1, tmpF);
                    Avx2.MaskStore(i * addressShift + rPtr + 48, storeMinusMaskL1, tmpG);
                    Avx2.MaskStore(i * addressShift + rPtr + 56, storeMinusMaskL1, tmpH);
                    Avx2.MaskStore(i * addressShift + rPtr + 64, storeMinusMaskL1, tmpI);
                    Avx2.MaskStore(i * addressShift + rPtr + 72, storeMinusMaskL1, tmpJ);
                    Avx2.MaskStore(i * addressShift + rPtr + 80, storeMinusMaskL1, tmpK);
                    Avx2.MaskStore(i * addressShift + rPtr + 88, storeMinusMaskL1, tmpL);
                    Avx2.MaskStore(i * addressShift + rPtr + 96, storeMinusMaskL1, tmpM);
                    Avx2.MaskStore(i * addressShift + rPtr + 104, storeMinusMaskL1, tmpN);
                    Avx2.MaskStore(i * addressShift + rPtr + 112, storeMinusMaskL1, tmpO);
                    Avx2.MaskStore(i * addressShift + rPtr + 120, storeMinusMaskL1, tmpP);
                }
                #endregion
                #region L2
                for (int i = 0; i < runs; i++)
                {
                    result[i * portion + 3] = result[i * portion + 3].TimesMinusI();
                    result[i * portion + 7] = result[i * portion + 7].TimesMinusI();
                    result[i * portion + 11] = result[i * portion + 11].TimesMinusI();
                    result[i * portion + 15] = result[i * portion + 15].TimesMinusI();
                    result[i * portion + 19] = result[i * portion + 19].TimesMinusI();
                    result[i * portion + 23] = result[i * portion + 23].TimesMinusI();
                    result[i * portion + 27] = result[i * portion + 27].TimesMinusI();
                    result[i * portion + 31] = result[i * portion + 31].TimesMinusI();
                    result[i * portion + 35] = result[i * portion + 35].TimesMinusI();
                    result[i * portion + 39] = result[i * portion + 39].TimesMinusI();
                    result[i * portion + 43] = result[i * portion + 43].TimesMinusI();
                    result[i * portion + 47] = result[i * portion + 47].TimesMinusI();
                    result[i * portion + 51] = result[i * portion + 51].TimesMinusI();
                    result[i * portion + 55] = result[i * portion + 55].TimesMinusI();
                    result[i * portion + 59] = result[i * portion + 59].TimesMinusI();
                    result[i * portion + 63] = result[i * portion + 63].TimesMinusI();

                    partA = Avx2.LoadVector256(i * addressShift + rPtr + 0);
                    partB = Avx2.LoadVector256(i * addressShift + rPtr + 8);
                    partC = Avx2.LoadVector256(i * addressShift + rPtr + 16);
                    partD = Avx2.LoadVector256(i * addressShift + rPtr + 24);
                    partE = Avx2.LoadVector256(i * addressShift + rPtr + 32);
                    partF = Avx2.LoadVector256(i * addressShift + rPtr + 40);
                    partG = Avx2.LoadVector256(i * addressShift + rPtr + 48);
                    partH = Avx2.LoadVector256(i * addressShift + rPtr + 56);
                    partI = Avx2.LoadVector256(i * addressShift + rPtr + 64);
                    partJ = Avx2.LoadVector256(i * addressShift + rPtr + 72);
                    partK = Avx2.LoadVector256(i * addressShift + rPtr + 80);
                    partL = Avx2.LoadVector256(i * addressShift + rPtr + 88);
                    partM = Avx2.LoadVector256(i * addressShift + rPtr + 96);
                    partN = Avx2.LoadVector256(i * addressShift + rPtr + 104);
                    partO = Avx2.LoadVector256(i * addressShift + rPtr + 112);
                    partP = Avx2.LoadVector256(i * addressShift + rPtr + 120);

                    partAs = Avx2.Permute2x128(partA, partA, permuteMaskL2);
                    partBs = Avx2.Permute2x128(partB, partB, permuteMaskL2);
                    partCs = Avx2.Permute2x128(partC, partC, permuteMaskL2);
                    partDs = Avx2.Permute2x128(partD, partD, permuteMaskL2);
                    partEs = Avx2.Permute2x128(partE, partE, permuteMaskL2);
                    partFs = Avx2.Permute2x128(partF, partF, permuteMaskL2);
                    partGs = Avx2.Permute2x128(partG, partG, permuteMaskL2);
                    partHs = Avx2.Permute2x128(partH, partH, permuteMaskL2);
                    partIs = Avx2.Permute2x128(partI, partI, permuteMaskL2);
                    partJs = Avx2.Permute2x128(partJ, partJ, permuteMaskL2);
                    partKs = Avx2.Permute2x128(partK, partK, permuteMaskL2);
                    partLs = Avx2.Permute2x128(partL, partL, permuteMaskL2);
                    partMs = Avx2.Permute2x128(partM, partM, permuteMaskL2);
                    partNs = Avx2.Permute2x128(partN, partN, permuteMaskL2);
                    partOs = Avx2.Permute2x128(partO, partO, permuteMaskL2);
                    partPs = Avx2.Permute2x128(partP, partP, permuteMaskL2);

                    tmpA = Avx2.Add(partA, partAs);
                    tmpB = Avx2.Add(partB, partBs);
                    tmpC = Avx2.Add(partC, partCs);
                    tmpD = Avx2.Add(partD, partDs);
                    tmpE = Avx2.Add(partE, partEs);
                    tmpF = Avx2.Add(partF, partFs);
                    tmpG = Avx2.Add(partG, partGs);
                    tmpH = Avx2.Add(partH, partHs);
                    tmpI = Avx2.Add(partI, partIs);
                    tmpJ = Avx2.Add(partJ, partJs);
                    tmpK = Avx2.Add(partK, partKs);
                    tmpL = Avx2.Add(partL, partLs);
                    tmpM = Avx2.Add(partM, partMs);
                    tmpN = Avx2.Add(partN, partNs);
                    tmpO = Avx2.Add(partO, partOs);
                    tmpP = Avx2.Add(partP, partPs);

                    Avx2.MaskStore(i * addressShift + rPtr + 0, storePlusMaskL2, tmpA);
                    Avx2.MaskStore(i * addressShift + rPtr + 8, storePlusMaskL2, tmpB);
                    Avx2.MaskStore(i * addressShift + rPtr + 16, storePlusMaskL2, tmpC);
                    Avx2.MaskStore(i * addressShift + rPtr + 24, storePlusMaskL2, tmpD);
                    Avx2.MaskStore(i * addressShift + rPtr + 32, storePlusMaskL2, tmpE);
                    Avx2.MaskStore(i * addressShift + rPtr + 40, storePlusMaskL2, tmpF);
                    Avx2.MaskStore(i * addressShift + rPtr + 48, storePlusMaskL2, tmpG);
                    Avx2.MaskStore(i * addressShift + rPtr + 56, storePlusMaskL2, tmpH);
                    Avx2.MaskStore(i * addressShift + rPtr + 64, storePlusMaskL2, tmpI);
                    Avx2.MaskStore(i * addressShift + rPtr + 72, storePlusMaskL2, tmpJ);
                    Avx2.MaskStore(i * addressShift + rPtr + 80, storePlusMaskL2, tmpK);
                    Avx2.MaskStore(i * addressShift + rPtr + 88, storePlusMaskL2, tmpL);
                    Avx2.MaskStore(i * addressShift + rPtr + 96, storePlusMaskL2, tmpM);
                    Avx2.MaskStore(i * addressShift + rPtr + 104, storePlusMaskL2, tmpN);
                    Avx2.MaskStore(i * addressShift + rPtr + 112, storePlusMaskL2, tmpO);
                    Avx2.MaskStore(i * addressShift + rPtr + 120, storePlusMaskL2, tmpP);

                    tmpA = Avx2.Subtract(partAs, partA);
                    tmpB = Avx2.Subtract(partBs, partB);
                    tmpC = Avx2.Subtract(partCs, partC);
                    tmpD = Avx2.Subtract(partDs, partD);
                    tmpE = Avx2.Subtract(partEs, partE);
                    tmpF = Avx2.Subtract(partFs, partF);
                    tmpG = Avx2.Subtract(partGs, partG);
                    tmpH = Avx2.Subtract(partHs, partH);
                    tmpI = Avx2.Subtract(partIs, partI);
                    tmpJ = Avx2.Subtract(partJs, partJ);
                    tmpK = Avx2.Subtract(partKs, partK);
                    tmpL = Avx2.Subtract(partLs, partL);
                    tmpM = Avx2.Subtract(partMs, partM);
                    tmpN = Avx2.Subtract(partNs, partN);
                    tmpO = Avx2.Subtract(partOs, partO);
                    tmpP = Avx2.Subtract(partPs, partP);

                    Avx2.MaskStore(i * addressShift + rPtr + 0, storeMinusMaskL2, tmpA);
                    Avx2.MaskStore(i * addressShift + rPtr + 8, storeMinusMaskL2, tmpB);
                    Avx2.MaskStore(i * addressShift + rPtr + 16, storeMinusMaskL2, tmpC);
                    Avx2.MaskStore(i * addressShift + rPtr + 24, storeMinusMaskL2, tmpD);
                    Avx2.MaskStore(i * addressShift + rPtr + 32, storeMinusMaskL2, tmpE);
                    Avx2.MaskStore(i * addressShift + rPtr + 40, storeMinusMaskL2, tmpF);
                    Avx2.MaskStore(i * addressShift + rPtr + 48, storeMinusMaskL2, tmpG);
                    Avx2.MaskStore(i * addressShift + rPtr + 56, storeMinusMaskL2, tmpH);
                    Avx2.MaskStore(i * addressShift + rPtr + 64, storeMinusMaskL2, tmpI);
                    Avx2.MaskStore(i * addressShift + rPtr + 72, storeMinusMaskL2, tmpJ);
                    Avx2.MaskStore(i * addressShift + rPtr + 80, storeMinusMaskL2, tmpK);
                    Avx2.MaskStore(i * addressShift + rPtr + 88, storeMinusMaskL2, tmpL);
                    Avx2.MaskStore(i * addressShift + rPtr + 96, storeMinusMaskL2, tmpM);
                    Avx2.MaskStore(i * addressShift + rPtr + 104, storeMinusMaskL2, tmpN);
                    Avx2.MaskStore(i * addressShift + rPtr + 112, storeMinusMaskL2, tmpO);
                    Avx2.MaskStore(i * addressShift + rPtr + 120, storeMinusMaskL2, tmpP);
                }
                #endregion
                #region L3
                for (int i = 0; i < runs; i++)
                {
                    omA = Avx2.LoadVector256((float*)om3);

                    partA = Avx2.LoadVector256(i * addressShift + rPtr + 0);
                    partB = Avx2.LoadVector256(i * addressShift + rPtr + 8);
                    partC = Avx2.LoadVector256(i * addressShift + rPtr + 16);
                    partD = Avx2.LoadVector256(i * addressShift + rPtr + 24);
                    partE = Avx2.LoadVector256(i * addressShift + rPtr + 32);
                    partF = Avx2.LoadVector256(i * addressShift + rPtr + 40);
                    partG = Avx2.LoadVector256(i * addressShift + rPtr + 48);
                    partH = Avx2.LoadVector256(i * addressShift + rPtr + 56);
                    partI = Avx2.LoadVector256(i * addressShift + rPtr + 64);
                    partJ = Avx2.LoadVector256(i * addressShift + rPtr + 72);
                    partK = Avx2.LoadVector256(i * addressShift + rPtr + 80);
                    partL = Avx2.LoadVector256(i * addressShift + rPtr + 88);
                    partM = Avx2.LoadVector256(i * addressShift + rPtr + 96);
                    partN = Avx2.LoadVector256(i * addressShift + rPtr + 104);
                    partO = Avx2.LoadVector256(i * addressShift + rPtr + 112);
                    partP = Avx2.LoadVector256(i * addressShift + rPtr + 120);


                    //a = data, b = omegas
                    bSwapA = Avx2.Shuffle(omA, omA, imm8bShuffle);

                    //I have no idea why those 2 lines work in reverse, but they work at least...
                    aIm = Avx2.Shuffle(partB, partB, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partB, partB, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpB = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partD, partD, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partD, partD, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpD = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partF, partF, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partF, partF, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpF = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partH, partH, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partH, partH, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpH = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partJ, partJ, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partJ, partJ, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpJ = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partL, partL, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partL, partL, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpL = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partN, partN, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partN, partN, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpN = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partP, partP, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partP, partP, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpP = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    Avx2.Store(i * addressShift + rPtr + 0, Avx2.Add(partA, tmpB));
                    Avx2.Store(i * addressShift + rPtr + 8, Avx2.Subtract(partA, tmpB));
                    Avx2.Store(i * addressShift + rPtr + 16, Avx2.Add(partC, tmpD));
                    Avx2.Store(i * addressShift + rPtr + 24, Avx2.Subtract(partC, tmpD));
                    Avx2.Store(i * addressShift + rPtr + 32, Avx2.Add(partE, tmpF));
                    Avx2.Store(i * addressShift + rPtr + 40, Avx2.Subtract(partE, tmpF));
                    Avx2.Store(i * addressShift + rPtr + 48, Avx2.Add(partG, tmpH));
                    Avx2.Store(i * addressShift + rPtr + 56, Avx2.Subtract(partG, tmpH));
                    Avx2.Store(i * addressShift + rPtr + 64, Avx2.Add(partI, tmpJ));
                    Avx2.Store(i * addressShift + rPtr + 72, Avx2.Subtract(partI, tmpJ));
                    Avx2.Store(i * addressShift + rPtr + 80, Avx2.Add(partK, tmpL));
                    Avx2.Store(i * addressShift + rPtr + 88, Avx2.Subtract(partK, tmpL));
                    Avx2.Store(i * addressShift + rPtr + 96, Avx2.Add(partM, tmpN));
                    Avx2.Store(i * addressShift + rPtr + 104, Avx2.Subtract(partM, tmpN));
                    Avx2.Store(i * addressShift + rPtr + 112, Avx2.Add(partO, tmpP));
                    Avx2.Store(i * addressShift + rPtr + 120, Avx2.Subtract(partO, tmpP));



                }
                #endregion
                #region L4
                for (int i = 0; i < runs; i++)
                {
                    omA = Avx2.LoadVector256((float*)om4);
                    omB = Avx2.LoadVector256((float*)(om4 + 4));


                    bSwapA = Avx2.Shuffle(omA, omA, imm8bShuffle);
                    bSwapB = Avx2.Shuffle(omB, omB, imm8bShuffle);

                    partA = Avx2.LoadVector256(i * addressShift + rPtr + 0);
                    partB = Avx2.LoadVector256(i * addressShift + rPtr + 8);
                    partC = Avx2.LoadVector256(i * addressShift + rPtr + 16);
                    partD = Avx2.LoadVector256(i * addressShift + rPtr + 24);
                    partE = Avx2.LoadVector256(i * addressShift + rPtr + 32);
                    partF = Avx2.LoadVector256(i * addressShift + rPtr + 40);
                    partG = Avx2.LoadVector256(i * addressShift + rPtr + 48);
                    partH = Avx2.LoadVector256(i * addressShift + rPtr + 56);
                    partI = Avx2.LoadVector256(i * addressShift + rPtr + 64);
                    partJ = Avx2.LoadVector256(i * addressShift + rPtr + 72);
                    partK = Avx2.LoadVector256(i * addressShift + rPtr + 80);
                    partL = Avx2.LoadVector256(i * addressShift + rPtr + 88);
                    partM = Avx2.LoadVector256(i * addressShift + rPtr + 96);
                    partN = Avx2.LoadVector256(i * addressShift + rPtr + 104);
                    partO = Avx2.LoadVector256(i * addressShift + rPtr + 112);
                    partP = Avx2.LoadVector256(i * addressShift + rPtr + 120);


                    aIm = Avx2.Shuffle(partC, partC, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partC, partC, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpC = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partD, partD, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partD, partD, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpD = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);


                    aIm = Avx2.Shuffle(partG, partG, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partG, partG, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpG = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partH, partH, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partH, partH, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpH = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);


                    aIm = Avx2.Shuffle(partK, partK, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partK, partK, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpK = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partL, partL, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partL, partL, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpL = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);


                    aIm = Avx2.Shuffle(partO, partO, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partO, partO, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpO = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partP, partP, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partP, partP, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpP = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);

                    Avx2.Store(i * addressShift + rPtr + 0, Avx2.Add(partA, tmpC));
                    Avx2.Store(i * addressShift + rPtr + 8, Avx2.Add(partB, tmpD));
                    Avx2.Store(i * addressShift + rPtr + 16, Avx2.Subtract(partA, tmpC));
                    Avx2.Store(i * addressShift + rPtr + 24, Avx2.Subtract(partB, tmpD));
                    Avx2.Store(i * addressShift + rPtr + 32, Avx2.Add(partE, tmpG));
                    Avx2.Store(i * addressShift + rPtr + 40, Avx2.Add(partF, tmpH));
                    Avx2.Store(i * addressShift + rPtr + 48, Avx2.Subtract(partE, tmpG));
                    Avx2.Store(i * addressShift + rPtr + 56, Avx2.Subtract(partF, tmpH));
                    Avx2.Store(i * addressShift + rPtr + 64, Avx2.Add(partI, tmpK));
                    Avx2.Store(i * addressShift + rPtr + 72, Avx2.Add(partJ, tmpL));
                    Avx2.Store(i * addressShift + rPtr + 80, Avx2.Subtract(partI, tmpK));
                    Avx2.Store(i * addressShift + rPtr + 88, Avx2.Subtract(partJ, tmpL));
                    Avx2.Store(i * addressShift + rPtr + 96, Avx2.Add(partM, tmpO));
                    Avx2.Store(i * addressShift + rPtr + 104, Avx2.Add(partN, tmpP));
                    Avx2.Store(i * addressShift + rPtr + 112, Avx2.Subtract(partM, tmpO));
                    Avx2.Store(i * addressShift + rPtr + 120, Avx2.Subtract(partN, tmpP));
                }
                #endregion
                #region L5
                for (int i = 0; i < runs; i++)
                {
                    omA = Avx2.LoadVector256((float*)om5);
                    omB = Avx2.LoadVector256((float*)(om5 + 4));
                    omC = Avx2.LoadVector256((float*)(om5 + 8));
                    omD = Avx2.LoadVector256((float*)(om5 + 12));

                    bSwapA = Avx2.Shuffle(omA, omA, imm8bShuffle);
                    bSwapB = Avx2.Shuffle(omB, omB, imm8bShuffle);
                    bSwapC = Avx2.Shuffle(omC, omC, imm8bShuffle);
                    bSwapD = Avx2.Shuffle(omD, omD, imm8bShuffle);

                    partA = Avx2.LoadVector256(i * addressShift + rPtr + 0);
                    partB = Avx2.LoadVector256(i * addressShift + rPtr + 8);
                    partC = Avx2.LoadVector256(i * addressShift + rPtr + 16);
                    partD = Avx2.LoadVector256(i * addressShift + rPtr + 24);
                    partE = Avx2.LoadVector256(i * addressShift + rPtr + 32);
                    partF = Avx2.LoadVector256(i * addressShift + rPtr + 40);
                    partG = Avx2.LoadVector256(i * addressShift + rPtr + 48);
                    partH = Avx2.LoadVector256(i * addressShift + rPtr + 56);
                    partI = Avx2.LoadVector256(i * addressShift + rPtr + 64);
                    partJ = Avx2.LoadVector256(i * addressShift + rPtr + 72);
                    partK = Avx2.LoadVector256(i * addressShift + rPtr + 80);
                    partL = Avx2.LoadVector256(i * addressShift + rPtr + 88);
                    partM = Avx2.LoadVector256(i * addressShift + rPtr + 96);
                    partN = Avx2.LoadVector256(i * addressShift + rPtr + 104);
                    partO = Avx2.LoadVector256(i * addressShift + rPtr + 112);
                    partP = Avx2.LoadVector256(i * addressShift + rPtr + 120);


                    aIm = Avx2.Shuffle(partE, partE, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partE, partE, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpE = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partF, partF, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partF, partF, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpF = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);

                    aIm = Avx2.Shuffle(partG, partG, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partG, partG, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapC);
                    tmpG = Fma.MultiplyAddSubtract(aRe, omC, aIm_bSwap);

                    aIm = Avx2.Shuffle(partH, partH, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partH, partH, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapD);
                    tmpH = Fma.MultiplyAddSubtract(aRe, omD, aIm_bSwap);




                    aIm = Avx2.Shuffle(partM, partM, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partM, partM, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpM = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partN, partN, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partN, partN, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpN = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);

                    aIm = Avx2.Shuffle(partO, partO, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partO, partO, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapC);
                    tmpO = Fma.MultiplyAddSubtract(aRe, omC, aIm_bSwap);

                    aIm = Avx2.Shuffle(partP, partP, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partP, partP, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapD);
                    tmpP = Fma.MultiplyAddSubtract(aRe, omD, aIm_bSwap);

                    Avx2.Store(i * addressShift + rPtr + 0, Avx2.Add(partA, tmpE));
                    Avx2.Store(i * addressShift + rPtr + 8, Avx2.Add(partB, tmpF));
                    Avx2.Store(i * addressShift + rPtr + 16, Avx2.Add(partC, tmpG));
                    Avx2.Store(i * addressShift + rPtr + 24, Avx2.Add(partD, tmpH));
                    Avx2.Store(i * addressShift + rPtr + 32, Avx2.Subtract(partA, tmpE));
                    Avx2.Store(i * addressShift + rPtr + 40, Avx2.Subtract(partB, tmpF));
                    Avx2.Store(i * addressShift + rPtr + 48, Avx2.Subtract(partC, tmpG));
                    Avx2.Store(i * addressShift + rPtr + 56, Avx2.Subtract(partD, tmpH));
                    Avx2.Store(i * addressShift + rPtr + 64, Avx2.Add(partI, tmpM));
                    Avx2.Store(i * addressShift + rPtr + 72, Avx2.Add(partJ, tmpN));
                    Avx2.Store(i * addressShift + rPtr + 80, Avx2.Add(partK, tmpO));
                    Avx2.Store(i * addressShift + rPtr + 88, Avx2.Add(partL, tmpP));
                    Avx2.Store(i * addressShift + rPtr + 96, Avx2.Subtract(partI, tmpM));
                    Avx2.Store(i * addressShift + rPtr + 104, Avx2.Subtract(partJ, tmpN));
                    Avx2.Store(i * addressShift + rPtr + 112, Avx2.Subtract(partK, tmpO));
                    Avx2.Store(i * addressShift + rPtr + 120, Avx2.Subtract(partL, tmpP));
                }

                #endregion
                #region L6
                for (int i = 0; i < runs; i++)
                {
                    omA = Avx2.LoadVector256((float*)om6);
                    omB = Avx2.LoadVector256((float*)(om6 + 4));
                    omC = Avx2.LoadVector256((float*)(om6 + 8));
                    omD = Avx2.LoadVector256((float*)(om6 + 12));
                    omE = Avx2.LoadVector256((float*)(om6 + 16));
                    omF = Avx2.LoadVector256((float*)(om6 + 20));
                    omG = Avx2.LoadVector256((float*)(om6 + 24));
                    omH = Avx2.LoadVector256((float*)(om6 + 28));

                    bSwapA = Avx2.Shuffle(omA, omA, imm8bShuffle);
                    bSwapB = Avx2.Shuffle(omB, omB, imm8bShuffle);
                    bSwapC = Avx2.Shuffle(omC, omC, imm8bShuffle);
                    bSwapD = Avx2.Shuffle(omD, omD, imm8bShuffle);
                    bSwapE = Avx2.Shuffle(omE, omE, imm8bShuffle);
                    bSwapF = Avx2.Shuffle(omF, omF, imm8bShuffle);
                    bSwapG = Avx2.Shuffle(omG, omG, imm8bShuffle);
                    bSwapH = Avx2.Shuffle(omH, omH, imm8bShuffle);

                    partA = Avx2.LoadVector256(i * addressShift + rPtr + 0);
                    partB = Avx2.LoadVector256(i * addressShift + rPtr + 8);
                    partC = Avx2.LoadVector256(i * addressShift + rPtr + 16);
                    partD = Avx2.LoadVector256(i * addressShift + rPtr + 24);
                    partE = Avx2.LoadVector256(i * addressShift + rPtr + 32);
                    partF = Avx2.LoadVector256(i * addressShift + rPtr + 40);
                    partG = Avx2.LoadVector256(i * addressShift + rPtr + 48);
                    partH = Avx2.LoadVector256(i * addressShift + rPtr + 56);
                    partI = Avx2.LoadVector256(i * addressShift + rPtr + 64);
                    partJ = Avx2.LoadVector256(i * addressShift + rPtr + 72);
                    partK = Avx2.LoadVector256(i * addressShift + rPtr + 80);
                    partL = Avx2.LoadVector256(i * addressShift + rPtr + 88);
                    partM = Avx2.LoadVector256(i * addressShift + rPtr + 96);
                    partN = Avx2.LoadVector256(i * addressShift + rPtr + 104);
                    partO = Avx2.LoadVector256(i * addressShift + rPtr + 112);
                    partP = Avx2.LoadVector256(i * addressShift + rPtr + 120);


                    aIm = Avx2.Shuffle(partI, partI, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partI, partI, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                    tmpI = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                    aIm = Avx2.Shuffle(partJ, partJ, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partJ, partJ, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                    tmpJ = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);

                    aIm = Avx2.Shuffle(partK, partK, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partK, partK, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapC);
                    tmpK = Fma.MultiplyAddSubtract(aRe, omC, aIm_bSwap);

                    aIm = Avx2.Shuffle(partL, partL, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partL, partH, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapD);
                    tmpL = Fma.MultiplyAddSubtract(aRe, omD, aIm_bSwap);

                    aIm = Avx2.Shuffle(partM, partM, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partM, partM, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapE);
                    tmpM = Fma.MultiplyAddSubtract(aRe, omE, aIm_bSwap);

                    aIm = Avx2.Shuffle(partN, partN, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partN, partN, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapF);
                    tmpN = Fma.MultiplyAddSubtract(aRe, omF, aIm_bSwap);

                    aIm = Avx2.Shuffle(partO, partO, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partO, partO, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapG);
                    tmpO = Fma.MultiplyAddSubtract(aRe, omG, aIm_bSwap);

                    aIm = Avx2.Shuffle(partP, partP, imm8aImShuffle);
                    aRe = Avx2.Shuffle(partP, partP, imm8aReShuffle);
                    aIm_bSwap = Avx.Multiply(aIm, bSwapH);
                    tmpP = Fma.MultiplyAddSubtract(aRe, omH, aIm_bSwap);

                    Avx2.Store(i * addressShift + rPtr + 0, Avx2.Add(partA, tmpI));
                    Avx2.Store(i * addressShift + rPtr + 8, Avx2.Add(partB, tmpJ));
                    Avx2.Store(i * addressShift + rPtr + 16, Avx2.Add(partC, tmpK));
                    Avx2.Store(i * addressShift + rPtr + 24, Avx2.Add(partD, tmpL));
                    Avx2.Store(i * addressShift + rPtr + 32, Avx2.Add(partE, tmpM));
                    Avx2.Store(i * addressShift + rPtr + 40, Avx2.Add(partF, tmpN));
                    Avx2.Store(i * addressShift + rPtr + 48, Avx2.Add(partG, tmpO));
                    Avx2.Store(i * addressShift + rPtr + 56, Avx2.Add(partH, tmpP));
                    Avx2.Store(i * addressShift + rPtr + 64, Avx2.Subtract(partA, tmpI));
                    Avx2.Store(i * addressShift + rPtr + 72, Avx2.Subtract(partB, tmpJ));
                    Avx2.Store(i * addressShift + rPtr + 80, Avx2.Subtract(partC, tmpK));
                    Avx2.Store(i * addressShift + rPtr + 88, Avx2.Subtract(partD, tmpL));
                    Avx2.Store(i * addressShift + rPtr + 96, Avx2.Subtract(partE, tmpM));
                    Avx2.Store(i * addressShift + rPtr + 104, Avx2.Subtract(partF, tmpN));
                    Avx2.Store(i * addressShift + rPtr + 112, Avx2.Subtract(partG, tmpO));
                    Avx2.Store(i * addressShift + rPtr + 120, Avx2.Subtract(partH, tmpP));
                }
                #endregion

                #region L7+

                for (int i = 1; i <= k-6; i++)
                {
                    fixed (ComplexFloat* omPtr = omegas[i+6])
                    {
                        tmpShift = 0;
                        tmpJump = (int)Math.Pow(2, i-1);
                        for (int j = 0; j < runs / 2; j++)
                        {
                            if (j % i == 0) tmpShift = j * 2;
                            else tmpShift += 1;

                            partA = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 0  );
                            partB = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 8  );
                            partC = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 16 );
                            partD = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 24 );
                            partE = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 32 );
                            partF = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 40 );
                            partG = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 48 );
                            partH = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 56 );
                            partI = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 64 );
                            partJ = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 72 );
                            partK = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 80 );
                            partL = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 88 );
                            partM = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 96 );
                            partN = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 104);
                            partO = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 112);
                            partP = Avx2.LoadVector256(tmpShift * addressShift + rPtr + 120);

                            partAs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 0  );
                            partBs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 8  );
                            partCs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 16 );
                            partDs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 24 );
                            partEs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 32 );
                            partFs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 40 );
                            partGs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 48 );
                            partHs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 56 );
                            partIs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 64 );
                            partJs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 72 );
                            partKs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 80 );
                            partLs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 88 );
                            partMs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 96 );
                            partNs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 104);
                            partOs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 112);
                            partPs = Avx2.LoadVector256((tmpShift + tmpJump) * addressShift + rPtr + 120);

                            omA = Avx2.LoadVector256((float*)(omPtr + 0));
                            omB = Avx2.LoadVector256((float*)(omPtr + 4));
                            omC = Avx2.LoadVector256((float*)(omPtr + 8));
                            omD = Avx2.LoadVector256((float*)(omPtr + 12));
                            omE = Avx2.LoadVector256((float*)(omPtr + 16));
                            omF = Avx2.LoadVector256((float*)(omPtr + 20));
                            omG = Avx2.LoadVector256((float*)(omPtr + 24));
                            omH = Avx2.LoadVector256((float*)(omPtr + 28));
                            omI = Avx2.LoadVector256((float*)(omPtr + 32));
                            omJ = Avx2.LoadVector256((float*)(omPtr + 36));
                            omK = Avx2.LoadVector256((float*)(omPtr + 40));
                            omL = Avx2.LoadVector256((float*)(omPtr + 44));
                            omM = Avx2.LoadVector256((float*)(omPtr + 48));
                            omN = Avx2.LoadVector256((float*)(omPtr + 52));
                            omO = Avx2.LoadVector256((float*)(omPtr + 56));
                            omP = Avx2.LoadVector256((float*)(omPtr + 60));

                            bSwapA = Avx2.Shuffle(omA, omA, imm8bShuffle);
                            bSwapB = Avx2.Shuffle(omB, omB, imm8bShuffle);
                            bSwapC = Avx2.Shuffle(omC, omC, imm8bShuffle);
                            bSwapD = Avx2.Shuffle(omD, omD, imm8bShuffle);
                            bSwapE = Avx2.Shuffle(omE, omE, imm8bShuffle);
                            bSwapF = Avx2.Shuffle(omF, omF, imm8bShuffle);
                            bSwapG = Avx2.Shuffle(omG, omG, imm8bShuffle);
                            bSwapH = Avx2.Shuffle(omH, omH, imm8bShuffle); 
                            bSwapI = Avx2.Shuffle(omI, omI, imm8bShuffle);
                            bSwapJ = Avx2.Shuffle(omJ, omJ, imm8bShuffle);
                            bSwapK = Avx2.Shuffle(omK, omK, imm8bShuffle);
                            bSwapL = Avx2.Shuffle(omL, omL, imm8bShuffle);
                            bSwapM = Avx2.Shuffle(omM, omM, imm8bShuffle);
                            bSwapN = Avx2.Shuffle(omN, omN, imm8bShuffle);
                            bSwapO = Avx2.Shuffle(omO, omO, imm8bShuffle);
                            bSwapP = Avx2.Shuffle(omP, omP, imm8bShuffle);

                            aIm = Avx2.Shuffle(partAs, partAs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partAs, partAs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapA);
                            tmpA = Fma.MultiplyAddSubtract(aRe, omA, aIm_bSwap);

                            aIm = Avx2.Shuffle(partBs, partBs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partBs, partBs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapB);
                            tmpB = Fma.MultiplyAddSubtract(aRe, omB, aIm_bSwap);

                            aIm = Avx2.Shuffle(partCs, partCs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partCs, partCs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapC);
                            tmpC = Fma.MultiplyAddSubtract(aRe, omC, aIm_bSwap);

                            aIm = Avx2.Shuffle(partDs, partDs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partDs, partDs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapD);
                            tmpD = Fma.MultiplyAddSubtract(aRe, omD, aIm_bSwap);

                            aIm = Avx2.Shuffle(partEs, partEs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partEs, partEs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapE);
                            tmpE = Fma.MultiplyAddSubtract(aRe, omE, aIm_bSwap);

                            aIm = Avx2.Shuffle(partFs, partFs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partFs, partFs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapF);
                            tmpF = Fma.MultiplyAddSubtract(aRe, omF, aIm_bSwap);

                            aIm = Avx2.Shuffle(partGs, partGs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partGs, partGs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapG);
                            tmpG = Fma.MultiplyAddSubtract(aRe, omG, aIm_bSwap);

                            aIm = Avx2.Shuffle(partHs, partHs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partHs, partHs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapH);
                            tmpH = Fma.MultiplyAddSubtract(aRe, omH, aIm_bSwap);

                            aIm = Avx2.Shuffle(partIs, partIs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partIs, partIs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapI);
                            tmpI = Fma.MultiplyAddSubtract(aRe, omI, aIm_bSwap);

                            aIm = Avx2.Shuffle(partJs, partJs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partJs, partJs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapJ);
                            tmpJ = Fma.MultiplyAddSubtract(aRe, omJ, aIm_bSwap);

                            aIm = Avx2.Shuffle(partKs, partKs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partKs, partKs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapK);
                            tmpK = Fma.MultiplyAddSubtract(aRe, omK, aIm_bSwap);

                            aIm = Avx2.Shuffle(partLs, partLs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partLs, partHs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapL);
                            tmpL = Fma.MultiplyAddSubtract(aRe, omL, aIm_bSwap);

                            aIm = Avx2.Shuffle(partMs, partMs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partMs, partMs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapM);
                            tmpM = Fma.MultiplyAddSubtract(aRe, omM, aIm_bSwap);

                            aIm = Avx2.Shuffle(partNs, partNs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partNs, partNs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapN);
                            tmpN = Fma.MultiplyAddSubtract(aRe, omN, aIm_bSwap);

                            aIm = Avx2.Shuffle(partOs, partOs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partOs, partOs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapO);
                            tmpO = Fma.MultiplyAddSubtract(aRe, omO, aIm_bSwap);

                            aIm = Avx2.Shuffle(partPs, partPs, imm8aImShuffle);
                            aRe = Avx2.Shuffle(partPs, partPs, imm8aReShuffle);
                            aIm_bSwap = Avx.Multiply(aIm, bSwapP);
                            tmpP = Fma.MultiplyAddSubtract(aRe, omP, aIm_bSwap);

                            Avx2.Store(tmpShift * addressShift + rPtr + 0  , Avx2.Add(partA, tmpA));
                            Avx2.Store(tmpShift * addressShift + rPtr + 8  , Avx2.Add(partB, tmpB));
                            Avx2.Store(tmpShift * addressShift + rPtr + 16 , Avx2.Add(partC, tmpC));
                            Avx2.Store(tmpShift * addressShift + rPtr + 24 , Avx2.Add(partD, tmpD));
                            Avx2.Store(tmpShift * addressShift + rPtr + 32 , Avx2.Add(partE, tmpE));
                            Avx2.Store(tmpShift * addressShift + rPtr + 40 , Avx2.Add(partF, tmpF));
                            Avx2.Store(tmpShift * addressShift + rPtr + 48 , Avx2.Add(partG, tmpG));
                            Avx2.Store(tmpShift * addressShift + rPtr + 56 , Avx2.Add(partH, tmpH));
                            Avx2.Store(tmpShift * addressShift + rPtr + 64 , Avx2.Add(partI, tmpI));
                            Avx2.Store(tmpShift * addressShift + rPtr + 72 , Avx2.Add(partJ, tmpJ));
                            Avx2.Store(tmpShift * addressShift + rPtr + 80 , Avx2.Add(partK, tmpK));
                            Avx2.Store(tmpShift * addressShift + rPtr + 88 , Avx2.Add(partL, tmpL));
                            Avx2.Store(tmpShift * addressShift + rPtr + 96 , Avx2.Add(partM, tmpM));
                            Avx2.Store(tmpShift * addressShift + rPtr + 104, Avx2.Add(partN, tmpN));
                            Avx2.Store(tmpShift * addressShift + rPtr + 112, Avx2.Add(partO, tmpO));
                            Avx2.Store(tmpShift * addressShift + rPtr + 120, Avx2.Add(partP, tmpP));
                                        
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 0  , Avx2.Subtract(partA, tmpA));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 8  , Avx2.Subtract(partB, tmpB));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 16 , Avx2.Subtract(partC, tmpC));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 24 , Avx2.Subtract(partD, tmpD));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 32 , Avx2.Subtract(partE, tmpE));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 40 , Avx2.Subtract(partF, tmpF));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 48 , Avx2.Subtract(partG, tmpG));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 56 , Avx2.Subtract(partH, tmpH));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 64 , Avx2.Subtract(partI, tmpI));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 72 , Avx2.Subtract(partJ, tmpJ));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 80 , Avx2.Subtract(partK, tmpK));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 88 , Avx2.Subtract(partL, tmpL));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 96 , Avx2.Subtract(partM, tmpM));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 104, Avx2.Subtract(partN, tmpN));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 112, Avx2.Subtract(partO, tmpO));
                            Avx2.Store((tmpShift + tmpJump) * addressShift + rPtr + 120, Avx2.Subtract(partP, tmpP));
                        }
                    }
                }
            }
            #endregion

            return result;
        }
    }
}
