using UnityEngine;
using System.Timers;
using UnityEngine.EventSystems;

public class SlotMouseQuest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Quest quest = FindObjectOfType<UIQuest>().questData[gameObject];

        // Вызов подсказки
        RectTransform rt = GetComponent<RectTransform>();
        FindObjectOfType<UIManager>().ShowHint(quest.questName, quest.FullDescriptions());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIManager>().HideHint();
    }
}
