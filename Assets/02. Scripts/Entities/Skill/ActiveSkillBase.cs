using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class ActiveSkillBase
{
    #region Skill Data
    public SkillData SkillData
    {
        get
        {
            if (skillData == null)
            {
                skillData = Resources.Load<SkillData>(SkillDataPath);
                if (skillData == null)
                {
                    Debug.LogError($"SkillData is null. Path: {SkillDataPath}");
                    skillData = ScriptableObject.CreateInstance<SkillData>();
                }
            }
            return skillData;
        }
    }
    private SkillData skillData;
    protected abstract string SkillDataPath { get; }
    protected abstract SkillType SkillType { get; }
    #endregion

    public float CooldownProgress { get; private set; } = 0f;
    private int cooldownCorID;
    private int executeCorId;

    public Action OnStart;
    public Action OnUpdate;
    public Action OnFinish;

    public virtual void Activate(IAttackable entity)
    {
        OnStart?.Invoke();
        switch (SkillType)
        {
            case SkillType.ONE_SHOT:
                Execute(entity);
                cooldownCorID = CoroutineRunner.Start(CoolDownCoroutine(SkillData.cooldownTime));
                break;
            case SkillType.CASTING:
                executeCorId = CoroutineRunner.Start(Casting(entity, SkillData.castTime));
                break;
            case SkillType.CHANNELING:
                executeCorId = CoroutineRunner.Start(Channeling(entity, SkillData.castTime));
                break;
            default:
                break;
        }
    }

    protected virtual void Start(IAttackable entity)
    {

        if (SkillType == SkillType.CHANNELING)
        {
            //player.isChanneling = true;
        }

    }
    protected virtual void Execute(IAttackable entity) { }

    protected virtual void Finish(IAttackable entity)
    {
        if (SkillType == SkillType.CHANNELING)
        {
            //player.isChanneling = false;
        }
        cooldownCorID = CoroutineRunner.Start(CoolDownCoroutine(SkillData.cooldownTime));
    }

    private IEnumerator CoolDownCoroutine(float cooldownTime)
    {
        CooldownProgress = 0;
        for (float elapsedTime = 0; elapsedTime < cooldownTime; elapsedTime += Time.deltaTime)
        {
            CooldownProgress = Mathf.Clamp01(elapsedTime / cooldownTime);
            yield return null;
        }
        CooldownProgress = 1;
    }

    private IEnumerator Casting(IAttackable entity, float castingTime)
    {
        Start(entity);
        for (float elapsedTime = 0; elapsedTime < castingTime; elapsedTime += Time.deltaTime)
        {
            Execute(entity);
            yield return null;
        }
        Finish(entity);
    }

    private IEnumerator Channeling(IAttackable entity, float castingTime)
    {
        Start(entity);
        for (float elapsedTime = 0; elapsedTime < castingTime; elapsedTime += Time.deltaTime)
        {
            //if (entity.HasEffect(StatusEffectType.HARD_CC))
            //{
            //    Finish(entity);
            //    yield break;
            //}
            Execute(entity);
            yield return null;
        }
        Finish(entity);
    }

    public bool CanActivateSkill() => CoroutineRunner.Instance.IsCoroutineRunning(cooldownCorID) == false;

    protected IDamagable[] GetEnemiesInRange(IAttackable entity, Vector2 castPos, float range)
    {

        var enemies = Physics2D.OverlapCircleAll(castPos, range, 1 << entity.AttackLayer);
        if (enemies.Length > 0)
        {
            var result = new List<IDamagable>();
            for (int i = 0; i < enemies.Length; i++)
            {
                result.Add(enemies[i].GetComponent<IDamagable>());
            }
            return result.ToArray();
        }
        else
            return null;
    }

    protected void CancelSkill()
    {
        CoroutineRunner.Stop(executeCorId);
    }

    protected void InitializeCooldown()
    {
        CoroutineRunner.Stop(cooldownCorID);
        CooldownProgress = 1;
    }
}