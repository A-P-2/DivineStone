using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Main Stats")]
    public int strength;
    public int intelligence;
    public int agility;
    [Header("Main Parameters")]
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    [Header("Leveling")]
    public int level;
    public int statPoints;
    public int skillPoints;
    public int currentExp;
    public int reqExpAmt;
    public int expIncreasedAmt;
    [Header("For Battle")]
    public int mainPhysDmg;
    public int mainMagicDmg;
    [Range(0, 100)]
    public int mainPhysDef;
    [Range(0, 100)]
    public int mainMagicDef;
    [Range(0, 100)]
    public int escapeChance;
    [Range(0, 100)]
    public int unlockChestChance;
    public float physDmgBoost;
    public float magicDmgBoost;
    [Header("Special Skills")]
    public bool specialWarriorSkill;
    public bool specialMageSkill;
    public bool specialTheifSkill;
    [Header("For Potions")]
    public bool ignorePhysDmg;
    public bool ignoreMagicDmg;

    public void increaseSTR()
    {
        strength++;
        maxHP += 20;
        currentHP = maxHP;
        statPoints--;
    }

    public void increaseINT()
    {
        intelligence++;
        maxMP += 20;
        currentMP = maxMP;
        statPoints--;
    }

    public void increaseAGI()
    {
        agility++;
        statPoints--;
    }

    public void changeHP(int HPAmount)
    {
        currentHP += HPAmount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        if (currentHP <= 0)
        {
            FindObjectOfType<UIDeathScrean>().OpenDeathScrean();
        }
    }

    public void changeMP(int MPAmount)
    {
        currentMP += MPAmount;
        if (currentMP > maxMP)
        {
            currentMP = maxMP;
        }
        if (currentMP < 0)
        {
            Debug.LogError("[ОШИБКА. Количество маны = " + currentMP + ", что меньше нуля.]");
        }
    }

    public void addExp(int expAmount)
    {
        currentExp += expAmount;
        while (currentExp >= reqExpAmt)
        {
            FindObjectOfType<UIPopupMessage>().ShowSideHint("Новый уровень!");
            level++;
            statPoints++;
            skillPoints++;
            reqExpAmt += expIncreasedAmt;
            expIncreasedAmt = (int)(1.05 * expIncreasedAmt);
        }
    }

    public PlayerStats ShallowCopy()
    {
        return (PlayerStats) MemberwiseClone();
    }
}
