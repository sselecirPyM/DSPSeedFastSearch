#include "Random.hlsli"
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

	return;
}