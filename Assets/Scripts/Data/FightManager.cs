using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightManager : MonoBehaviour
{
    public class FighterInfo
    {
        public int physDmgBoost = 0;
        public int magDmgBoost = 0;
        public string effectName = "Нет";
        public int effectLength = 0;
        public int effectPhysDmg = 0;
        public int effectMagDmg = 0;
        public int physDmgDefDefault = 0;
        public int magDmgDefDefault = 0;

        //Только для игрока
        public bool physBlock = false;
        public bool magBlock = false;

        public FighterInfo()
        {
            physDmgBoost = 0;
            magDmgBoost = 0;
            effectName = "Нет";
            effectLength = 0;
            effectPhysDmg = 0;
            effectMagDmg = 0;

            physBlock = false;
            magBlock = false;
        }
    }

    [HideInInspector] public FighterInfo player = new FighterInfo();
    [HideInInspector] public FighterInfo enemy = new FighterInfo();
    [HideInInspector] public int turnNumber = 0;

    private int sameTurn = 0;
    [HideInInspector] public string currentTurnName = "";
    [HideInInspector] public string nextTurnName = "";

    private bool specialWarriorSkillActive = false;
    private bool specialMageSkillActive = false;
    private bool specialTheifSkillActive = false;

    private UIFight uiFight;

    private void Start()
    {
        if (DataTransfer.status == DataTransfer.Status.fight) SoundManager.PlayBGMusic();
        else SoundManager.PlaySpecialMusic("BossFight");

        uiFight = FindObjectOfType<UIFight>();

        ChooseNextTurn();
        if (nextTurnName == "Отшельник") CheckSpecialTheifSkill();

        NextTurn();
    }

    public void SimpleAttack(bool physAttack, bool targetIsEnemy = true, bool ignorResist = false)
    {
        if (targetIsEnemy)
        {
            int damage;
            if (physAttack)
            {
                SoundManager.GetSound("physAttack").audioSource.Play();

                damage = DataTransfer.playerStats.mainPhysDmg;
                if (DataTransfer.inventory.usedWeaponID != "")
                    damage += FindObjectOfType<ItemsList>().getWeaponInfo(DataTransfer.inventory.usedWeaponID).x;

                if (player.physDmgBoost > 0)
                {
                    damage += damage * player.physDmgBoost / 100;
                    player.physDmgBoost = 0;
                }

                if (!ignorResist)
                {
                    damage -= damage * DataTransfer.npcFighter.physDmgResist / 100;
                }

                if (damage < 0) damage = 0;

                uiFight.AddInfoToActionText("Отшельник нанёс " + damage + " ед. физического урона персонажу \"" + DataTransfer.enemyName + "\"");
            }
            else
            {
                SoundManager.GetSound("magAttack").audioSource.Play();

                damage = DataTransfer.playerStats.mainMagicDmg;
                if (DataTransfer.inventory.usedWeaponID != "")
                    damage += FindObjectOfType<ItemsList>().getWeaponInfo(DataTransfer.inventory.usedWeaponID).y;

                if (player.magDmgBoost > 0)
                {
                    damage += damage * player.magDmgBoost / 100;
                    player.magDmgBoost = 0;
                }

                if (!ignorResist)
                {
                    damage -= damage * DataTransfer.npcFighter.magicDmgResist / 100;
                }

                if (damage < 0) damage = 0;

                uiFight.AddInfoToActionText("Отшельник нанёс " + damage + " ед. магического урона персонажу \"" + DataTransfer.enemyName + "\"");
            }
            player.physBlock = false;
            player.magBlock = false;
            DataTransfer.npcFighter.GetDamage(damage);

            NextTurn();
        }
        else
        {
            int damage;
            if (physAttack)
            {
                SoundManager.GetSound("physAttack").audioSource.Play();

                damage = Random.Range(DataTransfer.npcFighter.minPhysDmg, DataTransfer.npcFighter.maxPhysDmg + 1);
                if (Random.Range(1, 101) <= DataTransfer.npcFighter.critChance) damage *= 2;

                if (enemy.physDmgBoost > 0)
                {
                    damage += damage * enemy.physDmgBoost / 100;
                    enemy.physDmgBoost = 0;
                }

                if (!ignorResist)
                {
                    int resist = DataTransfer.playerStats.mainPhysDef + ((DataTransfer.inventory.usedArmorID != "")
                        ? FindObjectOfType<ItemsList>().getArmorInfo(DataTransfer.inventory.usedArmorID).x : 0);

                    if (resist > 95) resist = 95;
                    damage -= damage * resist / 100;
                }

                if (player.physBlock)
                {
                    damage /= 2;
                    player.physBlock = false;
                }

                if (DataTransfer.playerStats.ignorePhysDmg)
                {
                    damage = 0;
                    DataTransfer.playerStats.ignorePhysDmg = false;
                }

                if (damage < 0) damage = 0;

                uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" нанёс " + damage + " ед. физического урона отшельнику");
            }
            else
            {
                SoundManager.GetSound("magAttack").audioSource.Play();

                damage = Random.Range(DataTransfer.npcFighter.minMagicDmg, DataTransfer.npcFighter.maxMagicDmg + 1);

                if (enemy.magDmgBoost > 0)
                {
                    damage += damage * enemy.magDmgBoost / 100;
                    enemy.magDmgBoost = 0;
                }

                if (!ignorResist)
                {
                    int resist = DataTransfer.playerStats.mainMagicDef + ((DataTransfer.inventory.usedArmorID != "")
                        ? FindObjectOfType<ItemsList>().getArmorInfo(DataTransfer.inventory.usedArmorID).y : 0);

                    if (resist > 95) resist = 95;
                    damage -= damage * resist / 100;
                }

                if (player.magBlock)
                {
                    damage /= 2;
                    player.magBlock = false;
                }

                if (DataTransfer.playerStats.ignoreMagicDmg)
                {
                    damage = 0;
                    DataTransfer.playerStats.ignoreMagicDmg = false;
                }

                if (damage < 0) damage = 0;

                uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" нанёс " + damage + " ед. магического урона отшельнику");
            }
            if (specialTheifSkillActive)
            {
                specialTheifSkillActive = false;
                if (Random.Range(1, 101) <= 20)
                {
                    damage = 0;
                    uiFight.AddInfoToActionText("Отшельник смог увернуться от удара в самый последний момент!");
                }
            }

            DataTransfer.playerStats.changeHP(-damage);
            if (player.effectName == "Проклятая тень") HitEnemy(damage, physAttack, true);

            NextTurn();
        }
    }

    public void PutBlock(bool physBlock)
    {
        if (physBlock)
        {
            SoundManager.GetSound("physBlock").audioSource.Play();

            player.physBlock = true;
            player.magBlock = false;
            uiFight.AddInfoToActionText("Отшельник выставил физический блок");
        }
        else
        {
            SoundManager.GetSound("magBlock").audioSource.Play();

            player.physBlock = false;
            player.magBlock = true;
            uiFight.AddInfoToActionText("Отшельник выставил магический блок");
        }
        NextTurn();
    }

    public void EquipWeapon(string ID)
    {
        uiFight.CloseUIInventory();
        uiFight.AddInfoToActionText("Отшельник неожиданно решил поменять оружие!");
        DataTransfer.inventory.equipWeapon(ID);
        NextTurn();
    }

    public void EquipArmor(string ID)
    {
        uiFight.CloseUIInventory();
        uiFight.AddInfoToActionText("Отшельник решил переодеться прямо на поле боя! Хорошо, что его противник никуда не торопится");
        DataTransfer.inventory.equipArmor(ID);
        NextTurn();
    }

    public void UseConsumable(string ID)
    {
        uiFight.CloseUIInventory();
        uiFight.AddInfoToActionText("Отшельник принял \"" + FindObjectOfType<ItemsList>().getCommonInfo(ID).itemName + "\" и не поделился!");
        NextTurn();
    }

    private void HitEnemy(int damage, bool physAttack, bool ignorResist = false, bool fromEffect = false)
    {
        if (physAttack)
        {
            if (!ignorResist)
            {
                damage -= damage * DataTransfer.npcFighter.physDmgResist / 100;
            }

            if (fromEffect) damage += (int)DataTransfer.playerStats.physDmgBoost;

            uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" получил " 
                + damage + " ед. физического урона" + ((fromEffect) ? (" из-за эффекта \"" + enemy.effectName + "\"") : ""));
        }
        else
        {
            if (!ignorResist)
            {
                damage -= damage * DataTransfer.npcFighter.magicDmgResist / 100;
            }

            if (fromEffect) damage += (int)DataTransfer.playerStats.physDmgBoost;

            uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" получил "
                + damage + " ед. магического урона" + ((fromEffect) ? (" из-за эффекта \"" + enemy.effectName + "\"") : ""));
        }
        DataTransfer.npcFighter.GetDamage(damage);
    }

    private void SetEffect(bool toEnemy, string _effectName, int _effectLength, int _effectPhysDmg, int _effectMagDmg, 
        int _physDmgDefDecrease, int _magDmgDefDecrease)
    {
        if (toEnemy)
        {
            uiFight.AddInfoToActionText("На персонажа \"" + DataTransfer.enemyName + "\" накладывается эффект \"" + _effectName + "\"");

            enemy.effectName = _effectName;
            enemy.effectLength = _effectLength;
            enemy.effectPhysDmg = _effectPhysDmg;
            enemy.effectMagDmg = _effectMagDmg;
            enemy.physDmgDefDefault = DataTransfer.npcFighter.physDmgResist;
            enemy.magDmgDefDefault = DataTransfer.npcFighter.magicDmgResist;

            DataTransfer.npcFighter.physDmgResist -= _physDmgDefDecrease;
            if (DataTransfer.npcFighter.physDmgResist < 0) DataTransfer.npcFighter.physDmgResist = 0;

            DataTransfer.npcFighter.magicDmgResist -= _magDmgDefDecrease;
            if (DataTransfer.npcFighter.magicDmgResist < 0) DataTransfer.npcFighter.magicDmgResist = 0;
        }
        else
        {
            uiFight.AddInfoToActionText("На отшельника накладывается эффект \"" + _effectName + "\"");
            
            player.effectName = _effectName;
            player.effectLength = _effectLength;
            player.effectPhysDmg = _effectPhysDmg;
            player.effectMagDmg = _effectMagDmg;
            player.physDmgDefDefault = DataTransfer.playerStats.mainPhysDef;
            player.magDmgDefDefault = DataTransfer.playerStats.mainMagicDef;

            DataTransfer.playerStats.mainPhysDef -= _physDmgDefDecrease;
            if (DataTransfer.playerStats.mainPhysDef < 0) DataTransfer.playerStats.mainPhysDef = 0;
            else if (DataTransfer.playerStats.mainPhysDef > 100) DataTransfer.playerStats.mainPhysDef = 100;

            DataTransfer.playerStats.mainMagicDef -= _magDmgDefDecrease;
            if (DataTransfer.playerStats.mainMagicDef < 0) DataTransfer.playerStats.mainMagicDef = 0;
            else if (DataTransfer.playerStats.mainMagicDef > 100) DataTransfer.playerStats.mainMagicDef = 100;
        }
    }

    private void DeleteEffect(bool fromEnemy)
    {
        if (fromEnemy)
        {
            uiFight.AddInfoToActionText("С персонажа \"" + DataTransfer.enemyName + "\" снимается эффект \"" + enemy.effectName + "\"");

            enemy.effectName = "Нет";
            enemy.effectLength = 0;
            enemy.effectPhysDmg = 0;
            enemy.effectMagDmg = 0;
            enemy.physDmgDefDefault = 0;
            enemy.magDmgDefDefault = 0;
        }
        else
        {
            uiFight.AddInfoToActionText("С отшельника снимается эффект \"" + player.effectName + "\"");
            if (player.effectName == "Проклятая тень") specialMageSkillActive = false;
            else if (player.effectName == "Адреналин") specialWarriorSkillActive = false;

            player.effectName = "Нет";
            player.effectLength = 0;
            player.effectPhysDmg = 0;
            player.effectMagDmg = 0;
            player.physDmgDefDefault = 0;
            player.magDmgDefDefault = 0;
        }
    }

    public void UseSkill(string skillName)
    {
        SoundManager.GetSound("skill").audioSource.Play();

        uiFight.CloseUISkillList();

        ActiveSkill skill = DataTransfer.activeSkillList[skillName];
        uiFight.AddInfoToActionText("Отшельник применяет способность \"" + skillName + "\"");

        if (skill.physicalDamage > 0)
        {
            HitEnemy(skill.physicalDamage, true, skill.ignoreDef);
        }

        if (skill.magicalDamage > 0)
        {
            HitEnemy(skill.magicalDamage, false, skill.ignoreDef);
        }

        if (skill.HPAlteration != 0)
        {
            DataTransfer.playerStats.changeHP(skill.HPAlteration);
        }

        if (skill.MPAlteration != 0)
        {
            DataTransfer.playerStats.changeMP(skill.MPAlteration);
        }

        if (skill.physDmgIncrease > 0)
        {
            player.physDmgBoost = skill.physDmgIncrease;
        }

        if (skill.magicDmgIncrease > 0)
        {
            player.magDmgBoost = skill.magicDmgIncrease;
        }

        if (skill.moveNumber > 0)
        {
            SetEffect(true, skill.designation, skill.moveNumber, skill.physDmgPerMove, skill.magicDmgPerMove,
                skill.physDmgDefDecrease, skill.magicDmgDefDecrease);
        }

        NextTurn();
    }

    public void HealEnemy()
    {
        SoundManager.GetSound("skill").audioSource.Play();

        DataTransfer.npcFighter.HP += DataTransfer.npcFighter.canHeal;
        if (DataTransfer.npcFighter.HP > DataTransfer.npcFighter.maxHP) 
            DataTransfer.npcFighter.HP = DataTransfer.npcFighter.maxHP;

        uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" восстановил здоровье!");
        NextTurn();
    }

    public void EnemyBoostPhysDmg()
    {
        SoundManager.GetSound("change").audioSource.Play();

        enemy.physDmgBoost = DataTransfer.npcFighter.canBoostPhys;

        uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" встал в боевую стойку!");
        NextTurn();
    }

    public void EnemyBoostMagDmg()
    {
        SoundManager.GetSound("skill").audioSource.Play();

        enemy.magDmgBoost = DataTransfer.npcFighter.canBoostMagic;

        uiFight.AddInfoToActionText("Персонаж \"" + DataTransfer.enemyName + "\" на чём-то сконцентрировался!");
        NextTurn();
    }

    private int CountEscapeChance()
    {
        int chance = DataTransfer.playerStats.escapeChance;
        if (DataTransfer.inventory.haveItems("smokeBomb") > 0)
            chance += 30;
        if (enemy.effectName == "Ветер в лицо" || enemy.effectName == "Ослепление")
            chance += 25;

        if (chance > 100) chance = 100;
        return chance;
    }

    public string EscapeChanceDescription()
    {
        string description = "Шанс побега: " + CountEscapeChance() + "%";
        if (DataTransfer.inventory.haveItems("smokeBomb") > 0)
            description += "\nПри побеге будет потрачена дымовая бомба!";
        return description;
    }

    public void TryToEscape()
    {
        if (DataTransfer.inventory.haveItems("smokeBomb") > 0)
            DataTransfer.inventory.loseItem("smokeBomb", 1, false);

        if (Random.Range(1, 101) <= CountEscapeChance())
            DataTransfer.EndFight(false);
        else
        {
            uiFight.AddInfoToActionText("Отшельник попытался сбежать, но у него не получилось!");
            NextTurn();
        }
    }

    private void ChooseNextTurn()
    {
        currentTurnName = nextTurnName;

        int temp = DataTransfer.playerStats.agility + DataTransfer.npcFighter.agility;
        string _nextTurnName = (Random.Range(1, temp + 1) <= DataTransfer.playerStats.agility) ? "Отшельник" : DataTransfer.enemyName;

        if (_nextTurnName == nextTurnName) sameTurn++;
        else sameTurn = 0;

        if (sameTurn == 3)
        {
            sameTurn = 0;
            if (_nextTurnName == "Отшельник") nextTurnName = DataTransfer.enemyName;
            else nextTurnName = "Отшельник";
        }
        else
        {
            nextTurnName = _nextTurnName;
        }
    }

    public void NextTurn()
    {
        turnNumber++;
        if (nextTurnName == "Отшельник")
        {
            ChooseNextTurn();
            uiFight.SetAllButtonActive(true);

            if (player.effectName != "Нет")
            {
                player.effectLength--;
                if (player.effectLength == 0) DeleteEffect(false);
            }
        }
        else
        {
            ChooseNextTurn();
            uiFight.SetAllButtonActive(false);

            if (enemy.effectName != "Нет")
            {
                if (enemy.effectLength == 0) DeleteEffect(true);
                if (enemy.effectPhysDmg > 0) HitEnemy(enemy.effectPhysDmg, true, true, true);
                if (enemy.effectMagDmg > 0) HitEnemy(enemy.effectMagDmg, false, true, true);

                enemy.effectLength--;
            }
            GetComponent<EnemyAI>().ChooseMove();
        }
        CheckSpecialWarriorSkill();
        CheckSpecialMageSkill();

        uiFight.UpdateAllTextFields();
    }

    private void CheckSpecialWarriorSkill()
    {
        if (DataTransfer.playerStats.specialWarriorSkill && !specialMageSkillActive && !specialTheifSkillActive)
        {
            if (DataTransfer.playerStats.currentHP < DataTransfer.playerStats.maxHP * 0.25)
            {
                uiFight.AddInfoToActionText("Отшельник активирует умение \"Ярость\"");

                DataTransfer.playerStats.specialWarriorSkill = false;
                specialWarriorSkillActive = true;
                SetEffect(false, "Адреналин", 5, 0, 0, -35, -35);
                player.physDmgBoost = 75;
            }
        }
    }

    private void CheckSpecialMageSkill()
    {
        if (DataTransfer.playerStats.specialMageSkill && !specialWarriorSkillActive && !specialTheifSkillActive)
        {
            if (DataTransfer.npcFighter.HP < DataTransfer.npcFighter.maxHP / 2 && Random.Range(1, 101) <= 20)
            {
                uiFight.AddInfoToActionText("Отшельник активирует умение \"Проклятье Света и Тьмы\"");

                DataTransfer.playerStats.specialMageSkill = false;
                specialMageSkillActive = true;
                SetEffect(true, "Ослепление", 5, 0, 5, 25, 25);
                SetEffect(false, "Проклятая тень", 5, 0, 0, 0, 0);
            }
        }
    }

    private void CheckSpecialTheifSkill()
    {
        if (DataTransfer.playerStats.specialTheifSkill)
        {
            uiFight.AddInfoToActionText("Отшельник активирует умение \"Удар в Спину\"");

            DataTransfer.playerStats.specialTheifSkill = false;
            specialTheifSkillActive = true;
            player.physDmgBoost = 150;
        }
    }
}
