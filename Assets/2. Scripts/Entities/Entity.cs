using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IDamagable
{
    public Rigidbody2D rb2d { get; private set; }
    [HideInInspector] public Sprite sprite;
    protected Collider2D col;
    protected Stat stat;
    [SerializeField] public float HP { get; protected set; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<Sprite>();
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

    public (bool isIn, Collider2D? col)IsInShadow()
    {
        foreach (var shadow in ShadowPool.Instance.GetChildShadowList(false))
        {
            if (col.IsTouching(shadow.col))
                return (true, shadow.col);
        }
        return (false, null);
    }
}
