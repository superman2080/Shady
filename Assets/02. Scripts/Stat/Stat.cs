using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponStatType
{
    ATTACK_SPEED,
    DAMAGE,
    ATTACK_DISTANCE,
}

public enum EntityStatType
{
    MAX_HP,
    MOVE_SPEED,
}

public abstract class Stat<T> where T: Enum
{
    public Dictionary<T, float> defaultStat = new Dictionary<T, float>();
    public Dictionary<T, float> addValue = new Dictionary<T, float>();
    public Dictionary<T, float> multipleValue = new Dictionary<T, float>();
    public Dictionary<T, float> currentValue = new Dictionary<T, float>();

    public void Update()
    {
        UpdateStat();
        InitStat();
    }

    public void InitStat()
    {
        T[] statTypes = (T[])Enum.GetValues(typeof(T));

        foreach (var stat in statTypes)
        {
            addValue[stat] = 0;
            multipleValue[stat] = 1;
        }
    }

    public void UpdateStat()
    {
        T[] statTypes = (T[])Enum.GetValues(typeof(T));
        foreach (var type in statTypes)
        {
            currentValue[type] = (defaultStat[type] + addValue[type]) * multipleValue[type];
        }
    }

    public void SetDefault(T type, float defaultVal)
    {
        defaultStat[type] = defaultVal;
        Update();
    }

    public void Add(T type, float addition)
    {
        addValue[type] += addition;
    }

    public void Multiply(T type, float multiplier)
    {
        multipleValue[type] *= multiplier;
    }

    public float Get(T type)
    {
        return currentValue[type];
    }
}

public class EntityStat: Stat<EntityStatType>
{
    //public Dictionary<EntityStatType, float> defaultStat = new Dictionary<EntityStatType, float>()
    //    {
    //        [EntityStatType.MAX_HP] = 100f,
    //        [EntityStatType.MOVE_SPEED] = 1f,
    //    };

    //public Dictionary<EntityStatType, float> currentValue = new Dictionary<EntityStatType, float>()
    //    {
    //        [EntityStatType.MAX_HP] = 100f,
    //        [EntityStatType.MOVE_SPEED] = 1f,
    //    };

    public EntityStat()
    {
        defaultStat = new Dictionary<EntityStatType, float>()
        {
            [EntityStatType.MAX_HP] = 100f,
            [EntityStatType.MOVE_SPEED] = 1f,
        };

        currentValue = new Dictionary<EntityStatType, float>()
        {
            [EntityStatType.MAX_HP] = 100f,
            [EntityStatType.MOVE_SPEED] = 1f,
        };

        InitStat();
        UpdateStat();
    }
}

public class WeaponStat: Stat<WeaponStatType>
{
    public WeaponStat()
    {
        defaultStat = new Dictionary<WeaponStatType, float>()
        {
            [WeaponStatType.ATTACK_DISTANCE] = 2f,
            [WeaponStatType.ATTACK_SPEED] = 1f,
            [WeaponStatType.DAMAGE] = 25f,
        };

        currentValue = new Dictionary<WeaponStatType, float>()
        {
            [WeaponStatType.ATTACK_DISTANCE] = 2f,
            [WeaponStatType.ATTACK_SPEED] = 1f,
            [WeaponStatType.DAMAGE] = 25f,
        };

        InitStat();
        UpdateStat();
    }
}