using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyHUDPool : MonoBehaviour
{
    public Transform Pool { get => transform;  }
    public GameObject hudPrefab;

    public EnemyHUD Get(Enemy owner)
    {
        foreach (var child in GetChildList(true))
        {
            if (child.gameObject.activeSelf == false)
            {
                child.gameObject.SetActive(true);
                child.GetComponent<EnemyHUD>().owner = owner;
                return child.GetComponent<EnemyHUD>();
            }
        }

        var result = Instantiate(hudPrefab, Vector3.zero, Quaternion.identity, Pool).GetComponent<EnemyHUD>();
        result.owner = owner;
        return result;
    }

    public void Return(EnemyHUD hud)
    {
        hud.owner = null;
        hud.gameObject.SetActive(false);
    }

    public bool HasHUD(Enemy owner)
    {
        foreach (var child in GetChildList(false))
        {
            if (child.owner == owner)
                return true;
        }
        return false;
    }

    public List<EnemyHUD> GetChildList(bool includeInactive = true)
    {
        return transform.GetComponentsInChildren<EnemyHUD>(includeInactive).ToList();
    }
}
