using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShadowPool : Singleton<ShadowPool>
{
    public Transform Pool 
    { 
        get 
        {
            return transform;
        } 
    }

    
    [SerializeField] private GameObject shadowPrefab;

    public bool IsShadowExisting(Shadow comp)
    {
        return GetChildShadowList(true).Exists((s) => s == comp && s.lightSource != null);
    }

    //public bool IsExistingUsableShadow()
    //{
    //    return GetChildShadowList(true).Find((n) => n.gameObject.activeSelf == false);
    //}


    /*
        Getting all children shadows
        
        Param1: Finding shadows but is active or not
    */


    public List<Shadow> GetChildShadowList(bool includeInactive = true)
    {
        return transform.GetComponentsInChildren<Shadow>(includeInactive).ToList();
    }

    public Shadow InstantiateShadow(ShadowCaster lightSource)
    {
        foreach (var shadow in GetChildShadowList(true))
        {
            if(shadow.gameObject.activeSelf == false)
            {
                shadow.gameObject.SetActive(true);
                shadow.lightSource = lightSource;
                return shadow;
            }
        }
        var result = Instantiate(shadowPrefab, Vector3.zero, Quaternion.identity, Pool).GetComponent<Shadow>();
        result.lightSource = lightSource;
        return result;
    }
}
