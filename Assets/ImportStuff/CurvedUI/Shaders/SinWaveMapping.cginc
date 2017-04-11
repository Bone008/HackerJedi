#ifndef SINWAVEMAPPING_CGINC
#define SINWAVEMAPPING_CGINC

float SinWave_Scale;
float SinWave_Phase;
float SinWave_Amplitude;

float4 MapCoordinate(float4 coord)
{
	float theta = ((coord.x / _ScreenParams.x) * SinWave_Scale) + SinWave_Phase;
	coord.y += sin(theta) * SinWave_Amplitude;
	return coord;
}

#endif
