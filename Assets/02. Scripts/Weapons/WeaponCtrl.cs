using System;
using UnityEngine;

public class WeaponCtrl
{
    public IAttackable User { get; private set; }
    public IWeapon CurrentWeapon { get; private set; }
    public bool CanAttack => CurrentWeapon != null && canAttack;
    public float RemainTime => attackCooldownTimer?.TimeLeft ?? 0f;

    public Action onUpdate;
    public Action onComplete;

    private bool canAttack = true;
    private Timer attackCooldownTimer;


    public WeaponCtrl(IAttackable user)
    {
        User = user;
        onComplete += () => { 
            OnCooldownComplete();
        };
    }

    public void SetWeapon(IWeapon weapon)
    {
        if (CurrentWeapon == weapon) return;

        CurrentWeapon?.UnequipWeapon(User);
        ResetCooldown();

        CurrentWeapon = weapon;
        CurrentWeapon?.EquipWeapon(User);
    }

    public void UnequipWeapon()
    {
        CurrentWeapon?.UnequipWeapon(User);
        attackCooldownTimer?.Reset();

        CurrentWeapon = null;
        canAttack = true;
    }

    public bool TryUsingWeapon()
    {
        if (!CanAttack) return false;

        canAttack = false;
        CurrentWeapon.Using(User);
        StartCooldown();

        return true;
    }

    private void StartCooldown()
    {
        float attackSpeed = User?.WeaponStat.Get(WeaponStatType.ATTACK_SPEED) ?? 1f;

        if (attackSpeed <= 0f)
        {
            canAttack = true;
            return;
        }

        float cooldown = 1f / attackSpeed;
        attackCooldownTimer = new Timer(cooldown, onComplete, onUpdate);
    }

    private void OnCooldownComplete()
    {
        canAttack = true;
    }

    public void ResetCooldown()
    {
        attackCooldownTimer?.Reset();
        canAttack = true;
    }
}