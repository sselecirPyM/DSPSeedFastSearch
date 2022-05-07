using System;
using System.Collections.Generic;
using System.Text;

namespace DSPSeedFastSearch.Algorithms
{
    public ref struct XURandom2
    {
        public XURandom2(int Seed, Span<int> tempMemory)
        {
            tempMemory.Clear();
            SeedArray = tempMemory;
            int num = 161803398 - Math.Abs(Seed);
            this.SeedArray[55] = num;
            int num2 = 1;
            for (int i = 1; i < 55; i++)
            {
                int num3 = 21 * i % 55;
                this.SeedArray[num3] = num2;
                num2 = num - num2;
                if (num2 < 0)
                {
                    num2 += int.MaxValue;
                }
                num = this.SeedArray[num3];
            }
            for (int j = 1; j < 5; j++)
            {
                for (int k = 1; k < 56; k++)
                {
                    this.SeedArray[k] -= this.SeedArray[1 + (k + 30) % 55];
                    if (this.SeedArray[k] < 0)
                    {
                        this.SeedArray[k] += int.MaxValue;
                    }
                }
            }
            this.inext = 0;
            this.inextp = 31;
        }

        private double Sample()
        {
            if (++this.inext >= 56)
            {
                this.inext = 1;
            }
            if (++this.inextp >= 56)
            {
                this.inextp = 1;
            }
            int num = this.SeedArray[this.inext] - this.SeedArray[this.inextp];
            if (num < 0)
            {
                num += int.MaxValue;
            }
            this.SeedArray[this.inext] = num;
            return (double)num * 4.6566128752457969E-10;
        }

        private float SampleFloat()
        {
            if (++this.inext >= 56)
            {
                this.inext = 1;
            }
            if (++this.inextp >= 56)
            {
                this.inextp = 1;
            }
            int num = this.SeedArray[this.inext] - this.SeedArray[this.inextp];
            if (num < 0)
            {
                num += int.MaxValue;
            }
            this.SeedArray[this.inext] = num;
            return num * 4.6566128752457969E-10f;
        }

        public int Next()
        {
            if (++inext >= 56)
            {
                inext = 1;
            }
            if (++inextp >= 56)
            {
                inextp = 1;
            }
            int num = SeedArray[inext] - SeedArray[inextp];
            if (num < 0)
            {
                num += 2147483647;//int.MaxValue
            }
            SeedArray[inext] = num;

            if (num >= 1073742080)
            {
                if ((num - 1073742080) % 1024 == 0)
                    return num - 1;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    int d = 536871040 >> i;
                    int e = 1073741824 >> i;
                    if (num >= d)
                    {
                        if (num < e && (num - d) % (512 >> i) == 0)
                            return num - 1;
                        break;
                    }
                }
            }

            return num;
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("Max value is less than min value.");
            }
            return (int)(this.Sample() * (float)maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("Min value is greater than max value.");
            }
            uint num = (uint)(maxValue - minValue);
            if (num <= 1U)
            {
                return minValue;
            }
            return (int)((ulong)((uint)(this.Sample() * num)) + (ulong)((long)minValue));
        }

        public void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(this.Sample() * 256.0);
            }
        }

        public double NextDouble()
        {
            return this.Sample();
        }

        public float NextFloat()
        {
            return this.SampleFloat();
        }

        private const int MBIG = 2147483647;

        private const int MSEED = 161803398;

        private const int MZ = 0;

        private int inext;

        private int inextp;

        private Span<int> SeedArray;
    }
}
