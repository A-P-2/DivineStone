using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : MonoBehaviour
{
    [Header("Main Info")]
    public string skillName;
    [TextArea]
    public string description;
    public int skillLevel;
    public Type type;
    public bool unlocked;
    [Header("Requirements")]
    public ActiveSkill[] requiredActiveSkills;
    public PassiveSkill[] requiredPassiveSkills;
    [Header("Main Parameters")]
    public int mainPhysDmgIncrease;
    public int mainMagicDmgIncrease;
    [Range(0, 100)]
    public int mainPhysDefIncrease;
    [Range(0, 100)]
    public int mainMagicDefIncrease;
    [Range(0, 100)]
    public int escapeChanceIncrease;
    [Range(0, 100)]
    public int unlockChestChanceIncrease;
    [Header("To Effects")]
    public int addDmgToPhysDmgPerMove;
    public int addDmgToMagicDmgPerMove;
    [Header("Special Skills")]
    public bool unlockSpecialWarriorSkill;
    public bool unlockSpecialWizardSkill;
    public bool unlockSpecialThiefSkill;

    public enum Type
    {
        wizard,
        warrior,
        thief
    }

    public string getDescription(bool fullDescription = true)
    {
        string description = "";

        if (mainPhysDmgIncrease > 0)
        {
            description += "Повышение базового физ. урона: " + mainPhysDmgIncrease + "\n";
        }

        if (mainMagicDmgIncrease > 0)
        {
            description += "Повышение базового маг. урона: " + mainMagicDmgIncrease + "\n";
        }

        if (mainPhysDefIncrease > 0)
        {
            description += "Повышение базовой физ. защиты: " + mainPhysDefIncrease + "%\n";
        }

        if (mainMagicDefIncrease > 0)
        {
            description += "Повышение базовой маг. защиты: " + mainMagicDefIncrease + "%\n";
        }

        if (escapeChanceIncrease > 0)
        {
            description += "Повышение шанса на побег: " + escapeChanceIncrease + "%\n";
        }

        if (unlockChestChanceIncrease > 0)
        {
            description += "Повышение шанса на взлом сундука: " + unlockChestChanceIncrease + "%\n";
        }

        description += this.description;

        if (fullDescription)
        {
            if (requiredActiveSkills.Length > 0 || requiredPassiveSkills.Length > 0)
            {
                description += "\n\nТребуемые навыки:";
                foreach (ActiveSkill skill in (requiredActiveSkills))
                    description += "\n- " + skill.skillName;
                foreach (PassiveSkill skill in (requiredPassiveSkills))
                    description += "\n- " + skill.skillName;
            }
        }

        return description;
    }
    public bool ableToUnlock()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        bool unlockable = true;

        if (player.skillPoints < 1)
        {
            unlockable = false;
        }

        if ((player.strength < skillLevel) && (type == Type.warrior))
        {
            unlockable = false;
        }

        if ((player.intelligence < skillLevel) && (type == Type.wizard))
        {
            unlockable = false;
        }

        if ((player.agility < skillLevel) && (type == Type.thief))
        {
            unlockable = false;
        }

        foreach (ActiveSkill skill in requiredActiveSkills)
        {
            if (skill.unlocked == false)
                unlockable = false;
        }

        foreach (PassiveSkill skill in requiredPassiveSkills)
        {
            if (skill.unlocked == false)
                unlockable = false;
        }

        return unlockable;
    }

    public void unlockSkill()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        unlocked = true;
        player.skillPoints--;
        player.mainPhysDmg += mainPhysDmgIncrease;
        player.mainMagicDmg += mainMagicDmgIncrease;
        player.mainPhysDef += mainPhysDefIncrease;
        player.mainMagicDef += mainMagicDefIncrease;
        player.escapeChance += escapeChanceIncrease;
        player.unlockChestChance += unlockChestChanceIncrease;

        player.physDmgBoost += addDmgToPhysDmgPerMove;
        player.magicDmgBoost += addDmgToMagicDmgPerMove;

        if (unlockSpecialWarriorSkill) player.specialWarriorSkill = true;
        if (unlockSpecialWizardSkill) player.specialMageSkill = true;
        if (unlockSpecialThiefSkill) player.specialTheifSkill = true;
    }
}