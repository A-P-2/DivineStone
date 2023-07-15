using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public static class DataTransfer
{
    public enum Status
    {
        openWorld,
        fight,
        finalFight,
        winFight,
        loseFight,
        gameComplite
    }

    static public string dataFileName = "";
    static public Vector2 playerPosition;
    static public PlayerStats playerStats;
    static public Inventory inventory;
    static public Dictionary<string, ActiveSkill> activeSkillList = new Dictionary<string, ActiveSkill>();

    static public string enemyID;
    static public string enemyName;
    static public Sprite enemySprite;
    static public NPCFighter npcFighter;
    static public Status status = Status.openWorld;

    static public void StartFight(string _enemyName, Sprite _enemySprite, NPCFighter _npcFighter, bool isFinalFight = false)
    {
        if (!isFinalFight) status = Status.fight;
        else status = Status.finalFight;

        playerPosition = GameObject.Find("Player").transform.position;
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>().ShallowCopy();
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>().ShallowCopy();

        activeSkillList.Clear();
        foreach (Transform skill in GameObject.Find("SkillList").transform)
        {
            ActiveSkill activeSkill = skill.gameObject.GetComponent<ActiveSkill>();
            if (activeSkill != null && activeSkill.unlocked)
            {
                activeSkill = activeSkill.ShallowCopy();
                activeSkillList.Add(activeSkill.skillName, activeSkill);
            }
        }

        enemyID = _npcFighter.gameObject.name;
        enemyName = _enemyName;
        enemySprite = _enemySprite;
        npcFighter = _npcFighter.ShallowCopy();

        DataSaver.SaveGame("BeforeFightSave");

        SceneManager.LoadScene(2);
    }

    static public void EndFight(bool win = true)
    {
        if (status == Status.finalFight)
            status = Status.gameComplite;
        else
        {
            if (win) status = Status.winFight;
            else status = Status.loseFight;
        }
        SceneManager.LoadScene(1);
    }
}
