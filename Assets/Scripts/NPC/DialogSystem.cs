using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Dialog
{
    public string name;
    public string line;
}

[System.Serializable]
public class DialogSystem : MonoBehaviour
{
    [System.Serializable]
    public class Conditions
    {
        public ItemsAmount[] hasItems;
        public string[] questsNotStarted;
        public string[] questsRunning;
        public string[] questsCompleted;
        public string[] questsFailed;
    }

    [System.Serializable]
    public class Result
    {
        public ItemsAmount[] getItems;
        public ItemsAmount[] loseItems;
        public bool willTrustPlayer;

        [Header("Quests manipulation")]
        public string[] questsStart;
        public string[] questsNextStage;
        public string[] questsComplite;
        public string[] questsFail;

        [Header("Objects manipulation")]
        public GameObject[] objectsSpawn;
        public GameObject[] objectsDestroy;
    }

    [System.Serializable]
    public class DialogInfo
    {
        public Object dialogFile;
        public Conditions conditions;
        public Result result;

        public DialogInfo(Object _dialogFile)
        {
            dialogFile = _dialogFile;
            conditions = null;
            result = null;
        }
    }

    public string originalName;
    public Object regularDialog; // На случай, если не одно из условий не выполнено (лучше дать всем NPC с диалогами)
    public DialogInfo aggressiveDialog; // На случай, когда NPC настроен агрессивно (то, что он скажет перед тем как напасть)
    public DialogInfo[] dialogList;
    public bool trustPlayer = false;

    private bool isAggressive = false;
    private bool canFight = false;
    private DialogInfo currentDialog;

    private void Start()
    {
        if (GetComponent<NPCInfo>() != null) originalName = GetComponent<NPCInfo>().NPCName;
    }

    public void StartDialog(bool _isAggressive = false, bool _canFight = false)
    {
        isAggressive = _isAggressive;
        canFight = _canFight;
        currentDialog = null;

        if (isAggressive)
        {
            if (aggressiveDialog.dialogFile != null) currentDialog = aggressiveDialog;
        }
        else
        {
            foreach (DialogInfo dialogInfo in dialogList)
            {
                if (IsChosenOne(dialogInfo))
                {
                    currentDialog = dialogInfo;
                    break;
                }
            }
        }

        if (currentDialog == null && regularDialog != null && !isAggressive) currentDialog = new DialogInfo(regularDialog);

        if (currentDialog != null)
        {
            Queue<Dialog> dialogQueue = GetDialog(currentDialog.dialogFile);

            FindObjectOfType<UIDialogs>().StartDialog(dialogQueue);
        }

        StartCoroutine("WaitForDialogEnd");
    }

    private bool IsChosenOne(DialogInfo dialogInfo)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        QuestList questList = FindObjectOfType<QuestList>();

        foreach (ItemsAmount items in dialogInfo.conditions.hasItems)
            if (inventory.haveItems(items.ID) < items.amount) return false;

        foreach (string questID in dialogInfo.conditions.questsNotStarted)
            if (questList.GetQuest(questID).status != Quest.Status.notStarted) return false;
        foreach (string questID in dialogInfo.conditions.questsRunning)
            if (questList.GetQuest(questID).status != Quest.Status.running) return false;
        foreach (string questID in dialogInfo.conditions.questsCompleted)
            if (questList.GetQuest(questID).status != Quest.Status.completed &&
                questList.GetQuest(questID).status != Quest.Status.completedHide) return false;
        foreach (string questID in dialogInfo.conditions.questsFailed)
            if (questList.GetQuest(questID).status != Quest.Status.failed &&
                questList.GetQuest(questID).status != Quest.Status.failedHide) return false;

        return true;
    }

    public Queue<Dialog> GetDialog(Object file)
    {
        string[] lines = file.ToString().Split('\n');

        Queue<Dialog> dialogQueue = new Queue<Dialog>();
        int i = 0;

        string prevName = "";
        while (i < lines.Length)
        {
            Dialog dialog;
            if (lines[i].Length <= 2 && lines[i].Contains("-"))
                dialog.name = prevName;
            else if (lines[i].Length <= 2 && lines[i].Contains("^"))
                dialog.name = originalName;
            else
            {
                dialog.name = lines[i];
                prevName = lines[i];
            }
            dialog.line = lines[i + 1];
            dialogQueue.Enqueue(dialog);
            i += 3;
        }
        return dialogQueue;
    }

    private IEnumerator WaitForDialogEnd()
    {
        while (OpenWorldManager.GameStatusIsDialogRun()) yield return null;

        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld;

        Inventory inventory = FindObjectOfType<Inventory>();
        QuestList questList = FindObjectOfType<QuestList>();

        if (currentDialog != null && currentDialog.conditions != null)
        {
            foreach (ItemsAmount items in currentDialog.result.getItems)
                inventory.receiveItem(items.ID, items.amount);
            foreach (ItemsAmount items in currentDialog.result.loseItems)
                inventory.loseItem(items.ID, items.amount);

            if (currentDialog.result.willTrustPlayer) trustPlayer = true;

            foreach (string questID in currentDialog.result.questsStart)
                questList.GetQuest(questID).StartQuest();
            foreach (string questID in currentDialog.result.questsNextStage)
                questList.GetQuest(questID).NextStage();
            foreach (string questID in currentDialog.result.questsComplite)
                questList.GetQuest(questID).CompliteQuest();
            foreach (string questID in currentDialog.result.questsFail)
                questList.GetQuest(questID).FailQuest();

            foreach (GameObject obj in currentDialog.result.objectsSpawn)
                obj.SetActive(true);
            foreach (GameObject obj in currentDialog.result.objectsDestroy)
                obj.SetActive(false);
        }

        if (isAggressive && canFight)
        {
            DataTransfer.StartFight(GetComponent<NPCInfo>().NPCName, GetComponent<SpriteRenderer>().sprite, GetComponent<NPCFighter>());
        }

        if (trustPlayer && !isAggressive)
        {
            StartCoroutine("Brainwash");
        }
    }

    private IEnumerator Brainwash()
    {
        UIPopupMessage DM = FindObjectOfType<UIPopupMessage>();
        DM.ShowDialogMessage("Так как данный житель вам доверяет, он уязвим к заклинанию \"Подавление воли\".",
            "Использовать \"Подавление воли\"", "Не использовать \"Подавление воли\"");
        while (DM.DMAnswer == UIPopupMessage.DMAnswers.None) yield return null;

        if (DM.DMAnswer == UIPopupMessage.DMAnswers.Yes)
        {
            FindObjectOfType<MainQuest>().brainwashPoints++;
            Destroy(gameObject);
        }
    }
}
