using UnityEngine;
using System.Linq;

public enum StatType
{
    MAX_HP,
    MOVE_SPEED,

}

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Rigidbody2D rb2d;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    protected (bool, Collider2D?)IsInShadow()
    {
        foreach (var shadow in ShadowPool.Instance.GetChildShadowList(false))
        {
            if (col.IsTouching(shadow.col))
                return (true, shadow.col);
        }
        return (false, null);
    }
}
