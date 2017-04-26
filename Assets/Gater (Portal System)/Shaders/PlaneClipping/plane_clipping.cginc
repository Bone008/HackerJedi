#ifndef PLANE_CLIPPING_INCLUDED
#define PLANE_CLIPPING_INCLUDED

//Plane clipping definitions. Uses three planes for clipping, but this can be increased if necessary.

#define PLANE_CLIPPING_ENABLED 1

//http://mathworld.wolfram.com/Point-PlaneDistance.html
float distanceToPlane(half3 planePosition, half3 planeNormal, half3 pointInWorld)
{
  //w = vector from plane to point
  half3 w = - ( planePosition - pointInWorld );
  half res = ( planeNormal.x * w.x + 
				planeNormal.y * w.y + 
				planeNormal.z * w.z ) 
	/ sqrt( planeNormal.x * planeNormal.x +
			planeNormal.y * planeNormal.y +
			planeNormal.z * planeNormal.z );
  return res;
}

//we will have at least one plane.
float4 _planePos;
float4 _planeNorm;

void PlaneClip(float3 posWorld) {
clip(distanceToPlane(_planePos.xyz, _planeNorm.xyz, posWorld));
}

//preprocessor macro that will produce an empty block if no clipping planes are used.
#define PLANE_CLIP(posWorld) PlaneClip(posWorld);
    
#else
//empty definition
#define PLANE_CLIP(s)
#endif