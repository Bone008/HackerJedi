#ifndef SPHEREMAPPING_CGINC
#define SPHEREMAPPING_CGINC

float Sphere_Depth;
float Sphere_Angle_X;
float Sphere_Angle_Y;
float Sphere_Radius;

float4 MapCoordinate(float4 coord)
{
	float thetaX = (coord.x / _ScreenParams.x) * Sphere_Angle_X;
	float thetaY = (coord.y / _ScreenParams.y) * Sphere_Angle_Y;
	float radius = Sphere_Radius * _ScreenParams.x;
	float depth = Sphere_Depth * _ScreenParams.x;
	
	coord.x = sin(thetaX) * radius;
	coord.y = sin(thetaY) * radius;
	coord.z = (cos(thetaX) * cos(thetaY) * radius) + depth;
	
	return coord;
}

#endif
