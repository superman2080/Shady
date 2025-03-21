using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public float centerX, centerY, left, top, right, bottom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoxCollider2D boxCol = gameObject.GetComponent<BoxCollider2D>();
        centerX = transform.position.x + boxCol.bounds.extents.x;
        centerY = transform.position.y - boxCol.bounds.extents.y;
        left = transform.position.x;
        top = transform.position.y;
        right = transform.position.x + boxCol.bounds.size.x;
        bottom = transform.position.y - boxCol.bounds.size.y;
    }
}
