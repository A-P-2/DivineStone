using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenWorldManager : MonoBehaviour
{
    public enum GameStatus
    {
        openWorld,
        menu,
        dialogRun,
        dialogEnd,
        tradeOrChest
    }

    static public GameStatus gameStatus;
    [SerializeField] private Image blackScreen = null;

    static public bool GameStatusIsOpenWorld() { return gameStatus == GameStatus.openWorld; }
    static public bool GameStatusIsMenu() { return gameStatus == GameStatus.menu; }
    static public bool GameStatusIsDialogRun() { return gameStatus == GameStatus.dialogRun; }
    static public bool GameStatusIsDialogEnd() { return gameStatus == GameStatus.dialogEnd; }
    static public bool GameStatusIsTradeOrChest() { return gameStatus == GameStatus.tradeOrChest; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) DataSaver.SaveGame("TestSave");
        if (Input.GetKeyDown(KeyCode.U)) DataSaver.LoadGame("TestSave");
    }

    private void Awake()
    {
        gameStatus = GameStatus.openWorld;

        blackScreen.gameObject.SetActive(true);
        blackScreen.color = new Color(0, 0, 0, 1);
        Invoke("CheckForLoading", 0.05f);
    }

    private void CheckForLoading()
    {
        if (DataTransfer.status == DataTransfer.Status.openWorld)
        {
            if (DataTransfer.dataFileName != "")
            {
                SoundManager.PlayBGMusic();

                DataSaver.LoadGame(DataTransfer.dataFileName);
                blackScreen.gameObject.SetActive(false);
            }
            else
            {
                FindObjectOfType<MainQuest>().StartMainQuest();
            }
        }
        else if (DataTransfer.status == DataTransfer.Status.winFight)
        {
            SoundManager.PlayBGMusic();

            DataSaver.LoadGame("BeforeFightSave");
            SetStatAndInvData();

            if (DataTransfer.enemyID == "Golem Wyvhhelm")
            {
                FindObjectOfType<Inventory>().receiveItem("GolemHeartWyvhelm", 1);
                FindObjectOfType<PlayerStats>().addExp(DataTransfer.npcFighter.exp);
                FindObjectOfType<QuestList>().GetQuest("MainQuestWyvhelm").NextStage();
            }
            else if (DataTransfer.enemyID == "Golem Eghal")
            {
                FindObjectOfType<Inventory>().receiveItem("GolemHeartEghal", 1);
                FindObjectOfType<PlayerStats>().addExp(DataTransfer.npcFighter.exp);
                FindObjectOfType<QuestList>().GetQuest("MainQuestEghal").NextStage();
            }
            else if (DataTransfer.enemyID == "Golem Roothearth")
            {
                FindObjectOfType<Inventory>().receiveItem("GolemHeartRoothearth", 1);
                FindObjectOfType<PlayerStats>().addExp(DataTransfer.npcFighter.exp);
                FindObjectOfType<QuestList>().GetQuest("MainQuestRoothearth").NextStage();
            }
            else
            {
                GameObject.Find(DataTransfer.enemyID).GetComponent<NPCInfo>().Die();
            }
            DataTransfer.status = DataTransfer.Status.openWorld;
            blackScreen.gameObject.SetActive(false);
        }
        else if (DataTransfer.status == DataTransfer.Status.loseFight)
        {
            SoundManager.PlayBGMusic();

            DataSaver.LoadGame("BeforeFightSave");
            SetStatAndInvData();
            Vector3 escPos = new Vector3(DataTransfer.npcFighter.escapePoint.x, DataTransfer.npcFighter.escapePoint.y, 0);
            Vector3 pos = GameObject.Find(DataTransfer.enemyID).transform.position + escPos;
            GameObject.Find("Player").transform.position = pos;
            blackScreen.gameObject.SetActive(false);
        }
        else if (DataTransfer.status == DataTransfer.Status.gameComplite)
        {
            DataSaver.LoadGame("BeforeFightSave");
            FindObjectOfType<MainQuest>().StartEpilog();
        }
    }

    private void SetStatAndInvData()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        inventory.stockedItems = DataTransfer.inventory.stockedItems;
        inventory.usedWeaponID = DataTransfer.inventory.usedWeaponID;
        inventory.usedArmorID = DataTransfer.inventory.usedArmorID;

        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.currentHP = DataTransfer.playerStats.currentHP;
        playerStats.currentMP = DataTransfer.playerStats.currentMP;
        playerStats.ignorePhysDmg = DataTransfer.playerStats.ignorePhysDmg;
        playerStats.ignoreMagicDmg = DataTransfer.playerStats.ignoreMagicDmg;
    }
}
