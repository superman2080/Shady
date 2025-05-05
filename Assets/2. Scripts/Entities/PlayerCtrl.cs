using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCtrl : Entity
{
    public float dashDistance;
    public float dashTime;
    private TrailRenderer tR;
    private Coroutine dashCor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        tR = gameObject.GetComponent<TrailRenderer>();
        tR.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShadowDash(0.2f, dashDistance, dashTime);
        }
    } 

    protected override void OnEntityDied(Entity caster)
    {
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    protected override void OnTakeDamage(Entity caster, float amount)
    {
    }

    private void ShadowDash(float castingTime, float dashDist, float dashTime)
    {
        if (dashCor == null)
            dashCor = StartCoroutine(ShadowDashCor(castingTime, dashDist, dashTime));
    }

    private IEnumerator ShadowDashCor(float castingTime, float dashDist, float dashTime)
    {
        tR.enabled = true;

        for (float i = 0; i <= castingTime; i += Time.deltaTime) 
        {
            Time.timeScale = Mathf.Lerp(1f, 0.1f, i / castingTime);
            yield return null;
        }

        Vector2 origin = (Vector2)transform.position;
        Vector2 moveTo = Vector2.zero;
        IDamagable[] targets;
        Vector2 targetPos = Vector2.zero;
        while (true)
        {
            yield return null;
            if (Input.GetMouseButtonUp(0))
            {
                moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 dir = (moveTo - origin).normalized;
                Debug.DrawRay(origin, dir * dashDist, Color.red, 0.5f);
                RaycastHit2D[] targetHit = Physics2D.RaycastAll(transform.position, dir, dashDist, 1 << LayerMask.NameToLayer("Entity"));
                if (targetHit.Any(t => t.collider.GetComponent<Entity>() is Enemy) && targetHit.Last(t => t.collider.GetComponent<Entity>() is Enemy))
                {
                    targets = targetHit.Select(t => t.collider.GetComponent<IDamagable>()).Where(comp => comp != null).ToArray();
                    targetPos = (Vector2)targetHit.Last().transform.position - (targetHit.Last().point - (Vector2)targetHit.Last().transform.position) * 2;
                    break;
                }
                else
                {
                    Time.timeScale = 1;
                    tR.enabled = false;
                    dashCor = null;
                    Debug.Log("Failed casting");
                    yield break;
                }
            }
        }




        foreach (var target in targets)
        {
            if (!target.Equals(this))
            {
                Debug.Log(target.GetType().Name);
                target.TakeDamage(this, stat.Get(StatType.DAMAGE));
            }
        }

        for (float i = 0; i < dashTime; i += Time.deltaTime) 
        {
            Time.timeScale = Mathf.Lerp(0.1f, 1, i / dashTime);
            transform.position = Vector2.Lerp(origin, targetPos, i / dashTime);
            yield return null;
        }
        tR.enabled = false;
        dashCor = null;
    }
}
