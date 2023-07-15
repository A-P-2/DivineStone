using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using UnityEngine.EventSystems;

public class SlotMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type
    {
        inventory,
        chest,
        trade,
    }
    public Type type;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Items item = null;
        switch (type)
        {
            case Type.inventory:
                item = FindObjectOfType<UIInventory>().inventoryData[gameObject].Key;
                break;

            case Type.chest:
                item = FindObjectOfType<UIChest>().chestData[gameObject];
                break;

            case Type.trade:
                item = FindObjectOfType<UITrade>().tradeData[gameObject].item;
                break;
        }

        // Вызов подсказки
        RectTransform rt = GetComponent<RectTransform>();
        FindObjectOfType<UIManager>().ShowHint(item.itemName, item.describtion);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIManager>().HideHint();
    }
}
