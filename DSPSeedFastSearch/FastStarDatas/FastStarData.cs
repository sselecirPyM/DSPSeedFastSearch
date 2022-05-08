//#define USE_STAR_COLOR
//#define USE_STAR_CLASS_FACTOR
//#define USE_STAR_INDEX
using DSPSeedFastSearch.Algorithms;
using DSPSeedFastSearch.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPSeedFastSearch.FastStarDatas
{
    public class FastStarData
    {
        public FastStarData(int galaxySeed, int starCount)
        {
            Generate(galaxySeed, starCount);
        }
        public void Generate(int galaxySeed, int starCount)
        {
            this.seed = galaxySeed;
            stars = new Star[starCount];
            XURandom1 random = new XURandom1(galaxySeed, stackalloc int[56]);
            int seed1 = random.Next();

            int num6 = Mathf.CeilToInt(0.01f * (float)starCount + (float)random.NextDouble() * 0.3f);
            int num7 = Mathf.CeilToInt(0.01f * (float)starCount + (float)random.NextDouble() * 0.3f);
            int num8 = Mathf.CeilToInt(0.016f * (float)starCount + (float)random.NextDouble() * 0.4f);
            int num9 = Mathf.CeilToInt(0.013f * (float)starCount + (float)random.NextDouble() * 1.4f);
            int num10 = starCount - num6;
            int num11 = num10 - num7;
            int num12 = num11 - num8;
            int num13 = (num12 - 1) / num9;
            int num14 = num13 / 2;
            for (int i = 0; i < starCount; i++)
            {
                int seed2 = random.Next();
                if (i == 0)
                {
                    stars[i] = CreateBirthStar(seed2);
                }
                else
                {
                    ESpectrType needSpectr = ESpectrType.X;
                    if (i == 3)
                    {
                        needSpectr = ESpectrType.M;
                    }
                    else if (i == num12 - 1)
                    {
                        needSpectr = ESpectrType.O;
                    }
                    EStarType starType = EStarType.MainSeqStar;
                    if (i % num13 == num14)
                    {
                        starType = EStarType.GiantStar;
                    }
                    if (i >= num10)
                    {
                        starType = EStarType.BlackHole;
                    }
                    else if (i >= num11)
                    {
                        starType = EStarType.NeutronStar;
                    }
                    else if (i >= num12)
                    {
                        starType = EStarType.WhiteDwarf;
                    }
                    stars[i] = CreateStar(starCount, i + 1, seed2, starType, needSpectr);
                }
            }
        }

        private static Star CreateStar(int starCount, int id, int seed, EStarType starType, ESpectrType starSpectr)
        {
            Star starData = new Star();
#if USE_STAR_INDEX
            starData.index = (id - 1);
#endif
            starData.level = (float)(id - 1) / (float)(starCount - 1);


            starData.seed = seed;
            XURandom1 random = new XURandom1(seed, stackalloc int[56]);
            int seed2 = random.Next();
            int seed3 = random.Next();
            XURandom1 random2 = new XURandom1(seed3, stackalloc int[56]);
            double num3 = random2.NextDouble();
            double num4 = random2.NextDouble();
            double num5 = random2.NextDouble();
            double rn = random2.NextDouble();
            double rt = random2.NextDouble();
            double num6 = (random2.NextDouble() - 0.5) * 0.2;
            double num7 = random2.NextDouble() * 0.2 + 0.9;
            double num8 = random2.NextDouble() * 0.4 - 0.2;
            double num9 = Math.Pow(2.0, num8);
            float num10 = Mathf.Lerp(-0.98f, 0.88f, starData.level);
            if (num10 < 0f)
            {
                num10 -= 0.65f;
            }
            else
            {
                num10 += 0.65f;
            }
            float standardDeviation = 0.33f;
            if (starType == EStarType.GiantStar)
            {
                num10 = ((num8 <= -0.08) ? 1.6f : -1.5f);
                standardDeviation = 0.3f;
            }
            float num11 = RandNormal(num10, standardDeviation, num3, num4);
            if (starSpectr == ESpectrType.M)
            {
                num11 = -3f;
            }
            else if (starSpectr == ESpectrType.O)
            {
                num11 = 3f;
            }
            if (num11 > 0f)
            {
                num11 *= 2f;
            }
            else
            {
                num11 *= 1f;
            }
            num11 = Mathf.Clamp(num11, -2.4f, 4.65f) + (float)num6 + 1f;
            if (starType == EStarType.BlackHole)
            {
                starData.mass = 18f + (float)(num3 * num4) * 30f;
            }
            else if (starType == EStarType.NeutronStar)
            {
                starData.mass = 7f + (float)num3 * 11f;
            }
            else if (starType == EStarType.WhiteDwarf)
            {
                starData.mass = 1f + (float)num4 * 5f;
            }
            else
            {
                starData.mass = Mathf.Pow(2f, num11);
            }
            double d = 5.0;
            if (starData.mass < 2f)
            {
                d = 2.0 + 0.4 * (1.0 - (double)starData.mass);
            }
            starData.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)starData.mass * 0.5) / Math.Log10(d) + 1.0) * num7);
            if (starType == EStarType.GiantStar)
            {
                starData.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)starData.mass * 0.58) / Math.Log10(d) + 1.0) * num7);
                starData.age = (float)num5 * 0.04f + 0.96f;
            }
            else if (starType == EStarType.BlackHole || starType == EStarType.NeutronStar || starType == EStarType.WhiteDwarf)
            {
                starData.age = (float)num5 * 0.4f + 1f;
                if (starType == EStarType.WhiteDwarf)
                {
                    starData.lifetime += 10000f;
                }
                else if (starType == EStarType.NeutronStar)
                {
                    starData.lifetime += 1000f;
                }
            }
            else if ((double)starData.mass < 0.5)
            {
                starData.age = (float)num5 * 0.12f + 0.02f;
            }
            else if ((double)starData.mass < 0.8)
            {
                starData.age = (float)num5 * 0.4f + 0.1f;
            }
            else
            {
                starData.age = (float)num5 * 0.7f + 0.2f;
            }
            float num12 = starData.lifetime * starData.age;
            if (num12 > 5000f)
            {
                num12 = (Mathf.Log(num12 / 5000f) + 1f) * 5000f;
            }
            if (num12 > 8000f)
            {
                float num13 = num12 / 8000f;
                num13 = Mathf.Log(num13) + 1f;
                num13 = Mathf.Log(num13) + 1f;
                num13 = Mathf.Log(num13) + 1f;
                num12 = num13 * 8000f;
            }
            starData.lifetime = num12 / starData.age;
            float num14 = (1f - Mathf.Pow(Mathf.Clamp01(starData.age), 20f) * 0.5f) * starData.mass;
            starData.temperature = (float)(Math.Pow((double)num14, 0.56 + 0.14 / (Math.Log10((double)(num14 + 4f)) / Math.Log10(5.0))) * 4450.0 + 1300.0);
            double num15 = Math.Log10(((double)starData.temperature - 1300.0) / 4500.0) / Math.Log10(2.6) - 0.5;
            if (num15 < 0.0)
            {
                num15 *= 4.0;
            }
            if (num15 > 2.0)
            {
                num15 = 2.0;
            }
            else if (num15 < -4.0)
            {
                num15 = -4.0;
            }
            starData.spectr = (ESpectrType)Mathf.RoundToInt((float)num15 + 4f);
#if USE_STAR_COLOR
            starData.color = Mathf.Clamp01(((float)num15 + 3.5f) * 0.2f);
#endif
#if USE_STAR_CLASS_FACTOR
            starData.classFactor = (float)num15;
#endif
            starData.luminosity = Mathf.Pow(num14, 0.7f);
            starData.radius = (float)(Math.Pow((double)starData.mass, 0.4) * num9);
            float p = (float)num15 + 2f;
            starData.orbitScaler = Mathf.Pow(1.35f, p);
            if (starData.orbitScaler < 1f)
            {
                starData.orbitScaler = Mathf.Lerp(starData.orbitScaler, 1f, 0.6f);
            }
            SetStarAge(ref starData, starData.age, rn, rt);
            starData.dysonRadius = starData.orbitScaler * 0.28f;
            if ((double)starData.dysonRadius * 40000.0 < (double)(starData.physicsRadius * 1.5f))
            {
                starData.dysonRadius = (float)((double)(starData.physicsRadius * 1.5f) / 40000.0);
            }

            return starData;
        }

        private static Star CreateBirthStar(int seed)
        {
            Star star = new Star();
#if USE_STAR_INDEX
            star.index = 0;
#endif
            star.level = 0f;
            star.seed = seed;
            XURandom1 random = new XURandom1(seed, stackalloc int[56]);
            int seed2 = random.Next();
            int seed3 = random.Next();

            XURandom1 random2 = new XURandom1(seed3, stackalloc int[56]);
            double r = random2.NextDouble();
            double r2 = random2.NextDouble();
            double num = random2.NextDouble();
            double rn = random2.NextDouble();
            double rt = random2.NextDouble();
            double num2 = random2.NextDouble() * 0.2 + 0.9;
            double y = random2.NextDouble() * 0.4 - 0.2;
            double num3 = Math.Pow(2.0, y);
            float num4 = RandNormal(0f, 0.08f, r, r2);
            num4 = Mathf.Clamp(num4, -0.2f, 0.2f);
            star.mass = Mathf.Pow(2f, num4);
            double d = 2.0 + 0.4 * (1.0 - (double)star.mass);
            star.lifetime = (float)(10000.0 * Math.Pow(0.1, Math.Log10((double)star.mass * 0.5) / Math.Log10(d) + 1.0) * num2);
            star.age = (float)(num * 0.4 + 0.3);
            float num5 = (1f - Mathf.Pow(Mathf.Clamp01(star.age), 20f) * 0.5f) * star.mass;
            star.temperature = (float)(Math.Pow((double)num5, 0.56 + 0.14 / (Math.Log10((double)(num5 + 4f)) / Math.Log10(5.0))) * 4450.0 + 1300.0);
            double num6 = Math.Log10(((double)star.temperature - 1300.0) / 4500.0) / Math.Log10(2.6) - 0.5;
            if (num6 < 0.0)
            {
                num6 *= 4.0;
            }
            if (num6 > 2.0)
            {
                num6 = 2.0;
            }
            else if (num6 < -4.0)
            {
                num6 = -4.0;
            }
            star.spectr = (ESpectrType)Mathf.RoundToInt((float)num6 + 4f);
#if USE_STAR_COLOR
            star.color = Mathf.Clamp01(((float)num6 + 3.5f) * 0.2f);
#endif
#if USE_STAR_CLASS_FACTOR
            star.classFactor = (float)num6;
#endif
            star.luminosity = Mathf.Pow(num5, 0.7f);
            star.radius = (float)(Math.Pow((double)star.mass, 0.4) * num3);
            float p = (float)num6 + 2f;
            star.orbitScaler = Mathf.Pow(1.35f, p);
            if (star.orbitScaler < 1f)
            {
                star.orbitScaler = Mathf.Lerp(star.orbitScaler, 1f, 0.6f);
            }
            SetStarAge(ref star, star.age, rn, rt);
            star.dysonRadius = star.orbitScaler * 0.28f;
            if ((double)star.dysonRadius * 40000.0 < (double)(star.physicsRadius * 1.5f))
            {
                star.dysonRadius = (float)((double)(star.physicsRadius * 1.5f) / 40000.0);
            }
            return star;
        }

        public static void SetStarAge(ref Star star, float age, double rn, double rt)
        {
            float num = (float)(rn * 0.1 + 0.95);
            float num2 = (float)(rt * 0.4 + 0.8);
            float num3 = (float)(rt * 9.0 + 1.0);
            star.age = age;
            if (age >= 1f)
            {
                if (star.mass >= 18f)
                {
                    star.type = EStarType.BlackHole;
                    star.spectr = ESpectrType.X;
                    star.mass *= 2.5f * num2;
                    star.radius *= 1f;
                    star.temperature = 0f;
                    star.luminosity *= 0.001f * num;
                }
                else if (star.mass >= 7f)
                {
                    star.type = EStarType.NeutronStar;
                    star.spectr = ESpectrType.X;
                    star.mass *= 0.2f * num;
                    star.radius *= 0.15f;
                    star.temperature = num3 * 1E+07f;
                    star.luminosity *= 0.1f * num;
                    star.orbitScaler *= 1.5f * num;
                }
                else
                {
                    star.type = EStarType.WhiteDwarf;
                    star.spectr = ESpectrType.X;
                    star.mass *= 0.2f * num;
                    star.radius *= 0.2f;
                    star.temperature = num2 * 150000f;
                    star.luminosity *= 0.04f * num2;
                }
            }
            else if (age >= 0.96f)
            {
                float num4 = (float)(Math.Pow(5.0, Math.Abs(Math.Log10((double)star.mass) - 0.7)) * 5.0);
                if (num4 > 10f)
                {
                    num4 = (Mathf.Log(num4 * 0.1f) + 1f) * 10f;
                }
                float num5 = 1f - Mathf.Pow(star.age, 30f) * 0.5f;
                star.type = EStarType.GiantStar;
                star.mass = num5 * star.mass;
                star.radius = num4 * num2;
                star.temperature = num5 * star.temperature;
                star.luminosity = 1.6f * star.luminosity;
                star.orbitScaler = 3.3f * star.orbitScaler;
            }
        }

        private static float RandNormal(float average, float standardDeviation, double r1, double r2)
        {
            return average + standardDeviation * (float)(Math.Sqrt(-2.0 * Math.Log(1.0 - r1)) * Math.Sin(6.2831853071795862 * r2));
        }

        public int seed;
        public Star[] stars;
    }

    public class FastStarDataSuitForGPU
    {
        public FastStarDataSuitForGPU(int galaxySeed, int starCount)
        {
            Generate(galaxySeed, starCount);
        }
        public void Generate(int galaxySeed, int starCount)
        {
            this.seed = galaxySeed;
            stars = new Star[starCount];
            XURandom2 random = new XURandom2(galaxySeed, stackalloc int[56]);
            int seed1 = random.Next();

            int num6 = Mathf.CeilToInt(0.01f * starCount + random.NextFloat() * 0.3f);
            int num7 = Mathf.CeilToInt(0.01f * starCount + random.NextFloat() * 0.3f);
            int num8 = Mathf.CeilToInt(0.016f * starCount + random.NextFloat() * 0.4f);
            int num9 = Mathf.CeilToInt(0.013f * starCount + random.NextFloat() * 1.4f);
            int num10 = starCount - num6;
            int num11 = num10 - num7;
            int num12 = num11 - num8;
            int num13 = (num12 - 1) / num9;
            int num14 = num13 / 2;
            Span<int> tempMemory1 = stackalloc int[56];
            for (int i = 0; i < starCount; i++)
            {
                int seed2 = random.Next();
                XURandom2 randomX = new XURandom2(seed2, tempMemory1);
                int notuse1 = randomX.Next();
                int seed3 = randomX.Next();
                if (i == 0)
                {
                    stars[i] = CreateBirthStar(seed2, seed3);
                }
                else
                {
                    ESpectrType needSpectr = ESpectrType.X;
                    if (i == 3)
                    {
                        needSpectr = ESpectrType.M;
                    }
                    else if (i == num12 - 1)
                    {
                        needSpectr = ESpectrType.O;
                    }
                    EStarType starType = EStarType.MainSeqStar;
                    if (i % num13 == num14)
                    {
                        starType = EStarType.GiantStar;
                    }
                    if (i >= num10)
                    {
                        starType = EStarType.BlackHole;
                    }
                    else if (i >= num11)
                    {
                        starType = EStarType.NeutronStar;
                    }
                    else if (i >= num12)
                    {
                        starType = EStarType.WhiteDwarf;
                    }
                    stars[i] = CreateStar(starCount, i + 1, seed2, starType, needSpectr, seed3);
                }
            }
        }

        private static Star CreateStar(int starCount, int id, int seed, EStarType starType, ESpectrType starSpectr, int seed3)
        {
            Star starData = new Star();
#if USE_STAR_INDEX
            starData.index = (id - 1);
#endif
            starData.level = (id - 1) / (starCount - 1);


            starData.seed = seed;
            XURandom2 random2 = new XURandom2(seed3, stackalloc int[56]);
            float num3 = random2.NextFloat();
            float num4 = random2.NextFloat();
            float num5 = random2.NextFloat();
            float rn = random2.NextFloat();
            float rt = random2.NextFloat();
            float num6 = (random2.NextFloat() - 0.5f) * 0.2f;
            float num7 = random2.NextFloat() * 0.2f + 0.9f;
            float num8 = random2.NextFloat() * 0.4f - 0.2f;
            float num9 = MathF.Pow(2.0f, num8);
            float num10 = Mathf.Lerp(-0.98f, 0.88f, starData.level);
            if (num10 < 0f)
            {
                num10 -= 0.65f;
            }
            else
            {
                num10 += 0.65f;
            }
            float standardDeviation = 0.33f;
            if (starType == EStarType.GiantStar)
            {
                num10 = ((num8 <= -0.08) ? 1.6f : -1.5f);
                standardDeviation = 0.3f;
            }
            float num11 = RandNormal(num10, standardDeviation, num3, num4);
            if (starSpectr == ESpectrType.M)
            {
                num11 = -3f;
            }
            else if (starSpectr == ESpectrType.O)
            {
                num11 = 3f;
            }
            if (num11 > 0f)
            {
                num11 *= 2f;
            }
            else
            {
                num11 *= 1f;
            }
            num11 = Mathf.Clamp(num11, -2.4f, 4.65f) + num6 + 1f;
            if (starType == EStarType.BlackHole)
            {
                starData.mass = 18f + (num3 * num4) * 30f;
            }
            else if (starType == EStarType.NeutronStar)
            {
                starData.mass = 7f + num3 * 11f;
            }
            else if (starType == EStarType.WhiteDwarf)
            {
                starData.mass = 1f + num4 * 5f;
            }
            else
            {
                starData.mass = MathF.Pow(2f, num11);
            }
            float d = 5.0f;
            if (starData.mass < 2f)
            {
                d = 2.0f + 0.4f * (1.0f - starData.mass);
            }
            starData.lifetime = (10000.0f * MathF.Pow(0.1f, MathF.Log10(starData.mass * 0.5f) / MathF.Log10(d) + 1.0f) * num7);
            if (starType == EStarType.GiantStar)
            {
                starData.lifetime = (10000.0f * MathF.Pow(0.1f, MathF.Log10(starData.mass * 0.58f) / MathF.Log10(d) + 1.0f) * num7);
                starData.age = num5 * 0.04f + 0.96f;
            }
            else if (starType == EStarType.BlackHole || starType == EStarType.NeutronStar || starType == EStarType.WhiteDwarf)
            {
                starData.age = num5 * 0.4f + 1f;
                if (starType == EStarType.WhiteDwarf)
                {
                    starData.lifetime += 10000f;
                }
                else if (starType == EStarType.NeutronStar)
                {
                    starData.lifetime += 1000f;
                }
            }
            else if (starData.mass < 0.5)
            {
                starData.age = num5 * 0.12f + 0.02f;
            }
            else if (starData.mass < 0.8)
            {
                starData.age = num5 * 0.4f + 0.1f;
            }
            else
            {
                starData.age = num5 * 0.7f + 0.2f;
            }
            float num12 = starData.lifetime * starData.age;
            if (num12 > 5000f)
            {
                num12 = (MathF.Log(num12 / 5000f) + 1f) * 5000f;
            }
            if (num12 > 8000f)
            {
                float num13 = num12 / 8000f;
                num13 = MathF.Log(num13) + 1f;
                num13 = MathF.Log(num13) + 1f;
                num13 = MathF.Log(num13) + 1f;
                num12 = num13 * 8000f;
            }
            starData.lifetime = num12 / starData.age;
            float num14 = (1f - MathF.Pow(Mathf.Clamp01(starData.age), 20f) * 0.5f) * starData.mass;
            starData.temperature = MathF.Pow(num14, 0.56f + 0.14f / (MathF.Log10((num14 + 4f)) / MathF.Log10(5.0f))) * 4450.0f + 1300.0f;
            float num15 = MathF.Log10((starData.temperature - 1300.0f) / 4500.0f) / MathF.Log10(2.6f) - 0.5f;
            if (num15 < 0.0)
            {
                num15 *= 4.0f;
            }
            if (num15 > 2.0)
            {
                num15 = 2.0f;
            }
            else if (num15 < -4.0)
            {
                num15 = -4.0f;
            }
            starData.spectr = (ESpectrType)Mathf.RoundToInt(num15 + 4f);
#if USE_STAR_COLOR
            starData.color = Mathf.Clamp01(((float)num15 + 3.5f) * 0.2f);
#endif
#if USE_STAR_CLASS_FACTOR
            starData.classFactor = (float)num15;
#endif
            starData.luminosity = MathF.Pow(num14, 0.7f);
            starData.radius = (MathF.Pow(starData.mass, 0.4f) * num9);
            float p = num15 + 2f;
            starData.orbitScaler = MathF.Pow(1.35f, p);
            if (starData.orbitScaler < 1f)
            {
                starData.orbitScaler = Mathf.Lerp(starData.orbitScaler, 1f, 0.6f);
            }
            SetStarAge(ref starData, starData.age, rn, rt);
            starData.dysonRadius = starData.orbitScaler * 0.28f;
            if (starData.dysonRadius * 40000.0f < (starData.physicsRadius * 1.5f))
            {
                starData.dysonRadius = (starData.physicsRadius * 1.5f) / 40000.0f;
            }

            return starData;
        }

        private static Star CreateBirthStar(int seed, int seed3)
        {
            Star star = new Star();
#if USE_STAR_INDEX
            star.index = 0;
#endif
            star.level = 0f;
            star.seed = seed;

            XURandom2 random2 = new XURandom2(seed3, stackalloc int[56]);
            float r = random2.NextFloat();
            float r2 = random2.NextFloat();
            float num = random2.NextFloat();
            float rn = random2.NextFloat();
            float rt = random2.NextFloat();
            float num2 = random2.NextFloat() * 0.2f + 0.9f;
            float y = random2.NextFloat() * 0.4f - 0.2f;
            float num3 = MathF.Pow(2.0f, y);
            float num4 = RandNormal(0f, 0.08f, r, r2);
            num4 = Mathf.Clamp(num4, -0.2f, 0.2f);
            star.mass = MathF.Pow(2f, num4);
            float d = 2.0f + 0.4f * (1.0f - star.mass);
            star.lifetime = (10000.0f * MathF.Pow(0.1f, MathF.Log10(star.mass * 0.5f) / MathF.Log10(d) + 1.0f) * num2);
            star.age = (num * 0.4f + 0.3f);
            float num5 = (1f - MathF.Pow(Mathf.Clamp01(star.age), 20f) * 0.5f) * star.mass;
            star.temperature = (MathF.Pow(num5, 0.56f + 0.14f / (MathF.Log10((num5 + 4f)) / MathF.Log10(5.0f))) * 4450.0f + 1300.0f);
            float num6 = MathF.Log10((star.temperature - 1300.0f) / 4500.0f) / MathF.Log10(2.6f) - 0.5f;
            if (num6 < 0.0f)
            {
                num6 *= 4.0f;
            }
            if (num6 > 2.0f)
            {
                num6 = 2.0f;
            }
            else if (num6 < -4.0f)
            {
                num6 = -4.0f;
            }
            star.spectr = (ESpectrType)Mathf.RoundToInt(num6 + 4f);
#if USE_STAR_COLOR
            star.color = Mathf.Clamp01(((float)num6 + 3.5f) * 0.2f);
#endif
#if USE_STAR_CLASS_FACTOR
            star.classFactor = (float)num6;
#endif
            star.luminosity = MathF.Pow(num5, 0.7f);
            star.radius = (MathF.Pow(star.mass, 0.4f) * num3);
            float p = num6 + 2f;
            star.orbitScaler = MathF.Pow(1.35f, p);
            if (star.orbitScaler < 1f)
            {
                star.orbitScaler = Mathf.Lerp(star.orbitScaler, 1f, 0.6f);
            }
            SetStarAge(ref star, star.age, rn, rt);
            star.dysonRadius = star.orbitScaler * 0.28f;
            if (star.dysonRadius * 40000.0f < (star.physicsRadius * 1.5f))
            {
                star.dysonRadius = ((star.physicsRadius * 1.5f) / 40000.0f);
            }
            return star;
        }

        public static void SetStarAge(ref Star star, float age, float rn, float rt)
        {
            float num = (rn * 0.1f + 0.95f);
            float num2 = (rt * 0.4f + 0.8f);
            float num3 = (rt * 9.0f + 1.0f);
            star.age = age;
            if (age >= 1f)
            {
                if (star.mass >= 18f)
                {
                    star.type = EStarType.BlackHole;
                    star.spectr = ESpectrType.X;
                    star.mass *= 2.5f * num2;
                    star.radius *= 1f;
                    star.temperature = 0f;
                    star.luminosity *= 0.001f * num;
                }
                else if (star.mass >= 7f)
                {
                    star.type = EStarType.NeutronStar;
                    star.spectr = ESpectrType.X;
                    star.mass *= 0.2f * num;
                    star.radius *= 0.15f;
                    star.temperature = num3 * 1E+07f;
                    star.luminosity *= 0.1f * num;
                    star.orbitScaler *= 1.5f * num;
                }
                else
                {
                    star.type = EStarType.WhiteDwarf;
                    star.spectr = ESpectrType.X;
                    star.mass *= 0.2f * num;
                    star.radius *= 0.2f;
                    star.temperature = num2 * 150000f;
                    star.luminosity *= 0.04f * num2;
                }
            }
            else if (age >= 0.96f)
            {
                float num4 = MathF.Pow(5.0f, MathF.Abs(MathF.Log10(star.mass) - 0.7f)) * 5.0f;
                if (num4 > 10f)
                {
                    num4 = (MathF.Log(num4 * 0.1f) + 1f) * 10f;
                }
                float num5 = 1f - MathF.Pow(star.age, 30f) * 0.5f;
                star.type = EStarType.GiantStar;
                star.mass = num5 * star.mass;
                star.radius = num4 * num2;
                star.temperature = num5 * star.temperature;
                star.luminosity = 1.6f * star.luminosity;
                star.orbitScaler = 3.3f * star.orbitScaler;
            }
        }

        private static float RandNormal(float average, float standardDeviation, float r1, float r2)
        {
            return average + standardDeviation * (MathF.Sqrt(-2.0f * MathF.Log(1.0f - r1)) * MathF.Sin(6.2831853071795862f * r2));
        }

        public int seed;
        public Star[] stars;
    }

    public struct Star//将这里struct改为class会导致多核模式下额外30%的时间消耗
    {
        public float dysonLumino
        {
            get
            {
                return Mathf.Round((float)Math.Pow((double)this.luminosity, 0.33000001311302185) * 1000f) / 1000f;
            }
        }

        public float physicsRadius
        {
            get
            {
                return this.radius * 1200f;
            }
        }

        public float viewRadius
        {
            get
            {
                return this.radius * 800f;
            }
        }

        public string typeString
        {
            get
            {
                if (this.type == EStarType.GiantStar)
                {
                    if (this.spectr <= ESpectrType.K)
                    {
                        return "红巨星";
                    }
                    else if (this.spectr <= ESpectrType.F)
                    {
                        return "黄巨星";
                    }
                    else if (this.spectr == ESpectrType.A)
                    {
                        return "白巨星";
                    }
                    else
                    {
                        return "蓝巨星";
                    }
                }
                else if (this.type == EStarType.WhiteDwarf)
                {
                    return "白矮星";
                }
                else if (this.type == EStarType.NeutronStar)
                {
                    return "中子星";
                }
                else if (this.type == EStarType.BlackHole)
                {
                    return "黑洞";
                }
                else if (this.type == EStarType.MainSeqStar)
                {
                    return this.spectr.ToString() + "型恒星";
                }
                return "";
            }
        }

        public float dysonRadius;

        public float level;

        public int seed;
#if USE_STAR_INDEX
        public int index;
#endif
        public float mass;

        public float lifetime;

        public float age;

        public float luminosity;

        public float radius;

        public EStarType type;

        public float temperature;

        public ESpectrType spectr;

        public float orbitScaler;
#if USE_STAR_CLASS_FACTOR
        public float classFactor;
#endif
#if USE_STAR_COLOR
        public float color;
#endif
    }
}
