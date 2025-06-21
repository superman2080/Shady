using UnityEngine;

public class Util
{
    public static bool RollChanceByPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);
        return Random.value <= percent;
    }

    public static bool IsVisibleFromCamera(Camera cam, Transform target)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(target.position);
        return viewPos.z > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1;

    }
}
