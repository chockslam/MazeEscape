using System;
using UnityEngine;

public static class MathHelper
{
    public static float MagnitudeOnXZPlane(Vector3 vector)
    {
        return Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
    }

    public static Vector3 NormalizeOnXZPlane(Vector3 vector)
    {
        float magnitude = MagnitudeOnXZPlane(vector);

        if (magnitude > Mathf.Epsilon)
        {
            vector.x /= magnitude;
            vector.z /= magnitude;
        }
        else
        {
            vector.x = 0;
            vector.z = 0;
        }
        vector.y = 0;
        return vector;
    }

    public static int GetManhattanDistance(Vector3 posA, Vector3 posB)
    {
        return Mathf.Abs((int)(posA.x - posB.x)) + Mathf.Abs((int)(posA.z - posB.z));
    }

    public static float SqrMagnitudeOnXZPlane(Vector3 vector)
    {
        return vector.x * vector.x + vector.z * vector.z;
    }

    public static float DotProductOnXZPlane(Vector3 vectorA, Vector3 vectorB)
    {
        return vectorA.x * vectorB.x + vectorA.z * vectorB.z;
    }

    public static float DistanceOnXZPlane(Vector3 pointA, Vector3 pointB)
    {
        return MagnitudeOnXZPlane(pointB - pointA);
    }

    public static Vector3 RotateAroundYAxis(Vector3 vector, float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        
        float newX = vector.x * cos - vector.z * sin;
        float newZ = vector.x * sin + vector.z * cos;

        return new Vector3(newX, 0, newZ);
    }

    public static Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal, float yAngleDegrees)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += yAngleDegrees;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
