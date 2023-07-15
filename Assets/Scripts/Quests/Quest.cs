using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public enum Status
    {
        notStarted,
        running,
        completed,
        failed,
        completedHide,
        failedHide
    }

    public string ID;
    public string questName;
    [TextArea]
    public string mainDescriptions;
    [TextArea]
    public string[] stageDescriptions;
    public Status status;

    [Header("Reward")]
    public int money;
    public int exp;
    public ItemsAmount[] items;
    public MainQuest.Villages questPoint;

    [HideInInspector] public int currentStage = 0;
    private int lastStage;

    private void Start()
    {
        if (stageDescriptions.Length == 0) Debug.LogError("Квест \"" + questName + "\" должен содержать хотя бы одну стадию!");
        lastStage = stageDescriptions.Length;
    }

    public void StartQuest()
    {
        if (status != Status.notStarted) Debug.LogError("Квест \"" + questName + "\" уже был начат!");

        FindObjectOfType<UIPopupMessage>().ShowSideHint("Задание \"" + questName + "\" начато!");

        status = Status.running;
    }

    public void NextStage()
    {
        if (status != Status.running) Debug.LogError("Квест \"" + questName + "\" ещё не был начат!");

        currentStage++;
        if (currentStage == lastStage) CompliteQuest();
    }

    public void CompliteQuest()
    {
        if (status == Status.running)
        {
            FindObjectOfType<UIPopupMessage>().ShowSideHint("Задание \"" + questName + "\" выполнено!");
            status = Status.completed;
        }
        else
        {
            status = Status.completedHide;
        }

        Inventory inventory = FindObjectOfType<Inventory>();

        FindObjectOfType<UIPopupMessage>().ShowSideHint("Получен опыт: " + exp);
        FindObjectOfType<PlayerStats>().addExp(exp);

        inventory.changeMoney(money);
        foreach (ItemsAmount item in items)
            inventory.receiveItem(item.ID, item.amount);

        FindObjectOfType<MainQuest>().AddPoint(questPoint, true);
    }

    public void FailQuest()
    {
        if (status == Status.running)
        {
            FindObjectOfType<UIPopupMessage>().ShowSideHint("Задание \"" + questName + "\" провалено!");
            status = Status.failed;
        }
        else
        {
            status = Status.failedHide;
        }
    }

    public string FullDescriptions()
    {
        string answer = mainDescriptions;
        if (status == Status.running)
            answer += "\n\n" + stageDescriptions[currentStage];
        return answer;
    }
}
