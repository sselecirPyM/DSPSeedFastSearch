struct URandom
{
	int inext;

	int inextp;

	int SeedArray[56];
};

void InitRandom(inout URandom random, int Seed)
{
	int num = 161803398 - abs(Seed);
	random.SeedArray[0] = 0;
	random.SeedArray[55] = num;
	int num2 = 1;
	for (int i = 1; i < 55; i++)
	{
		int num3 = 21 * i % 55;
		random.SeedArray[num3] = num2;
		num2 = num - num2;
		if (num2 < 0)
		{
			num2 += 2147483647;//int.MaxValue
		}
		num = random.SeedArray[num3];
	}
	for (int j = 1; j < 5; j++)
	{
		for (int k = 1; k < 56; k++)
		{
			random.SeedArray[k] -= random.SeedArray[1 + (k + 30) % 55];
			if (random.SeedArray[k] < 0)
			{
				random.SeedArray[k] += 2147483647;//int.MaxValue
			}
		}
	}
	random.inext = 0;
	random.inextp = 31;
}
double RandomSample(inout URandom random)
{
	if (++random.inext >= 56)
	{
		random.inext = 1;
	}
	if (++random.inextp >= 56)
	{
		random.inextp = 1;
	}
	int num = random.SeedArray[random.inext] - random.SeedArray[random.inextp];
	if (num < 0)
	{
		num += 2147483647;//int.MaxValue
	}
	random.SeedArray[random.inext] = num;
	return (double)num * 4.6566128752457969E-10;
}
int RandomNext(inout URandom random)
{
	return (int)(RandomSample(random) * 2147483647.0);
}
double RandomNextDouble(inout URandom random)
{
	return RandomSample(random);
}

RWStructuredBuffer<int> Result : register(u0);
cbuffer cb0 : register(b0)
{
	int _startIndex;
	int _saveStartIndex;
}
[numthreads(16, 1, 1)]
void main(uint3 id : SV_DispatchThreadID)
{
	int index = id.x + _startIndex;
	int saveIndex = id.x + _saveStartIndex;
	URandom random;
	InitRandom(random, index);
	for (int j = 0; j < 5; j++)
	{
		RandomNext(random);
	}
	Result[saveIndex] = RandomNext(random);
	//Result[index] = 123456;
	return;
}