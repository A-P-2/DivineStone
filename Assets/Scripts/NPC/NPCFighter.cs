using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCFighter : MonoBehaviour
{
    public int backgroundNumber = 0;
    [HideInInspector] public int maxHP;
    public int HP = 1;
    public int level = 1;
    public int exp = 0;

    [Header("Attack stat")]
    public int minPhysDmg;
    public int maxPhysDmg;
    public int minMagicDmg;
    public int maxMagicDmg;
    [Range(0, 100)]
    public int critChance;

    [Header("Defense stat")]
    [Range(0, 100)]
    public int physDmgResist;
    [Range(0, 100)]
    public int magicDmgResist;

    [Header("Skill list")]
    public int agility = 1;
    [Range(0, 100)]
    public int canBoostPhys;
    [Range(0, 100)]
    public int canBoostMagic;
    public int canHeal = 0;

    [Header("Escape Point")]
    public Vector2 escapePoint;

    private void Start()
    {
        maxHP = HP;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(escapePoint.x, escapePoint.y, 0), 0.5f);
    }

    public void GetDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
            DataTransfer.EndFight();
    }

    public NPCFighter ShallowCopy()
    {
        return (NPCFighter)MemberwiseClone();
    }
}
