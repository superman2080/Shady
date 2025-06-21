using UnityEngine;

public class GameMath
{
    public static float DirectionToAngle(Vector3 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
    public static Vector2 AngleToDirection(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    public static float GetNormalAngle(float angle)
    {
        return (angle % 360f + 360f) % 360f;
    }

    public static Vector2 GetOffsetPosition(Vector2 origin, Vector2 moveTo, float dist)
    {
        return moveTo - ((origin - moveTo).normalized * -dist);
    }

    public static Vector2 RotateDirection(Vector2 dir, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            dir.x * cos - dir.y * sin,
            dir.x * sin + dir.y * cos
        ).normalized;
    }


    public static bool IsLookingDir(Transform tr, Vector2 target, float threshold)
    {
        return Vector2.Angle(tr.right.normalized, target) <= threshold;
    }


}
