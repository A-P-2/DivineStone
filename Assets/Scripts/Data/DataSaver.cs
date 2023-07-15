using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataSaver
{
    [System.Serializable]
    private class MainQuestData
    {
        public int questPointsWyvhelm;
        public int killingPointsWyvhelm;

        public int questPointsEghal;
        public int killingPointsEghal;

        public int questPointsRoothearth;
        public int killingPointsRoothearth;

        public int brainwashPoints;

        public MainQuestData(MainQuest mainQuest)
        {
            questPointsWyvhelm = mainQuest.questPointsWyvhelm;
            killingPointsWyvhelm = mainQuest.killingPointsWyvhelm;

            questPointsEghal = mainQuest.questPointsEghal;
            killingPointsEghal = mainQuest.killingPointsEghal;

            questPointsRoothearth = mainQuest.questPointsRoothearth;
            killingPointsRoothearth = mainQuest.killingPointsRoothearth;

            brainwashPoints = mainQuest.brainwashPoints;
        }
    }

    [System.Serializable]
    private class QuestData
    {
        public Quest.Status status;
        public int currentStage;

        public QuestData(Quest quest)
        {
            status = quest.status;
            currentStage = quest.currentStage;
        }
    }

    [System.Serializable]
    private class PlayerData
    {
        public float[] position;

        public Dictionary<string, int> itemsInInventory;
        public string usedArmorID;
        public string usedWeaponID;
        public int currentMoney;

        public int strength;
        public int intelligence;
        public int agility;

        public int currentHP;
        public int maxHP;
        public int currentMP;
        public int maxMP;

        public int level;
        public int statPoints;
        public int skillPoints;
        public int currentExp;
        public int reqExpAmt;
        public int expIncreasedAmt;

        public int mainPhysDmg;
        public int mainMagicDmg;

        public int mainPhysDef;
        public int mainMagicDef;

        public int escapeChance;

        public int unlockChestChance;
        public float physDmgBoost;
        public float magicDmgBoost;

        public bool specialWarriorSkill;
        public bool specialMageSkill;
        public bool specialTheifSkill;

        public bool ignorePhysDmg;
        public bool ignoreMagicDmg;

        public PlayerData(GameObject player)
        {
            position = new float[] { player.transform.position.x, player.transform.position.y };

            Inventory inventory = player.GetComponentInChildren<Inventory>();
            itemsInInventory = inventory.stockedItems;
            usedArmorID = inventory.usedArmorID;
            usedWeaponID = inventory.usedWeaponID;
            currentMoney = inventory.currentMoney;

            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            strength = playerStats.strength;
            intelligence = playerStats.intelligence;
            agility = playerStats.agility;

            currentHP = playerStats.currentHP;
            maxHP = playerStats.maxHP;
            currentMP = playerStats.currentMP;
            maxMP = playerStats.maxMP;

            level = playerStats.level;
            statPoints = playerStats.statPoints;
            skillPoints = playerStats.skillPoints;
            currentExp = playerStats.currentExp;
            reqExpAmt = playerStats.reqExpAmt;
            expIncreasedAmt = playerStats.expIncreasedAmt;

            mainPhysDmg = playerStats.mainPhysDmg;
            mainMagicDmg = playerStats.mainMagicDmg;

            mainPhysDef = playerStats.mainPhysDef;
            mainMagicDef = playerStats.mainMagicDef;

            escapeChance = playerStats.escapeChance;

            unlockChestChance = playerStats.unlockChestChance;
            physDmgBoost = playerStats.physDmgBoost;
            magicDmgBoost = playerStats.magicDmgBoost;

            specialWarriorSkill = playerStats.specialWarriorSkill;
            specialMageSkill = playerStats.specialMageSkill;
            specialTheifSkill = playerStats.specialTheifSkill;

            ignorePhysDmg = playerStats.ignorePhysDmg;
            ignoreMagicDmg = playerStats.ignoreMagicDmg;
        }
    }

    [System.Serializable]
    private class SkillData
    {
        public bool unlocked;

        public SkillData(GameObject skill)
        {
            unlocked = (skill.GetComponent<ActiveSkill>() != null) ? 
                skill.GetComponent<ActiveSkill>().unlocked : skill.GetComponent<PassiveSkill>().unlocked;
        }
    }

    [System.Serializable]
    private class NPCData
    {
        public bool isActive = true;
        public bool isAggressive = false;
        public NPCTrader.ItemForSale[] itemsForSale;
        public bool trustPlayer = false;

        public NPCData(GameObject npc)
        {
            isActive = npc.activeSelf;
            isAggressive = npc.GetComponent<NPCInfo>().isAggressive;
            if (npc.GetComponent<NPCTrader>() != null) itemsForSale = npc.GetComponent<NPCTrader>().itemsForSale;
            if (npc.GetComponent<DialogSystem>() != null) trustPlayer = npc.GetComponent<DialogSystem>().trustPlayer;
        }
    }

    [System.Serializable]
    private class ChestData
    {
        public bool isActive;
        public bool locked;
        public Dictionary<string, int> itemsInChest;

        public ChestData(Chests chest)
        {
            isActive = chest.gameObject.activeSelf;
            locked = chest.chestLocked;
            itemsInChest = chest.stockedItems;
        }
    }

    [System.Serializable]
    private class GameData // Хранит всё инфу об игру, используя классы выше
    {
        public MainQuestData mainQuestData;
        public Dictionary<string, QuestData> questsData;
        public PlayerData playerData;
        public Dictionary<string, SkillData> skillsData;
        public Dictionary<string, bool> collectableItemsData;
        public Dictionary<string, Dictionary<string, NPCData>> NPCListData;
        public Dictionary<string, bool> questObjectsData;
        public Dictionary<string, ChestData> chestsData;

        public GameData ()
        {
            mainQuestData = new MainQuestData(Object.FindObjectOfType<MainQuest>());
            questsData = GetQuestsData();
            playerData = new PlayerData(GameObject.Find("Player"));
            skillsData = GetSkillsData();
            collectableItemsData = GetCollectableItemsData();
            NPCListData = GetNPCData();
            questObjectsData = GetQuestObjectsData();
            chestsData = GetChestsData();
        }
    }

    private static Dictionary<string, QuestData> GetQuestsData()
    {
        Dictionary<string, QuestData> answer = new Dictionary<string, QuestData>();
        Transform questList = GameObject.Find("Quest List").transform;
        foreach (Transform quest in questList)
        {
            answer.Add(quest.name, new QuestData(quest.gameObject.GetComponent<Quest>()));
        }

        return answer;
    }

    private static Dictionary<string, SkillData> GetSkillsData()
    {
        Dictionary<string, SkillData> answer = new Dictionary<string, SkillData>();
        Transform skillList = GameObject.Find("SkillList").transform;
        foreach (Transform skill in skillList)
        {
            answer.Add(skill.name, new SkillData(skill.gameObject));
        }

        return answer;
    }

    private static Dictionary<string, bool> GetCollectableItemsData()
    {
        Dictionary<string, bool> answer = new Dictionary<string, bool>();
        Transform colItemsList = GameObject.Find("Collectable Items").transform;
        foreach (Transform colItem in colItemsList)
        {
            answer.Add(colItem.name, colItem.gameObject.activeSelf);
        }

        return answer;
    }

    private static Dictionary<string, Dictionary<string, NPCData>> GetNPCData()
    {
        Dictionary<string, Dictionary<string, NPCData>> answer = new Dictionary<string, Dictionary<string, NPCData>>();
        Transform NPCList = GameObject.Find("NPC List").transform;
        foreach (Transform npcGroup in NPCList.transform)
        {
            string groupName = npcGroup.name;
            answer.Add(groupName, new Dictionary<string, NPCData>());

            foreach (Transform npc in npcGroup)
            {
                answer[groupName].Add(npc.name, new NPCData(npc.gameObject));
            }
        }

        return answer;
    }

    private static Dictionary<string, bool> GetQuestObjectsData()
    {
        Dictionary<string, bool> answer = new Dictionary<string, bool>();
        Transform colItemsList = GameObject.Find("Quest Objects").transform;
        foreach (Transform colItem in colItemsList)
        {
            answer.Add(colItem.name, colItem.gameObject.activeSelf);
        }

        return answer;
    }

    private static Dictionary<string, ChestData> GetChestsData()
    {
        Dictionary<string, ChestData> answer = new Dictionary<string, ChestData>();
        Transform chestList = GameObject.Find("Chest List").transform;
        foreach (Transform chest in chestList)
        {
            answer.Add(chest.name, new ChestData(chest.gameObject.GetComponent<Chests>()));
        }

        return answer;
    }

    private static void LoadMainQuestData(MainQuestData mainQuestData)
    {
        MainQuest mainQuest = Object.FindObjectOfType<MainQuest>();

        mainQuest.questPointsWyvhelm = mainQuestData.questPointsWyvhelm;
        mainQuest.killingPointsWyvhelm = mainQuestData.killingPointsWyvhelm;

        mainQuest.questPointsEghal = mainQuestData.questPointsEghal;
        mainQuest.killingPointsEghal = mainQuestData.killingPointsEghal;

        mainQuest.questPointsRoothearth = mainQuestData.questPointsRoothearth;
        mainQuest.killingPointsRoothearth = mainQuestData.killingPointsRoothearth;

        mainQuest.brainwashPoints = mainQuestData.brainwashPoints;
    }

    private static void LoadQuestsData(Dictionary<string, QuestData> questsData)
    {
        Transform questList = GameObject.Find("Quest List").transform;
        foreach (Transform quest in questList)
        {
            QuestData tempQuest;
            if (questsData.TryGetValue(quest.name, out tempQuest))
            {
                quest.GetComponent<Quest>().status = tempQuest.status;
                quest.GetComponent<Quest>().currentStage = tempQuest.currentStage;
            }
        }
    }

    private static void LoadPlayerData(PlayerData playerData)
    {
        GameObject.Find("Player").transform.position = new Vector3(playerData.position[0], playerData.position[1], 0);

        Inventory inventory = Object.FindObjectOfType<Inventory>();
        inventory.stockedItems = playerData.itemsInInventory;
        inventory.usedArmorID = playerData.usedArmorID;
        inventory.usedWeaponID = playerData.usedWeaponID;
        inventory.currentMoney = playerData.currentMoney;

        PlayerStats playerStats = Object.FindObjectOfType<PlayerStats>();
        playerStats.strength = playerData.strength;
        playerStats.intelligence = playerData.intelligence;
        playerStats.agility = playerData.agility;

        playerStats.currentHP = playerData.currentHP;
        playerStats.maxHP = playerData.maxHP;
        playerStats.currentMP = playerData.currentMP;
        playerStats.maxMP = playerData.maxMP;

        playerStats.level = playerData.level;
        playerStats.statPoints = playerData.statPoints;
        playerStats.skillPoints = playerData.skillPoints;
        playerStats.currentExp = playerData.currentExp;
        playerStats.reqExpAmt = playerData.reqExpAmt;
        playerStats.expIncreasedAmt = playerData.expIncreasedAmt;

        playerStats.mainPhysDmg = playerData.mainPhysDmg;
        playerStats.mainMagicDmg = playerData.mainMagicDmg;

        playerStats.mainPhysDef = playerData.mainPhysDef;
        playerStats.mainMagicDef = playerData.mainMagicDef;

        playerStats.escapeChance = playerData.escapeChance;

        playerStats.unlockChestChance = playerData.unlockChestChance;
        playerStats.physDmgBoost = playerData.physDmgBoost;
        playerStats.magicDmgBoost = playerData.magicDmgBoost;

        playerStats.specialWarriorSkill = playerData.specialWarriorSkill;
        playerStats.specialMageSkill = playerData.specialMageSkill;
        playerStats.specialTheifSkill = playerData.specialTheifSkill;

        playerStats.ignorePhysDmg = playerData.ignorePhysDmg;
        playerStats.ignoreMagicDmg = playerData.ignoreMagicDmg;
}

    private static void LoadSkillsData(Dictionary<string, SkillData> skillsData)
    {
        Transform skillList = GameObject.Find("SkillList").transform;
        foreach (Transform skill in skillList)
        {
            SkillData tempSkill;
            if (skillsData.TryGetValue(skill.name, out tempSkill))
            {
                if (skill.GetComponent<ActiveSkill>() != null)
                    skill.GetComponent<ActiveSkill>().unlocked = tempSkill.unlocked;
                else
                    skill.GetComponent<PassiveSkill>().unlocked = tempSkill.unlocked;
            }
        }
    }

    private static void LoadCollectableItemsData(Dictionary<string, bool> collectableItemsData)
    {
        Transform colItemList = GameObject.Find("Collectable Items").transform;
        foreach (Transform colItem in colItemList)
        {
            bool colItemIsActive;
            if (collectableItemsData.TryGetValue(colItem.name, out colItemIsActive))
            {
                colItem.gameObject.SetActive(colItemIsActive);
            }
            else
            {
                Object.Destroy(colItem.gameObject);
            }
        }
    }

    private static void LoadNPCData(Dictionary<string, Dictionary<string, NPCData>> NPCListData)
    {
        GameObject npcList = GameObject.Find("NPC List");
        foreach (Transform npcGroup in npcList.transform)
        {
            string groupName = npcGroup.name;
            Dictionary<string, NPCData> npcInGroup = NPCListData[groupName];

            foreach (Transform npc in npcGroup)
            {
                NPCData npcData;
                if (npcInGroup.TryGetValue(npc.name, out npcData))
                {
                    npc.gameObject.SetActive(npcData.isActive);
                    npc.gameObject.GetComponent<NPCInfo>().isAggressive = npcData.isAggressive;
                    if (npcData.itemsForSale != null)
                        npc.gameObject.GetComponent<NPCTrader>().itemsForSale = npcData.itemsForSale;
                    if (npc.GetComponent<DialogSystem>() != null) 
                        npc.GetComponent<DialogSystem>().trustPlayer = npcData.trustPlayer;
                }
                else
                {
                    Object.Destroy(npc.gameObject);
                }
            }
        }
    }

    private static void LoadQuestObjectsData(Dictionary<string, bool> questObjectsData)
    {
        Transform questObjectsList = GameObject.Find("Quest Objects").transform;
        foreach (Transform questObject in questObjectsList)
        {
            bool questObjectIsActive;
            if (questObjectsData.TryGetValue(questObject.name, out questObjectIsActive))
            {
                questObject.gameObject.SetActive(questObjectIsActive);
            }
        }
    }

    private static void LoadChestsData(Dictionary<string, ChestData> chestsData)
    {
        Transform chestsList = GameObject.Find("Chest List").transform;
        foreach (Transform chest in chestsList)
        {
            ChestData tempChest;
            if (chestsData.TryGetValue(chest.name, out tempChest))
            {
                chest.gameObject.SetActive(tempChest.isActive);
                chest.gameObject.GetComponent<Chests>().chestLocked = tempChest.locked;
                chest.gameObject.GetComponent<Chests>().stockedItems = tempChest.itemsInChest;
            }
        }
    }

    public static void SaveGame(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + fileName + ".dsdata";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData();
        formatter.Serialize(stream, data);
        stream.Close();

        path = Application.persistentDataPath + "/" + fileName + "Meta.dsdata";
        stream = new FileStream(path, FileMode.Create);

        string date = System.DateTime.Now.ToString();
        formatter.Serialize(stream, date);
        stream.Close();
    }

    public static void LoadGame(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + fileName + ".dsdata";

        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            LoadMainQuestData(data.mainQuestData);
            LoadQuestsData(data.questsData);
            LoadPlayerData(data.playerData);
            LoadSkillsData(data.skillsData);
            LoadCollectableItemsData(data.collectableItemsData);
            LoadNPCData(data.NPCListData);
            LoadQuestObjectsData(data.questObjectsData);
            LoadChestsData(data.chestsData);
        }
        else
        {
            Debug.LogError("Попытка загрузить несуществующий файл!");
            return;
        }
    }

    public static string GetMetaData(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + fileName + "Meta.dsdata";

        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            string date = formatter.Deserialize(stream) as string;
            stream.Close();

            return date;
        }
        return "";        
    }
}
