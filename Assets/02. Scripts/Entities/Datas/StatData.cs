using System.Collections.Generic;
using UnityEngine;
using PlayerNameSpace;

public abstract class StatData : ScriptableObject
{
    [HideInInspector] public Entity owner;
    public abstract void ApplyTo(Entity entity);
}


[CreateAssetMenu(fileName = "EntityStatData", menuName = "Stats/EntityStatData")]
public class EntityStatData : StatData
{
    [Min(0)] public float maxHp;
    [Min(0)] public float hpRegen;
    [Min(0)] public float moveSpeed;
    public override void ApplyTo(Entity entity)
    {
        owner = entity;
        entity.Stat = new DefaultStat(maxHp, hpRegen, moveSpeed);
    }
}