using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    public StatBar hpBar;
    public StatBar dashBar;
    public StatBar lanternBar;


    public Image lanternImage;
    public Sprite lanternOnSprite;
    public Sprite lanternOffSprite;
    private PlayerCtrl player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBar();
        UpdateLanternState();
    }

    private void UpdateBar()
    {
        if (player != null)
        {
            hpBar.SetStatValue(new SliderValue(player.HP, player.entityStat.Get(EntityStatType.MAX_HP), 0));
            dashBar.SetStatValue(player.dashValue);
            lanternBar.SetStatValue(player.lanternValue);
        }
    }

    private void UpdateLanternState()
    {
        if(player != null)
        {
            lanternImage.sprite = player.lantern != null ? lanternOnSprite : lanternOffSprite;
        }
    }
}
