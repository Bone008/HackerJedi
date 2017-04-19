#ifndef CYLINDERMAPPING_CGINC
#define CYLINDERMAPPING_CGINC

float Cylinder_Depth;
float Cylinder_Angle;
float Cylinder_Radius;

float4 MapCoordinate(float4 coord)
{
	float theta = (coord.x / _ScreenParams.x) * Cylinder_Angle;
	float radius = Cylinder_Radius * _ScreenParams.x;
	float depth = Cylinder_Depth * _ScreenParams.x;
	
	coord.x = sin(theta) * radius;
	coord.z = (cos(theta) * radius) + depth;
	
	return coord;
}

#endif
