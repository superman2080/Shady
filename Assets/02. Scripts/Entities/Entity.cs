#nullable enable
#pragma warning disable CS8618
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IDamagable, IAttackable
{
    public Rigidbody2D rb2d { get; private set; }
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public Stat stat;
    protected Collider2D col;
    [SerializeField] public float HP { get; protected set; }

    public Coroutine AttackTimerCor { get; protected set; } = null;

    public WeaponCtrl WeaponController { get; protected set; }

    [SerializeField] public LayerMask attackLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        stat = new Stat();
        stat.InitStat();
        HP = stat.Get(StatType.MAX_HP);
    }

    protected virtual void LateUpdate()
    {
        stat.Update();
    }

    public void TakeDamage(Entity caster, float amount)
    {
        if (amount < 0)
            return;
        OnTakeDamage(caster, amount);
        HP -= amount;
        if (HP <= 0)
        {
            OnEntityDied(caster);
            Destroy(gameObject);
        }
    }

    public void Heal(Entity caster, float amount)
    {
        if (amount < 0)
            return;
        OnEntityHeal(caster, amount);
        HP = Mathf.Clamp(HP + amount, 0, stat.Get(StatType.MAX_HP));
    }

    protected abstract void OnEntityDied(Entity caster);
    protected abstract void OnTakeDamage(Entity caster, float amount);
    protected abstract void OnEntityHeal(Entity caster, float amount);
    protected abstract void OnEntityAttack(Entity caster, float amount);

    public (bool isIn, Collider2D? col)IsInShadow()
    {
        foreach (var shadow in ShadowPool.Instance.GetChildShadowList(false))
        {
            if (col.IsTouching(shadow.col))
                return (true, shadow.col);
        }
        return (false, null);
    }

    public List<GameObject>? FieldOfView(float range, float angle, int layer)
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
        return result;
    }

    public void Attack(Entity caster, float amount)
    {
        if(AttackTimerCor == null && WeaponController.nowWeapon != null)
        {
            AttackTimerCor = StartCoroutine(AttackTimer(stat.Get(StatType.ATTACK_SPEED)));
            WeaponController.UsingWeapon();
            OnEntityAttack(this, stat.Get(StatType.DAMAGE));
        }
    }

    private IEnumerator AttackTimer(float attackSpeed)
    {
        yield return new WaitForSeconds(1f / attackSpeed);
        AttackTimerCor = null;
    }
}
