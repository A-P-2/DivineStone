using UnityEngine;
using System.Timers;
using UnityEngine.EventSystems;

public class UIInvSlotFight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Items item = FindObjectOfType<UIInvFight>().inventoryData[gameObject].Key;
        FindObjectOfType<UIFight>().ShowHint(item.itemName, item.describtion);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIFight>().HideHint();
    }
}
