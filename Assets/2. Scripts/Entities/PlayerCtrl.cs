using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class PlayerCtrl : Entity
{
    public float dashDistance;
    public float dashTime;
    public LayerMask attackLayer;

    private TrailRenderer tR;
    [HideInInspector] public AnimationCurve dashSpeed = AnimationCurve.Linear(0, 1, 1, 0);
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
        // Start casting
        tR.enabled = true;
        UI.Instance.Fade(false, Color.black, castingTime, 0.5f);
        for (float eT = 0; eT <= castingTime; eT += Time.deltaTime) 
        {
            Time.timeScale = Mathf.Lerp(1f, 0.1f, eT / castingTime);
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
                RaycastHit2D[] targetHit = Physics2D.RaycastAll(transform.position, dir, dashDist, attackLayer);
                var lastTarget = targetHit.LastOrDefault(t => t.collider.GetComponent<Entity>() is Enemy && t.collider.GetComponent<Enemy>().IsInShadow().isIn);
                if (lastTarget != default)
                {
                    targets = targetHit.Select(t => t.collider.GetComponent<IDamagable>()).Where(comp => comp != null).Take(Array.IndexOf(targetHit, lastTarget)).ToArray();
                    targetPos = (Vector2)lastTarget.transform.position - (lastTarget.point - (Vector2)lastTarget.transform.position) * 2;
                    break;
                }
                else
                {
                    Time.timeScale = 1;
                    UI.Instance.Fade(true, Color.black, 0.1f, 0.5f);
                    tR.enabled = false;
                    dashCor = null;
                    Debug.Log("Failed casting");
                    yield break;
                }
            }
        }



        Time.timeScale = 1;
        UI.Instance.Fade(true, Color.black, 0.1f, 0.5f);

        for (float eT = 0; eT <= dashTime; eT += Time.deltaTime) 
        {
            transform.position = Vector2.Lerp(origin, targetPos, dashSpeed.Evaluate(eT / dashTime));
            yield return null;
        }

        foreach (var target in targets)
        {
            if (!target.Equals(this))
            {
                Debug.Log(target.GetType().Name);
                target.TakeDamage(this, stat.Get(StatType.DAMAGE));
            }
        }

        tR.enabled = false;
        dashCor = null;
    }
}
