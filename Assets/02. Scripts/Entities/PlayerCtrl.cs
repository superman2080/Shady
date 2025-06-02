using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class PlayerCtrl : Entity, ICameraLookable
{
    [Header("Related to light")]
    #region Light Attribute
    public GameObject lightPrefab;
    [Min(1)] public float throwPower;
    public float retrieveDist;
    private Coroutine throwCor;
    #endregion

    [Header("Related to shadow")]
    #region Shadow Attribute
    [HideInInspector] public AnimationCurve dashSpeed = AnimationCurve.Linear(0, 1, 1, 0);
    private TrailRenderer dashTrail;
    private Coroutine dashCor;
    private LineRenderer dashTrajectory;
    #endregion

    #region Shadow Dash Attribute
    [Header("Related to shadow dash")]
    public float dashDistance;
    public float maxDashStamina;
    public float dashStamina;
    public float dashCost;
    public float delayRecoverTime;
    public float recoverStamina;
    public float dashTime;
    public float maxDiveTime;
    public float diveTime;
    #endregion


    void OnEnable()
    {
        EnableCamera();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        dashTrail = gameObject.GetComponent<TrailRenderer>();
        dashTrail.enabled = false;
        WeaponController = new WeaponCtrl(this, new Dagger());

        dashTrajectory = gameObject.GetComponent<LineRenderer>();
        dashTrajectory.enabled = false;

        dashStamina = maxDashStamina;
        diveTime = maxDiveTime;
        StartCoroutine(StaminaCor());
        #region Debug...
        stat.SetDefault(StatType.MOVE_SPEED, 5);
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        DiveShadow();
    }

    void FixedUpdate()
    {
        KeyInput();
        Move();
    }

    void OnDisable()
    {
        DisableCamera();
    }

    private void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShadowDash(0.2f, dashDistance, dashTime);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowLight(throwPower);
        }
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
                Camera.main.ScreenPointToRay(Input.mousePosition).direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("Light"));
            if(hit && Vector2.Distance(transform.position, hit.transform.position) <= retrieveDist)
            {
                Destroy(hit.collider.gameObject);
            }
            else
            {
                Attack(this, stat.Get(StatType.DAMAGE));
            }
        }
    }

    private void Move()
    {
        var inputVector = (Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical")).normalized;
        rb2d.MovePosition((Vector2)transform.position + (inputVector * stat.Get(StatType.MOVE_SPEED) * Time.fixedDeltaTime));
        var dir = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        var targetRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

    }

    private void DiveShadow()
    {
        diveTime = IsInShadow().isIn ? Mathf.Clamp(diveTime - Time.deltaTime, 0, maxDiveTime) : Mathf.Clamp(diveTime + Time.deltaTime, 0, maxDiveTime);

        if(diveTime > 0 && IsInShadow().isIn)
        {
            IsInShadow().col.isTrigger = true;
        }
        if(diveTime <= 0 && IsInShadow().isIn)
        {
            IsInShadow().col.isTrigger = false;
        }
    }

    private IEnumerator StaminaCor()
    {
        float lateStamina = dashStamina;
        while (true)
        {
            if (lateStamina != dashStamina)
            {
                yield return new WaitForSeconds(delayRecoverTime);
            }
            dashStamina = Mathf.Clamp(dashStamina + recoverStamina * Time.deltaTime, 0, maxDashStamina);
            lateStamina = dashStamina;
            yield return null;
        }
    }

    private void ThrowLight(float tP)
    {
        if (throwCor == null)
            throwCor = StartCoroutine(ThrowLightCor(tP));
    }

    private void ShadowDash(float castingTime, float dashDist, float dashTime)
    {
        if (dashCor == null && dashStamina > 0)
            dashCor = StartCoroutine(ShadowDashCor(castingTime, dashDist, dashTime));
    }

    private IEnumerator ShadowDashCor(float castingTime, float dashDist, float dashTime)
    {
        // Start casting


        UI.Instance.Fade(false, Color.black, castingTime, 0.4f);
        for (float eT = 0; eT <= castingTime; eT += Time.deltaTime) 
        {
            Time.timeScale = Mathf.Lerp(1f, 0.1f, eT / castingTime);
            yield return null;
        }

        Vector2 origin = (Vector2)transform.position;
        Vector2 moveTo = Vector2.zero;
        IDamagable[] targets;
        Vector2 targetPos = Vector2.zero;
        dashTrail.enabled = true;
        dashTrajectory.enabled = true;

        while (true)
        {
            yield return null;
            // Draw dash trajectory
            moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (moveTo - origin).normalized;
            Vector2 maxDashPos = origin + dir * dashDist;
            targetPos = Vector2.Distance(origin, moveTo) > dashDist ? maxDashPos : moveTo;
            dashTrajectory.SetPosition(0, origin);
            dashTrajectory.SetPosition(1, targetPos);
            //


            if (Input.GetMouseButtonUp(0))
            {
                dashTrajectory.enabled = false;
                moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
                    Camera.main.ScreenPointToRay(Input.mousePosition).direction, Vector2.Distance(origin, moveTo),
                    1 << LayerMask.NameToLayer("Shadow"));

                if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Shadow") && IsInShadow().isIn)
                {
                    RaycastHit2D[] targetHit = Physics2D.CircleCastAll(transform.position, (col as CircleCollider2D).radius, dir, dashDist, attackLayer);
                    targets = targetHit.Select(t => t.collider.GetComponent<IDamagable>()).ToArray();
                    foreach (var target in targets)
                    {
                        if (!target.Equals(this))
                        {
                            target.TakeDamage(this, target.HP);
                            Debug.Log(target.GetType().Name);
                        }
                    }
                }
                break;
            }
        }
        Time.timeScale = 1;

        UI.Instance.Fade(true, Color.black, castingTime, 0.4f);
        dashStamina -= dashCost;

        for (float eT = 0; eT <= dashTime; eT += Time.deltaTime) 
        {
            transform.position = Vector2.Lerp(origin, targetPos, dashSpeed.Evaluate(eT / dashTime));
            yield return null;
        }

        dashTrail.enabled = false;
        dashCor = null;
    }

    IEnumerator ThrowLightCor(float tP)
    {
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        Rigidbody2D obj = Instantiate(lightPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        obj.linearDamping = 1f;
        obj.AddForce(dir * tP, ForceMode2D.Impulse);
        throwCor = null;
    }

    public void EnableCamera()
    {
        MainCineCam.Instance.targetTrList.Add(transform);
    }

    public void DisableCamera()
    {
        MainCineCam.Instance.targetTrList.Remove(transform);
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

    protected override void OnEntityAttack(Entity caster, float amount)
    {
    }


}
