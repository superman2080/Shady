using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public Transform hudTr;
    public Slider hpBar;
    public Image doubtMark;
    private Entity entity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entity = gameObject.GetComponent<Entity>();
        doubtMark.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        hudTr.eulerAngles = Vector3.zero;
        hudTr.transform.position = entity.transform.position;
        hpBar.value = entity.HP / entity.entityStat.Get(EntityStatType.MAX_HP);
    }
}
