using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class StatusEffectHandler
{
    public Entity owner;
    public List<StatusEffect> StatusEffects => statusEffects;
    private List<StatusEffect> statusEffects = new List<StatusEffect>();
    public Action<StatusEffect> OnEnter;
    public Action<StatusEffect> OnExecute;
    public Action<StatusEffect> OnExit;

    public StatusEffectHandler(Entity owner)
    {
        this.owner = owner;
    }

    // 정렬 필요 플래그 필드
    private bool isDirty = true;

    public bool HasHardCC => HasEffect(StatusEffectType.HARD_CC);

    public bool HasNormalCC => HasEffect(StatusEffectType.NORMAL_CC);

    public bool HasBuff => HasEffect(StatusEffectType.BUFF);


    public void AddEffect(StatusEffect effect)
    {
        if (HasEffect<Unstoppable>() &&
            ((effect.Type == StatusEffectType.HARD_CC) || (effect.Type == StatusEffectType.NORMAL_CC)))
            return;

        OnEnter?.Invoke(effect);
        statusEffects.Add(effect);
        effect.OnEnter(owner);

        isDirty = true;
    }

    public bool HasEffect<E>() where E : StatusEffect
    {
        return !(statusEffects.Find(n => n is E) is null);
    }

    public bool HasEffect(StatusEffectType type)
    {
        return !(statusEffects.Find(n => n.Type == type) is null);
    }


    public void UpdateEffect()
    {
        if (statusEffects.Count <= 0)
            return;

        if (isDirty)
        {
            SortEffects();
            isDirty = false;
        }

        List<StatusEffect> delEffect = new List<StatusEffect>();
        foreach (var eff in statusEffects)
        {
            if (!HasEffect<Unstoppable>()
                || (HasEffect<Unstoppable>() && eff.Type == StatusEffectType.BUFF))
                eff.OnExecute(owner);

            eff.Duration -= Time.deltaTime;
            if (eff.Duration <= 0)
            {
                OnExit(eff);
                eff.OnExit(owner);
                delEffect.Add(eff);
            }
        }

        statusEffects.RemoveAll(eff => delEffect.Contains(eff));
    }

    private void SortEffects()
    {
        statusEffects = statusEffects.OrderBy(eff => eff.Type).
            ThenBy(effect => effect.Duration).ToList();
    }
}
