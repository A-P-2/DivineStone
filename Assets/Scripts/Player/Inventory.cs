using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> stockedItems = new Dictionary<string, int>();
    public string usedArmorID = "";
    public string usedWeaponID = "";
    public int currentMoney;

    public int haveItems(string ID)
    {
        if (ID == "money") return currentMoney;
        else
        {
            int amount = 0;

            if (stockedItems.ContainsKey(ID))
            {
                amount = stockedItems[ID];
            }

            return amount;
        }
    }

    public void receiveItem(string ID, int amount, bool message = true)
    {
        if (ID == "money") changeMoney(amount, message);
        else
        {
            if (stockedItems.ContainsKey(ID))
            {
                stockedItems[ID] += amount;
            }
            else
            {
                stockedItems.Add(ID, amount);
            }

            if (message)
            {
                string itemName = FindObjectOfType<ItemsList>().getCommonInfo(ID).itemName;
                FindObjectOfType<UIPopupMessage>().ShowSideHint("Получен предмет \"" + itemName + "\" в количестве " + amount + " шт.");
            }
        }
    }

    public void loseItem(string ID, int amount, bool message = true)
    {
        if (ID == "money") changeMoney(-amount, message);
        else
        {
            if (stockedItems.ContainsKey(ID))
            {
                if (stockedItems[ID] < amount)
                {
                    Debug.LogError("Предметов с ID " + ID + " меньше, чем вы изымаете(" + amount + ")!");
                }
                else
                {
                    if (stockedItems[ID] == amount)
                    {
                        stockedItems.Remove(ID);
                    }
                    else
                    {
                        stockedItems[ID] -= amount;
                    }
                }
            }
            else
            {
                Debug.LogError("Астрологи объявили вас рукожопом! Так как нельзя изъять то, чего нет в инвентаре!");
            }

            if (message)
            {
                string itemName = FindObjectOfType<ItemsList>().getCommonInfo(ID).itemName;
                FindObjectOfType<UIPopupMessage>().ShowSideHint("Потерян предмет \"" + itemName + "\" в количестве " + amount + " шт.");
            }
        }
    }

    public void changeMoney(int amount, bool message = true)
    {
        if (amount > 0)
        {
            currentMoney += amount;
            if (message)
            {
                FindObjectOfType<UIPopupMessage>().ShowSideHint("Получены монеты в количестве " + amount + " шт.");
            }
        }
        else if (amount < 0)
        {
            amount = -amount;
            if (currentMoney < amount)
                Debug.LogError("Денег у игрока: " + currentMoney + "; Изымается: " + amount);

            currentMoney -= amount;
            if (message)
            {
                FindObjectOfType<UIPopupMessage>().ShowSideHint("Потеряны монеты в количестве " + amount + " шт.");
            }
        }
    }

    public void equipWeapon(string ID = "")
    {
        SoundManager.GetSound("sword").audioSource.Play();

        if (usedWeaponID != "")
        {
            receiveItem(usedWeaponID, 1, false);
        }

        if (ID != "")
        {
            loseItem(ID, 1, false);
        }

        usedWeaponID = ID;
    }

    public void equipArmor(string ID = "")
    {
        SoundManager.GetSound("change").audioSource.Play();

        if (usedArmorID != "")
        {
            receiveItem(usedArmorID, 1, false);
        }

        if (ID != "")
        {
            loseItem(ID, 1, false);
        }

        usedArmorID = ID;
    }

    public bool UseConsumable(string ID, bool message = true)
    {
        bool temp = FindObjectOfType<ItemsList>().UseConsumable(ID);
        if (temp)
            SoundManager.GetSound("drink").audioSource.Play();
            loseItem(ID, 1, message);
        return temp;
    }

    public Dictionary<Items, int> getItemsList(Items.Type requestedType)
    {
        Dictionary<Items, int> typeItems = new Dictionary<Items, int>();

        foreach (string ID in stockedItems.Keys)
        {
            Items item = FindObjectOfType<ItemsList>().getCommonInfo(ID);
            if (item.type == requestedType)
            {
                typeItems.Add(item, stockedItems[ID]);
            }
        }

        return typeItems;
    }

    public Inventory ShallowCopy()
    {
        return (Inventory)MemberwiseClone();
    }
}