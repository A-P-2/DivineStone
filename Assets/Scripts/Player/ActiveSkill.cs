using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
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
    public bool ignoreDef;
    public int physicalDamage;
    public int magicalDamage;
    public int HPAlteration;
    public int MPAlteration;
    [Range(0, 100)]
    public int physDmgIncrease;
    [Range(0, 100)]
    public int magicDmgIncrease;
    [Header("Effects")]
    public int moveNumber;
    public string designation;
    public int physDmgPerMove;
    public int magicDmgPerMove;
    [Range(0, 100)]
    public int physDmgDefDecrease;
    [Range(0, 100)]
    public int magicDmgDefDecrease;


    public enum Type
    {
        wizard,
        warrior,
        thief
    }

    public string getDescription(bool fullDescription = true)
    {
        string description = "";

        if (ignoreDef == true)
        {
            description += "Игнорирует защиту\n";
        }

        if (physicalDamage > 0)
        {
            description += "Физический урон: " + physicalDamage + "\n";
        }

        if (magicalDamage > 0)
        {
            description += "Магический урон: " + magicalDamage + "\n";
        }

        if (HPAlteration > 0)
        {
            description += "Восстановление здоровья: " + HPAlteration + "\n";
        }

        if (HPAlteration < 0)
        {
            description += "Трата здоровья: " + -HPAlteration + "\n";
        }

        if (MPAlteration > 0)
        {
            description += "Восстановление магии: " + MPAlteration + "\n";
        }

        if (MPAlteration < 0)
        {
            description += "Трата магии: " + -MPAlteration + "\n";
        }

        if (physDmgIncrease > 0)
        {
            description += "Повышение физ. урона на " + physDmgIncrease + "%\n";
        }

        if (magicDmgIncrease > 0)
        {
            description += "Повышение маг. урона на " + magicDmgIncrease + "%\n";
        }

        if (moveNumber > 0)
        {
            description += "\nНакладывает на врага эффект \"" + designation + "\":\n";

            description += "- Длительность эффекта: " + moveNumber + "\n";

            if (physDmgPerMove > 0)
            {
                description += "- Физ. урон за ход: " + physDmgPerMove + "\n";
            }

            if (magicDmgPerMove > 0)
            {
                description += "- Маг. урон за ход: " + magicDmgPerMove + "\n";
            }

            if (physDmgDefDecrease > 0)
            {
                description += "- Снижение физ. защиты: " + physDmgDefDecrease + "%\n";
            }

            if (magicDmgDefDecrease > 0)
            {
                description += "- Снижение маг. защиты: " + magicDmgDefDecrease + "%\n";
            }
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
    }

    public bool ableToUse()
    {
        PlayerStats player = DataTransfer.playerStats;
        bool usable = true;

        if ((HPAlteration != 0) || (MPAlteration != 0))
        {
            if (player.currentHP <= -HPAlteration)
            {
                usable = false;
            }

            if (player.currentMP <= -MPAlteration)
            {
                usable = false;
            }
        }

        return usable;
    }

    public ActiveSkill ShallowCopy()
    {
        return (ActiveSkill)MemberwiseClone();
    }
}
