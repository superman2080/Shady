using UnityEngine;
using System.Collections.Generic;

public class DropTile : MonoBehaviour
{
    private List<PolygonCollider2D> ignores = new List<PolygonCollider2D>();
    private List<Shadow> shadows = new List<Shadow>();

    void Update()
    {
        RemoveInactiveShadows();
        EnsureColliderCountMatches();
        UpdateIgnoreVertices();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shadow"))
        {
            var shadow = collision.GetComponent<Shadow>();
            if (shadow != null && !shadows.Contains(shadow))
            {
                shadows.Add(shadow);
            }
        }
    }

    private void RemoveInactiveShadows()
    {
        for (int i = shadows.Count - 1; i >= 0; i--)
        {
            if (!shadows[i].gameObject.activeSelf)
            {
                if (i < ignores.Count)
                {
                    Destroy(ignores[i]);
                    ignores.RemoveAt(i);
                }

                shadows.RemoveAt(i);
            }
        }
    }


    // Equalizing ignore collier count with shadow count
    private void EnsureColliderCountMatches()
    {
        while (ignores.Count < shadows.Count)
        {
            var newCol = gameObject.AddComponent<PolygonCollider2D>();
            newCol.compositeOperation = Collider2D.CompositeOperation.Difference;
            newCol.pathCount = 0;
            ignores.Add(newCol);
        }
    }

    private void UpdateIgnoreVertices()
    {
        for (int i = 0; i < ignores.Count && i < shadows.Count; i++)
        {
            SetIgnoreVertex(ignores[i], shadows[i]);
        }
    }

    private void SetIgnoreVertex(PolygonCollider2D ignore, Shadow shadow)
    {
        if (shadow.col == null) return;

        ignore.pathCount = shadow.col.pathCount;
        for (int i = 0; i < shadow.col.pathCount; i++)
        {
            ignore.SetPath(i, shadow.col.GetPath(i));
        }
    }
}
