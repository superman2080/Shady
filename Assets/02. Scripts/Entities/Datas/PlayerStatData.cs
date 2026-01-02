using UnityEngine;
using PlayerNameSpace;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Stats/PlayerStatData")]

public class PlayerStatData : EntityStatData
{
    [Min(0)] public float dashSpeed;
    [Min(0)] public float dashCostPerSec;
    [Min(0)] public float maxDashStamina;
    [Min(0)] public float dashStaminaRegen;
    [Min(0)] public float maxLampStamina;
    [Min(0)] public float lampStaminaRegen;

    public override void ApplyTo(Entity entity)
    {
        base.ApplyTo(entity);
        Player player = (Player)entity;
        player.PlayerStat = new PlayerStat(dashSpeed, maxDashStamina, maxLampStamina, dashStaminaRegen, lampStaminaRegen, dashCostPerSec);
    }
}
