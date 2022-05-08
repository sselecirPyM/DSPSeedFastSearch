#include "Random.hlsli"
RWStructuredBuffer<float> Result : register(u0);
cbuffer cb0 : register(b0)
{
	int _startIndex;
	int _saveStartIndex;
}

typedef int ESpectrType;
typedef int EStarType;

const static int EStarType_MainSeqStar = 0;
const static int EStarType_GiantStar = 1;
const static int EStarType_WhiteDwarf = 2;
const static int EStarType_NeutronStar = 3;
const static int EStarType_BlackHole = 4;


const static int ESpectrType_M = 0;
const static int ESpectrType_K = 1;
const static int ESpectrType_G = 2;
const static int ESpectrType_F = 3;
const static int ESpectrType_A = 4;
const static int ESpectrType_B = 5;
const static int ESpectrType_O = 6;
const static int ESpectrType_X = 7;

struct Star
{
	float dysonRadius;

	float level;

	int seed;

	float mass;

	float lifetime;

	float age;

	float luminosity;

	float radius;

	EStarType type;

	float temperature;

	ESpectrType spectr;

	float orbitScaler;
};

float RandNormal(float average, float standardDeviation, float r1, float r2)
{
	return average + standardDeviation * (sqrt(-2.0f * log(1.0f - r1)) * sin(6.2831853071795862f * r2));
}

void SetStarAge(inout Star star, float age, float rn, float rt)
{
	float num = (rn * 0.1f + 0.95f);
	float num2 = (rt * 0.4f + 0.8f);
	float num3 = (rt * 9.0f + 1.0f);
	star.age = age;
	if (age >= 1)
	{
		if (star.mass >= 18)
		{
			star.type = EStarType_BlackHole;
			star.spectr = ESpectrType_X;
			star.mass *= 2.5f * num2;
			star.radius *= 1.0f;
			star.temperature = 0.0f;
			star.luminosity *= 0.001f * num;
		}
		else if (star.mass >= 7)
		{
			star.type = EStarType_NeutronStar;
			star.spectr = ESpectrType_X;
			star.mass *= 0.2f * num;
			star.radius *= 0.15f;
			star.temperature = num3 * 1E+07f;
			star.luminosity *= 0.1f * num;
			star.orbitScaler *= 1.5f * num;
		}
		else
		{
			star.type = EStarType_WhiteDwarf;
			star.spectr = ESpectrType_X;
			star.mass *= 0.2f * num;
			star.radius *= 0.2f;
			star.temperature = num2 * 150000.0f;
			star.luminosity *= 0.04f * num2;
		}
	}
	else if (age >= 0.96f)
	{
		float num4 = pow(5.0f, abs(log10(star.mass) - 0.7f)) * 5.0f;
		if (num4 > 10)
		{
			num4 = (log(num4 * 0.1f) + 1) * 10.0f;
		}
		float num5 = 1.0f - pow(star.age, 30) * 0.5f;
		star.type = EStarType_GiantStar;
		star.mass = num5 * star.mass;
		star.radius = num4 * num2;
		star.temperature = num5 * star.temperature;
		star.luminosity = 1.6f * star.luminosity;
		star.orbitScaler = 3.3f * star.orbitScaler;
	}
}

Star CreateStar(int starCount, int id, int seed, EStarType starType, ESpectrType starSpectr, int seed3)
{
	Star starData;

	starData.level = (id - 1) / (starCount - 1);

	starData.seed = seed;
	URandom random2;
	InitRandom(random2, seed3);

	float num3 = RandomNextFloat(random2);
	float num4 = RandomNextFloat(random2);
	float num5 = RandomNextFloat(random2);
	float rn = RandomNextFloat(random2);
	float rt = RandomNextFloat(random2);
	float num6 = (RandomNextFloat(random2) - 0.5f) * 0.2f;
	float num7 = RandomNextFloat(random2) * 0.2f + 0.9f;
	float num8 = RandomNextFloat(random2) * 0.4f - 0.2f;
	float num9 = pow(2.0f, num8);
	float num10 = lerp(-0.98f, 0.88f, starData.level);
	if (num10 < 0)
	{
		num10 -= 0.65f;
	}
	else
	{
		num10 += 0.65f;
	}
	float standardDeviation = 0.33f;
	if (starType == EStarType_GiantStar)
	{
		num10 = ((num8 <= -0.08) ? 1.6f : -1.5f);
		standardDeviation = 0.3f;
	}
	float num11 = RandNormal(num10, standardDeviation, num3, num4);
	if (starSpectr == ESpectrType_M)
	{
		num11 = -3;
	}
	else if (starSpectr == ESpectrType_O)
	{
		num11 = 3;
	}
	if (num11 > 0.0f)
	{
		num11 *= 2;
	}
	else
	{
		num11 *= 1;
	}
	num11 = clamp(num11, -2.4f, 4.65f) + num6 + 1;
	if (starType == EStarType_BlackHole)
	{
		starData.mass = 18 + (num3 * num4) * 30.0f;
	}
	else if (starType == EStarType_NeutronStar)
	{
		starData.mass = 7 + num3 * 11;
	}
	else if (starType == EStarType_WhiteDwarf)
	{
		starData.mass = 1 + num4 * 5;
	}
	else
	{
		starData.mass = pow(2, num11);
	}
	float d = 5.0f;
	if (starData.mass < 2)
	{
		d = 2.0f + 0.4f * (1.0f - starData.mass);
	}
	starData.lifetime = (10000.0f * pow(0.1f, log10(starData.mass * 0.5f) / log10(d) + 1.0f) * num7);
	if (starType == EStarType_GiantStar)
	{
		starData.lifetime = (10000.0f * pow(0.1f, log10(starData.mass * 0.58f) / log10(d) + 1.0f) * num7);
		starData.age = num5 * 0.04f + 0.96f;
	}
	else if (starType == EStarType_BlackHole || starType == EStarType_NeutronStar || starType == EStarType_WhiteDwarf)
	{
		starData.age = num5 * 0.4f + 1.0f;
		if (starType == EStarType_WhiteDwarf)
		{
			starData.lifetime += 10000.0f;
		}
		else if (starType == EStarType_NeutronStar)
		{
			starData.lifetime += 1000.0f;
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
	if (num12 > 5000.0f)
	{
		num12 = (log(num12 / 5000.0f) + 1) * 5000.0f;
	}
	if (num12 > 8000.0f)
	{
		float num13 = num12 / 8000.0f;
		num13 = log(num13) + 1.0f;
		num13 = log(num13) + 1.0f;
		num13 = log(num13) + 1.0f;
		num12 = num13 * 8000.0f;
	}
	starData.lifetime = num12 / starData.age;
	float num14 = (1.0f - pow(saturate(starData.age), 20.0f) * 0.5f) * starData.mass;
	starData.temperature = pow(num14, 0.56f + 0.14f / (log10((num14 + 4.0f)) / log10(5.0f))) * 4450.0f + 1300.0f;
	float num15 = log10((starData.temperature - 1300.0f) / 4500.0f) / log10(2.6f) - 0.5f;
	if (num15 < 0.0f)
	{
		num15 *= 4.0f;
	}
	if (num15 > 2.0)
	{
		num15 = 2.0f;
	}
	else if (num15 < -4.0f)
	{
		num15 = -4.0f;
	}
	starData.spectr = (ESpectrType)round(num15 + 4.0f);

	starData.luminosity = pow(num14, 0.7f);
	starData.radius = (pow(starData.mass, 0.4f) * num9);
	float p = num15 + 2.0f;
	starData.orbitScaler = pow(1.35f, p);
	if (starData.orbitScaler < 1.0f)
	{
		starData.orbitScaler = lerp(starData.orbitScaler, 1, 0.6f);
	}
	SetStarAge(starData, starData.age, rn, rt);
	starData.dysonRadius = starData.orbitScaler * 0.28f;
	if (starData.dysonRadius * 40000.0f < (starData.radius * 1200.0f * 1.5f))
	{
		starData.dysonRadius = (starData.radius * 1200.0f * 1.5f) / 40000.0f;
	}

	return starData;
}
Star CreateBirthStar(int seed, int seed3)
{
	Star star;
#if USE_STAR_INDEX
	star.index = 0;
#endif
	star.level = 0.0f;
	star.seed = seed;

	URandom random2;
	InitRandom(random2, seed3);

	float r = RandomNextFloat(random2);
	float r2 = RandomNextFloat(random2);
	float num = RandomNextFloat(random2);
	float rn = RandomNextFloat(random2);
	float rt = RandomNextFloat(random2);
	float num2 = RandomNextFloat(random2) * 0.2f + 0.9f;
	float y = RandomNextFloat(random2) * 0.4f - 0.2f;
	float num3 = pow(2.0f, y);
	float num4 = RandNormal(0.0f, 0.08f, r, r2);
	num4 = clamp(num4, -0.2f, 0.2f);
	star.mass = pow(2.0f, num4);
	float d = 2.0f + 0.4f * (1.0f - star.mass);
	star.lifetime = (10000.0f * pow(0.1f, log10(star.mass * 0.5f) / log10(d) + 1.0f) * num2);
	star.age = (num * 0.4f + 0.3f);
	float num5 = (1.0f - pow(saturate(star.age), 20.0f) * 0.5f) * star.mass;
	star.temperature = (pow(num5, 0.56f + 0.14f / (log10((num5 + 4.0f)) / log10(5.0f))) * 4450.0f + 1300.0f);
	float num6 = log10((star.temperature - 1300.0f) / 4500.0f) / log10(2.6f) - 0.5f;
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
	star.spectr = (ESpectrType)round(num6 + 4.0f);

	star.luminosity = pow(num5, 0.7f);
	star.radius = (pow(star.mass, 0.4f) * num3);
	float p = num6 + 2.0f;
	star.orbitScaler = pow(1.35f, p);
	if (star.orbitScaler < 1.0f)
	{
		star.orbitScaler = lerp(star.orbitScaler, 1.0f, 0.6f);
	}
	SetStarAge(star, star.age, rn, rt);
	star.dysonRadius = star.orbitScaler * 0.28f;
	if (star.dysonRadius * 40000.0f < (star.radius * 1200.0f * 1.5f))
	{
		star.dysonRadius = ((star.radius * 1200.0f * 1.5f) / 40000.0f);
	}
	return star;
}

float Generate(int seed, int starCount)
{
	Star stars[64];
	URandom random;
	InitRandom(random, seed);
	int seed1 = RandomNext(random);

	int num6 = ceil(0.01f * starCount + RandomNextFloat(random) * 0.3f);
	int num7 = ceil(0.01f * starCount + RandomNextFloat(random) * 0.3f);
	int num8 = ceil(0.016f * starCount + RandomNextFloat(random) * 0.4f);
	int num9 = ceil(0.013f * starCount + RandomNextFloat(random) * 1.4f);

	int num10 = starCount - num6;
	int num11 = num10 - num7;
	int num12 = num11 - num8;
	int num13 = (num12 - 1) / num9;
	int num14 = num13 / 2;

	float maxLum = 0.0f;

	for (int i = 0; i < starCount; i++)
	{
		int seed2 = RandomNext(random);
		URandom randomX;
		InitRandom(randomX, seed2);
		int notuse1 = RandomNext(randomX);
		int seed3 = RandomNext(randomX);
		if (i == 0)
		{
			stars[i] = CreateBirthStar(seed2, seed3);
		}
		else
		{
			ESpectrType needSpectr = ESpectrType_X;
			if (i == 3)
			{
				needSpectr = ESpectrType_M;
			}
			else if (i == num12 - 1)
			{
				needSpectr = ESpectrType_O;
			}
			EStarType starType = EStarType_MainSeqStar;
			if (i % num13 == num14)
			{
				starType = EStarType_GiantStar;
			}
			if (i >= num10)
			{
				starType = EStarType_BlackHole;
			}
			else if (i >= num11)
			{
				starType = EStarType_NeutronStar;
			}
			else if (i >= num12)
			{
				starType = EStarType_WhiteDwarf;
			}
			stars[i] = CreateStar(starCount, i + 1, seed2, starType, needSpectr, seed3);
		}
		maxLum = max(maxLum, stars[i].luminosity);
	}
	return maxLum;
}

//[numthreads(16, 1, 1)]
//void main(uint3 id : SV_DispatchThreadID)
//{
//	int index = id.x + _startIndex;
//	int saveIndex = id.x + _saveStartIndex;
//	URandom random;
//	InitRandom(random, index);
//	for (int j = 0; j < 5; j++)
//	{
//		RandomNext(random);
//	}
//	Result[saveIndex] = RandomNext(random);
//
//	return;
//}

[numthreads(16, 1, 1)]
void main(uint3 id : SV_DispatchThreadID)
{
	int index = id.x + _startIndex;
	int saveIndex = id.x + _saveStartIndex;

	Result[saveIndex] = Generate(index, 64);

	return;
}