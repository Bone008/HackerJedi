#define DEBUG_VISUALISE

using UnityEngine;
using System.Collections;

public class SphereMapping : CanvasMapping
{
    [Range(-10.0f, 10.0f)]
    public float depth = -1.0f;

    [Range(0.0f, 360.0f)]
    public float angleX = 180.0f;

    [Range(0.0f, 360.0f)]
    public float angleY = 180.0f;

    [Range(0.0f, 10.0f)]
    public float radius = 1.0f;

    protected override void Awake()
    {
        base.Awake();

        if (m_canvas.renderMode != RenderMode.ScreenSpaceCamera)
            Debug.LogWarning("Sphere mapping probably works best in ScreenSpaceCamera mode", this);
    }

    #region CanvasMapping

    public override bool MapScreenToCanvas(Vector2 screenCoord, out Vector2 o_canvasCoord)
    {
        Camera worldCamera = m_canvas.worldCamera;
        if (worldCamera != null)
        {
            // Get the camera transform
            Transform worldCameraTransform = worldCamera.transform;

            // Get a ray from the camera through the point on the screen
            Ray ray3D = worldCamera.ScreenPointToRay(screenCoord);

            // Transform the ray direction into view space
            Vector3 localRayDirection = worldCameraTransform.InverseTransformDirection(ray3D.direction);

            // Calculate the view space size of the canvas
            float thetaFOVH = worldCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float tanFOVH = Mathf.Tan(thetaFOVH);
            float tanFOVW = tanFOVH * worldCamera.aspect;

            // Flatten cylinder and ray to 2D so this becomes a ray circle intersection
            Vector2 rayDirection2DX = new Vector2(localRayDirection.x, localRayDirection.z);
            Vector2 circlePositionX = new Vector2(0.0f, 1.0f + (tanFOVW * depth));
            float circleRadiusX = tanFOVW * radius;

            // dont know if correct
            Vector2 rayDirection2DY = new Vector2(localRayDirection.y, localRayDirection.z);
            Vector2 circlePositionY = new Vector2(0.0f, 1.0f + (tanFOVH * depth));
            float circleRadiusY = tanFOVH * radius;

            // Determine the far intersection, if there is one
            float farIntersectionX;
            float farIntersectionY;
            if (RayCircle2D(Vector2.zero, rayDirection2DX, circlePositionX, circleRadiusX, out farIntersectionX))
            {
                if (RayCircle2D(Vector2.zero, rayDirection2DY, circlePositionY, circleRadiusY, out farIntersectionY))
                {
                    // Intersection point on the XZ plane
                    Vector2 cylinderXZ = (rayDirection2DX * farIntersectionX) - circlePositionX;
                    Vector2 cylinderYZ = (rayDirection2DY * farIntersectionY) - circlePositionY;

                    // XZ -> angle around cylinder
                    float cylinderThetaX = Mathf.Atan2(cylinderXZ.x, cylinderXZ.y);
                    float cylinderThetaY = Mathf.Atan2(cylinderYZ.x, cylinderYZ.y);

                    // Y intersection
                    //float cylinderY = localRayDirection.y * farIntersection;

                    // To viewport [-1, 1]
                    float viewportX = cylinderThetaX / (angleX * 0.5f * Mathf.Deg2Rad);
                    float viewportY = cylinderThetaY / (angleY * 0.5f * Mathf.Deg2Rad); // cylinderY / tanFOVH;

                    // To canvas
                    Vector2 canvasSize = m_canvas.pixelRect.size;
                    o_canvasCoord.x = ((viewportX * 0.5f) + 0.5f) * canvasSize.x;
                    o_canvasCoord.y = ((viewportY * 0.5f) + 0.5f) * canvasSize.y;

#if DEBUG_VISUALISE
                    {
                        Vector3 intersection3DX = worldCameraTransform.TransformPoint(localRayDirection * (farIntersectionX * m_canvas.planeDistance));
                        Debug.DrawLine(worldCameraTransform.position, intersection3DX, Color.blue);
                        Vector3 intersection3DY = worldCameraTransform.TransformPoint(localRayDirection * (farIntersectionY * m_canvas.planeDistance));
                        Debug.DrawLine(worldCameraTransform.position, intersection3DY, Color.red);
                    }
#endif

                    return true;
                }                
            }
        }

        o_canvasCoord = Vector2.zero;
        return false;
    }

    public override void SetCanvasToScreenParameters(Material material)
    {
        material.SetFloat("Sphere_Depth", depth * 0.5f);
        material.SetFloat("Sphere_Angle_X", angleX * Mathf.Deg2Rad);
        material.SetFloat("Sphere_Angle_Y", angleY * Mathf.Deg2Rad);
        material.SetFloat("Sphere_Radius", radius * 0.5f);
    }

    #endregion
    
    bool RayCircle2D(Vector2 rayStart, Vector2 rayDirection, Vector2 circlePosition, float circleRadius, out float o_farIntersection)
    {
        Vector2 f = rayStart - circlePosition;

        float a = Vector2.Dot(rayDirection, rayDirection);
        float b = 2.0f * Vector2.Dot(f, rayDirection);
        float c = Vector2.Dot(f, f) - (circleRadius * circleRadius);

        float discriminantSq = (b * b) - (4.0f * a * c);
        if (discriminantSq < 0.0f)
        {
            // No intersection
            o_farIntersection = 0.0f;
            return false;
        }
        else
        {
            float discriminant = Mathf.Sqrt(discriminantSq);
            o_farIntersection = (-b + discriminant) / (2.0f * a);
            return true;
        }
    }

}
