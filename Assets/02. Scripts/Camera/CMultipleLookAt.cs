using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CMultipleLookAt : StateBase<MainCineCam>
{

    public override void Enter(MainCineCam caster)
    {
    }

    public override void Execute(MainCineCam caster)
    {
        List<Transform> tL = caster.targetTrList;

        Vector2 targetPos = new Vector2(tL.Average(tr => tr.position.x), tL.Average(tr => tr.position.y));
        caster.LookAt(targetPos);

        float minX = tL.Min(o => o.position.x);
        float maxX = tL.Max(o => o.position.x);
        float minY = tL.Min(o => o.position.y);
        float maxY = tL.Max(o => o.position.y);

        float width = maxX - minX;
        float height = maxY - minY;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        float aspect = (float)Screen.width / Screen.height;

        float requiredOrthoSize = Mathf.Max(halfHeight, halfWidth / aspect);

        caster.SetOrthoSize(Mathf.Max(requiredOrthoSize + 5f, caster.minOrthoSize));
    }
    public override void Exit(MainCineCam caster)
    {
    }
}
