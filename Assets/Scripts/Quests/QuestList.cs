using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList : MonoBehaviour
{
    private Dictionary<string, Quest> list = new Dictionary<string, Quest>();

    private void Awake()
    {
        list.Clear();
        foreach(Transform child in transform)
        {
            Quest quest = child.gameObject.GetComponent<Quest>();
            if (list.ContainsKey(quest.ID))
                Debug.LogError("Квестовый ID \"" + quest.ID + "\" повторяется несколько раз!");
            else
                list.Add(quest.ID, quest);
        }
    }

    public Quest GetQuest(string ID)
    {
        if (!list.ContainsKey(ID))
            Debug.LogError("Квестового ID \"" + ID + "\" нет в списке!");
        return list[ID];
    }

    public List<Quest> GetListByStatus(Quest.Status status)
    {
        List<Quest> answer = new List<Quest>();
        foreach (var quest in list)
        {
            if (quest.Value.status == status) answer.Add(quest.Value);
        }
        return answer;
    }
}
