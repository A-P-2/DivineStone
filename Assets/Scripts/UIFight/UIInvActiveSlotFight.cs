using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInvActiveSlotFight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isWeaponSlot;

    private void OnEnable()
    {
        if (isWeaponSlot && DataTransfer.inventory.usedWeaponID != "")
        {
            GetComponent<Image>().sprite = 
                FindObjectOfType<ItemsList>().getCommonInfo(DataTransfer.inventory.usedWeaponID).sprite;
        }
        else if (!isWeaponSlot && DataTransfer.inventory.usedArmorID != "")
        {
            GetComponent<Image>().sprite =
                FindObjectOfType<ItemsList>().getCommonInfo(DataTransfer.inventory.usedArmorID).sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Inventory inventory = DataTransfer.inventory;
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
            FindObjectOfType<UIFight>().ShowHint(item.itemName, item.describtion);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIFight>().HideHint();
    }
}