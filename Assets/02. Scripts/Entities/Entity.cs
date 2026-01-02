using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnEntityDiedEventHandler(IAttackable caster);
public delegate void OnEntityTakeDamageEventHandler(IAttackable caster, float amount);
public delegate void OnEntityHealEventHandler(Entity entity, float amount);

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IDamagable
{
    public bool canBehavior = true;
    #region Default components(Rigidbody2D, SpriteRenderer, Collider2D...)
    public Rigidbody2D rb2d { get; private set; }
    [HideInInspector] public SpriteRenderer spriteRenderer;
    protected Collider2D col;
    #endregion

    #region Stat
    public StatData entityStatData;
    public DefaultStat Stat { get; set; }
    public Resource HP => Stat.HP;
    #endregion

    #region Events
    // 누구에 의해 죽었는지
    public event Action<IAttackable> OnEntityDied;
    // 대미지 준 객체, 대미지 Amount
    public event Action<IAttackable, float> OnTakeDamage;

    public event OnEntityHealEventHandler OnEntityHeal;

    #endregion

    private StatusEffectHandler effectHandler;
    public StatusEffectHandler EffectHandler => effectHandler;

    protected virtual void Awake()
    {
        entityStatData?.ApplyTo(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        effectHandler = new StatusEffectHandler(this);
        col = gameObject.GetComponent<Collider2D>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        Stat.InitStat();
    }

    protected virtual void LateUpdate()
    {
        Stat.Update();
        effectHandler.UpdateEffect();
    }

    public void TakeDamage(IAttackable caster, float amount)
    {
        if (amount < 0)
            return;
        OnTakeDamage?.Invoke(caster, amount);
        Stat.HP.Subtract(amount);
        if (HP <= 0)
        {
            OnEntityDied?.Invoke(caster);
            Destroy(gameObject);
        }
    }

    public void Heal(Entity caster, float amount)
    {
        if (amount < 0)
            return;
        OnEntityHeal?.Invoke(caster, amount);
        Stat.HP.Add(amount);
    }

    public bool IsInShadow(out Shadow shadow)
    {
        shadow = null;
        foreach (var s in ShadowPool.Instance.GetChildShadowList(false))
        {
            if (col.IsTouching(s.col))
            {
                shadow = s;
                return true;
            }
        }
        return false;
    }
    public bool IsInShadow()
    {
        foreach (var s in ShadowPool.Instance.GetChildShadowList(false))
        {
            if (col.IsTouching(s.col))
            {
                return true;
            }
        }
        return false;
    }

    public List<GameObject> FieldOfView(float range, float angle, int layer)
    {
        Vector2 origin = transform.position;
        List<GameObject> result = new List<GameObject>();
        Collider2D[] targets = Physics2D.OverlapCircleAll(origin, range, layer);
        if (targets.Length <= 0)
            return null;
        else
        {
            foreach (var target in targets)
            {
                Vector2 targetPos = target.transform.position;
                Vector2 dir = (targetPos - origin).normalized;
                float theta = Mathf.Acos(Vector3.Dot(transform.right, dir)) * Mathf.Rad2Deg;

                if (Physics2D.Raycast(origin, dir, range, layer).collider == target && theta <= angle)
                {
                    result.Add(target.gameObject);
                }
            }
        }
        return result.OrderBy((s) => (origin - (Vector2)s.transform.position).sqrMagnitude).ToList();
    }

    public List<T> FieldOfView<T>(float range, float angle, int layer) where T : MonoBehaviour
    {
        Vector2 origin = transform.position;
        List<T> result = new List<T>();
        Collider2D[] targets = Physics2D.OverlapCircleAll(origin, range, layer);
        if (targets.Length <= 0)
            return null;
        else
        {
            foreach (var target in targets)
            {
                if (target.TryGetComponent(out T obj) == true)
                {
                    Vector2 targetPos = target.transform.position;
                    Vector2 dir = (targetPos - origin).normalized;
                    float theta = Mathf.Acos(Vector3.Dot(transform.right, dir)) * Mathf.Rad2Deg;

                    if (Physics2D.Raycast(origin, dir, range, layer).collider == target && theta <= angle)
                    {
                        result.Add(target.GetComponent<T>());
                    }
                }
            }
        }
        return result.OrderBy((s) => (origin - (Vector2)s.transform.position).sqrMagnitude).ToList();
    }
}

public abstract class Entity<T>: Entity where T : Entity<T>
{
    public StateMachine<T> StateMachine => stateMachine;
    protected StateMachine<T> stateMachine;

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new StateMachine<T>((T)this);
    }

    protected virtual void Update()
    {
        stateMachine?.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    protected abstract void RegisterStates();
    protected abstract void RegisterConditions();
}