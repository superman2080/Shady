using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class PlayerCtrl : Entity, ICameraLookable, IAttackable
{
    [Header("Related to light")]
    #region Light Attribute
    [Min(1)] public float throwPower;
    [Min(1)] public float throwTime;
    public float retrieveDist;
    [HideInInspector] public ShadowCaster lantern;
    public SliderValue lanternValue;
    private Coroutine throwCor;
    #endregion

    #region Shadow Attribute
    [Header("Related to shadow")]
    #region Shadow Dash Attribute
    public bool isDiving;
    #endregion

    [Header("Related to shadow dash")]
    public float dashDistance;
    public float dashCost;
    public float dashTime;
    public SliderValue dashValue;
    private TrailRenderer dashTrail;
    private Coroutine dashCor;
    private LineRenderer dashTrajectory;
    [HideInInspector] public AnimationCurve dashSpeed = AnimationCurve.Linear(0, 1, 1, 0);
    #endregion

    #region IAttackable

    public WeaponCtrl WeaponController { get; protected set; }

    [SerializeField] public LayerMask AttackLayer { get => 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Enemy"); }
    #endregion

    void OnEnable()
    {
        EnableCamera();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        WeaponController = new WeaponCtrl(this);
        WeaponController.SetWeapon(new Dagger());
        WeaponController.weaponStat.InitStat();

        #region Lantern
        lantern = transform.Find("Light").gameObject.GetComponent<ShadowCaster>();
        lantern.transform.SetParent(transform);
        lantern.transform.localPosition = Vector2.zero;
        lantern.gameObject.SetActive(false);
        #endregion
        
        #region Dash
        dashTrail = gameObject.GetComponent<TrailRenderer>();
        dashTrail.enabled = false;

        dashTrajectory = gameObject.GetComponent<LineRenderer>();
        dashTrajectory.enabled = false;

        #endregion
        #region Debug...
        entityStat.SetDefault(EntityStatType.MOVE_SPEED, 5);
        #endregion

    }

    void Update()
    {
        DiveShadow();
        UpdateStamina();
    }

    void FixedUpdate()
    {
        if (canBehavior)
        {
            KeyInput();
            Move();
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        WeaponController.weaponStat.Update();
    }

    void OnDisable()
    {
        DisableCamera();
    }

    private void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShadowDash(0.2f, dashDistance, dashTime);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowLight(throwPower, throwTime);
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition).origin,
                Camera.main.ScreenPointToRay(Input.mousePosition).direction, Mathf.Infinity);
            var hit = hits.Select(h => h.collider.GetComponent<ITouchable>()).Where(h => h != null).FirstOrDefault();
            if (hit != default && Vector3.Magnitude((hit as MonoBehaviour).transform.position - transform.position) <= retrieveDist)
            {
                hit.HasTouched(this);
            }
            else
            {
                Attack(this, WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDiving = !isDiving;
        }
    }

    private void Move()
    {
        var inputVector = (Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical")).normalized;
        rb2d.MovePosition((Vector2)transform.position + (inputVector * entityStat.Get(EntityStatType.MOVE_SPEED) * Time.fixedDeltaTime));
        var dir = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        var targetRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

    }

    private void DiveShadow()
    {
        //diveTime = isDiving && IsInShadow().isIn ? Mathf.Clamp(diveTime - Time.deltaTime, 0, maxDiveTime) : Mathf.Clamp(diveTime + Time.deltaTime, 0, maxDiveTime);

        if (isDiving && lanternValue.val > 0 && IsInShadow().isIn)
        {
            IsInShadow().col.isTrigger = true;
        }
        if (!isDiving || lanternValue.val <= 0)
        {
            foreach (var shadow in ShadowPool.Instance.GetChildShadowList())
            {
                shadow.col.isTrigger = false;
            }
        }
    }

    private void UpdateStamina()
    {
        dashValue.val = Mathf.Clamp(dashValue.val + dashValue.recoverVal * Time.deltaTime, 0, dashValue.maxVal);
        if(lantern != null)
        {
            lanternValue.val = Mathf.Clamp(lanternValue.val + lanternValue.recoverVal * Time.deltaTime, 0, lanternValue.maxVal);
        }
    }

    private void ThrowLight(float tP, float time)
    {
        if (throwCor == null && lantern != null)
            throwCor = StartCoroutine(ThrowLightCor(tP, time));
    }

    private void ShadowDash(float castingTime, float dashDist, float dashTime)
    {
        if (dashCor == null && dashValue.val > 0)
            dashCor = StartCoroutine(ShadowDashCor(castingTime, dashDist, dashTime));
    }

    private IEnumerator ShadowDashCor(float castingTime, float dashDist, float dashTime)
    {
        // Start casting


        InGameUI.Instance.Fade(false, Color.black, castingTime, 0.4f);
        for (float eT = 0; eT <= castingTime; eT += Time.deltaTime) 
        {
            Time.timeScale = Mathf.Lerp(1f, 0.1f, eT / castingTime);
            yield return null;
        }

        Vector2 origin = transform.position;
        Vector2 moveTo = Vector2.zero;
        List<IDamagable> damagables = new List<IDamagable>();
        Vector2 targetPos = Vector2.zero;
        var startShadow = IsInShadow().col;
        dashTrail.enabled = true;
        dashTrajectory.enabled = true;

        while (true)
        {
            entityStat.Multiply(EntityStatType.MOVE_SPEED, 0);
            yield return null;
            // Draw dash trajectory
            moveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (moveTo - origin).normalized;
            Vector2 maxDashPos = origin + dir * dashDist;
            moveTo = Vector2.Distance(origin, moveTo) > dashDist ? maxDashPos : moveTo;
            dashTrajectory.SetPosition(0, origin);
            dashTrajectory.SetPosition(1, moveTo);
            //



            if (Input.GetMouseButtonUp(0))
            {
                dashTrajectory.enabled = false;
                RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, (col as CircleCollider2D).radius, (moveTo - origin).normalized, Vector2.Distance(origin, moveTo),
                    1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Tile") | 1 << LayerMask.NameToLayer("ScanTile") 
                    | 1 << LayerMask.NameToLayer("Shadow") | 1 << LayerMask.NameToLayer("Drop") | 1 << LayerMask.NameToLayer("Enemy"));
                targetPos = moveTo;
                if (hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider.TryGetComponent(out IDamagable damagable))
                        {
                            Debug.LogError(hits[i].collider.name);
                            damagables.Add(damagable);
                        }
                        else if (hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Shadow") && hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
                        {
                            targetPos = GameMath.GetOffsetPosition(origin, hits[i].point, (col as CircleCollider2D).radius);
                            break;
                        }
                    } 
                }
                break;
            }
        }
        Time.timeScale = 1;

        InGameUI.Instance.Fade(true, Color.black, castingTime, 0.4f);
        dashValue.val -= dashCost;

        for (float eT = 0; eT <= dashTime; eT += Time.deltaTime) 
        {
            transform.position = Vector2.Lerp(origin, targetPos, dashSpeed.Evaluate(eT / dashTime));
            yield return null;
        }

        if(startShadow != null && IsInShadow().col != null && startShadow != IsInShadow().col)
        {
            foreach (var obj in damagables)
            {
                obj.TakeDamage(this, obj.HP);
            }
            dashTrail.startColor = dashTrail.endColor = Color.white;
        }
        else
        {
            dashTrail.startColor = dashTrail.endColor = Color.black;
        }

        dashTrail.enabled = false;
        dashCor = null;
    }

    IEnumerator ThrowLightCor(float tP, float time)
    {
        float eT = 0;
        
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                eT += Time.deltaTime;
            }
            if (Input.GetMouseButtonUp(0))
                break;
            yield return null;
        }
        float power = eT > time ? tP : Mathf.Lerp(0.1f, tP, eT / time);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        lantern.gameObject.SetActive(true);
        lantern.transform.SetParent(null);
        Rigidbody2D lanternRb = lantern.gameObject.GetComponent<Rigidbody2D>();
        lanternRb.linearVelocity = Vector2.zero; 
        lanternRb.angularVelocity = 0f;    
        lanternRb.linearDamping = 1f;
        lanternRb.AddForce(dir * power, ForceMode2D.Impulse);
        lantern = null;
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

    protected override void OnEntityDied(IAttackable caster)
    {
        MainCineCam.Instance.targetTrList.Remove(transform);
    }

    protected override void OnEntityHeal(Entity caster, float amount)
    {
    }

    protected override void OnTakeDamage(IAttackable caster, float amount)
    {
    }

    public void OnEntityAttack(Entity caster, float amount)
    {
    }

    public void Attack(Entity caster, float amount)
    {
        if (WeaponController.CanAttack)
        {
            WeaponController.UsingWeapon();
            OnEntityAttack(this, WeaponController.weaponStat.Get(WeaponStatType.DAMAGE));
        }
    }
}
