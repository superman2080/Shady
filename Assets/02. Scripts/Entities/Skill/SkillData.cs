using UnityEngine;
public enum SkillType
{
    ONE_SHOT,
    CASTING,
    CHANNELING,
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName = "Skill Name";
    public string description = "Skill Description";
    public Sprite icon;

    [Header("비용 및 쿨다운")]
    public float cooldownTime = 5f;

    [Header("캐스팅")]
    public float castRange = 5f;
    public float castTime = 0f;

    [Header("이펙트")]
    public AudioClip castSound;
}
