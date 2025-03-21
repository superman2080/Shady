using UnityEngine;
using System.Collections.Generic;

public class LightObject : MonoBehaviour
{
    //public List<Entity> collidedEntities { private set; get; }

    public Dictionary<Entity, Vector2[]> test;

    public Dictionary<Entity, Vector2[]> GetCollidedPosition()
    {
        Dictionary<Entity, Vector2[]> result = new Dictionary<Entity, Vector2[]>();

        Entity[] entities = GameObject.FindObjectsByType<Entity>(FindObjectsSortMode.InstanceID);
        foreach (var entity in entities)
        {
            Vector2 dir = (entity.transform.position - transform.position).normalized;
            Vector2 upDir, downDir;
            upDir = downDir = dir;
            RaycastHit2D hit;
            int deg = 0;
            for (deg = 0; deg < 180; deg++)
            {
                hit = Physics2D.Raycast(transform.position, upDir, float.MaxValue);
                if (hit)
                {
                    upDir = Quaternion.Euler(0, 0, 1) * dir;
                }
                else
                {
                    break;
                }
            }
            upDir = Quaternion.Euler(0, 0, deg) * upDir;
            downDir = Quaternion.Euler(0, 0, -deg) * downDir;
            if(upDir != downDir)
            {
                result.Add(entity, new Vector2[] { upDir, downDir });
            }
            else
            {
                Debug.Log("Can't detect object!");
            }

        }
        return result;
    }

    [ContextMenu("½ÇÇà")]
    public void Test()
    {
        test = GetCollidedPosition();
    }

    private void OnDrawGizmos()
    {
        if(test != null)
        {
            Gizmos.color = Color.red;
            foreach (var item in test)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, item.Value[i] * 30);
                    Gizmos.DrawRay(transform.position, (item.Key.transform.position - transform.position).normalized * 30f);
                }
            }
        }
    }
}
