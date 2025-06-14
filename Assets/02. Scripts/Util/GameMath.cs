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
    public static bool RollChanceByPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);
        return Random.value <= percent;
    }

    public static Vector2 GetOffsetPosition(Vector2 origin, Vector2 moveTo, float dist)
    {
        return moveTo - ((origin - moveTo).normalized * -dist);
    }
}
