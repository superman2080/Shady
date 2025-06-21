using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public Enemy owner;
    public StatBar hpBar;

    // Update is called once per frame
    void Update()
    {
        if(Util.IsVisibleFromCamera(Camera.main, owner.transform))
        {
            transform.position = Camera.main.WorldToScreenPoint(owner.transform.position);
            hpBar.SetStatValue(new SliderValue(owner.HP, owner.entityStat.Get(EntityStatType.MAX_HP), 0));
        }
    }
}
