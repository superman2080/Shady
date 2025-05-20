using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Dagger : IWeapon
{
    public LayerMask AttackLayer { get; private set; }

    public void InitWeapon(Entity user)
    {
        AttackLayer = user.attackLayer;
        user.stat.SetDefault(StatType.ATTACK_SPEED, 2);
        user.stat.SetDefault(StatType.ATTACK_DISTANCE, 1);
        user.stat.SetDefault(StatType.DAMAGE, 50);
    }

    public void Using(Entity user)
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<GameObject> objs = user.FieldOfView(user.stat.Get(StatType.ATTACK_DISTANCE), 90, AttackLayer);
            if( objs == null)
            {
                return;
            }    

            foreach (var obj in objs)
            {
                if(obj.TryGetComponent(out Entity entity))
                {
                    entity.TakeDamage(user, user.stat.Get(StatType.DAMAGE));
                    Debug.LogWarning(entity.HP);
                }
            }
        }
    }
}
