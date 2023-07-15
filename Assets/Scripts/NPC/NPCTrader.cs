using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTrader : MonoBehaviour
{
    [System.Serializable]
    public struct TradeSlot
    {
        public Items item;
        public int amount;
        public int cost;
    }

    [System.Serializable]
    public struct ItemForSale
    {
        public string ID;
        public int amount; // -1, если бесконечно
        public int cost;
    }

    public ItemForSale[] itemsForSale = new ItemForSale[0];

    private void Start()
    {
        for (int i = 0; i < itemsForSale.Length; i++)
        {
            if (itemsForSale[i].amount <= 0) itemsForSale[i].amount = -1;
            if (itemsForSale[i].cost <= 0) Debug.LogError(gameObject + " продаёт вещи за бесплатно!");
        }
    }

    public List<TradeSlot> GetTradeList(bool fromPlayer)
    {
        List<TradeSlot> tradeList = new List<TradeSlot>();
        if (fromPlayer)
        {
            Inventory inventory = FindObjectOfType<Inventory>();
            AddFromInventoryToTradeList(inventory.getItemsList(Items.Type.weapon), ref tradeList);
            AddFromInventoryToTradeList(inventory.getItemsList(Items.Type.armor), ref tradeList);
            AddFromInventoryToTradeList(inventory.getItemsList(Items.Type.consumable), ref tradeList);
            AddFromInventoryToTradeList(inventory.getItemsList(Items.Type.rubbish), ref tradeList);
            return tradeList;
        }
        else
        {
            ItemsList itemsList = FindObjectOfType<ItemsList>();
            foreach (var item in itemsForSale)
            {
                TradeSlot temp;
                temp.item = itemsList.getCommonInfo(item.ID);
                temp.amount = item.amount;
                temp.cost = item.cost;
                if (temp.amount != 0) tradeList.Add(temp);
            }
            return tradeList;
        }
    }

    private void AddFromInventoryToTradeList(Dictionary<Items, int> items, ref List<TradeSlot> tradeList)
    {
        foreach (var item in items)
        {
            TradeSlot temp;
            temp.item = item.Key;
            temp.amount = item.Value;
            temp.cost = item.Key.cost;
            tradeList.Add(temp);
        }
    }

    public void SellItem(string ID) // Игрок продаёт торговцу
    {
        Items item = FindObjectOfType<ItemsList>().getCommonInfo(ID);
        Inventory inventory = FindObjectOfType<Inventory>();

        inventory.changeMoney(item.cost, false);
        inventory.loseItem(ID, 1, false);
    }

    public void SellAllRubbish()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        Dictionary<Items, int> items = inventory.getItemsList(Items.Type.rubbish);
        foreach(var item in items)
        {
            inventory.changeMoney(item.Key.cost * item.Value, false);
            inventory.loseItem(item.Key.ID, item.Value, false);
        }
    }

    public void BuyItem(string ID) // Игрок покупает у торговца
    {
        int index = -1;
        for (int i = 0; i < itemsForSale.Length; i++)
            if (itemsForSale[i].ID == ID) index = i;
        if (index == -1) Debug.LogError("Игрок пытается купить предмет, ID которого нет в списке!");

        Inventory inventory = FindObjectOfType<Inventory>();
        inventory.changeMoney(-itemsForSale[index].cost, false);
        inventory.receiveItem(ID, 1, false);

        if (itemsForSale[index].amount != -1) itemsForSale[index].amount--;
    }

}
