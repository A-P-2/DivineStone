using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveSlotMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isWeaponSlot;

    private void OnEnable()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (isWeaponSlot && inventory.usedWeaponID != "")
        {
            GetComponent<Image>().sprite =
                FindObjectOfType<ItemsList>().getCommonInfo(inventory.usedWeaponID).sprite;
        }
        else if (!isWeaponSlot && inventory.usedArmorID != "")
        {
            GetComponent<Image>().sprite =
                FindObjectOfType<ItemsList>().getCommonInfo(inventory.usedArmorID).sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        Items item = null;

        if (isWeaponSlot == true && inventory.usedWeaponID != "")
        {
            item = FindObjectOfType<ItemsList>().getCommonInfo(inventory.usedWeaponID);
        }
        else if (!isWeaponSlot && inventory.usedArmorID != "")
        {
            item = FindObjectOfType<ItemsList>().getCommonInfo(inventory.usedArmorID);
        }
        
        if (item != null)
        {
            FindObjectOfType<UIManager>().ShowHint(item.itemName, item.describtion);
        }
        // Вызов подсказки
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIManager>().HideHint();
    }
}
