using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chests : MonoBehaviour
{
    public bool hideOnStart = false;
    public Dictionary<string, int> stockedItems = new Dictionary<string, int>();
    public ItemsAmount[] currentItems;
    public bool chestLocked = true;

    private bool inRange = false;
    private GameObject selfHint;

    void Start()
    {
        if (DataTransfer.dataFileName == "")
        {
            foreach (ItemsAmount item in currentItems)
            {
                stockedItems.Add(item.ID, item.amount);
            }
            if (hideOnStart) gameObject.SetActive(false);
        }
    }

    public Dictionary<Items, int> getItemsList()
    {
        Dictionary<Items, int> allItems = new Dictionary<Items, int>();

        foreach (string ID in stockedItems.Keys)
        {
            Items item = FindObjectOfType<ItemsList>().getCommonInfo(ID);
            allItems.Add(item, stockedItems[ID]);

        }

        return allItems;
    }

    public void seizeItem(string ID)
    {
        FindObjectOfType<Inventory>().receiveItem(ID, stockedItems[ID]);
        stockedItems.Remove(ID);
    }

    public void seizeAllItems()
    {
        List<string> chestItemsList = new List<string>();
        foreach(string ID in stockedItems.Keys)
        {
            chestItemsList.Add(ID);
        }

        foreach (string items in chestItemsList)
        {
            seizeItem(items);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (chestLocked)
            {
                selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Взломать сундук");
                inRange = true;
            }
            else
            {
                selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Открыть сундук");
                inRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FindObjectOfType<UIPopupMessage>().HideHint(selfHint);
            inRange = false;
        }
    }

    private void Update()
    {
        if ((inRange == true) && (Input.GetKeyDown(KeyCode.E)) && OpenWorldManager.GameStatusIsOpenWorld())
        {
            if (chestLocked)
            {
                if (FindObjectOfType<Inventory>().haveItems("lockPick") > 0)
                {
                    FindObjectOfType<Inventory>().loseItem("lockPick", 1);

                    if (Random.Range(1, 101) <= FindObjectOfType<PlayerStats>().unlockChestChance)
                    {
                        FindObjectOfType<UIPopupMessage>().ShowSideHint("Успешно взломано!");
                        FindObjectOfType<UIPopupMessage>().HideHint(selfHint);
                        selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Открыть сундук");
                        chestLocked = false;
                    }
                    else
                    {
                        FindObjectOfType<UIPopupMessage>().ShowSideHint("Взлом не удался!");
                    }
                }
                else
                {
                    FindObjectOfType<UIPopupMessage>().ShowSideHint("У вас нет отмычек!");
                }
            }
            else
            {
                FindObjectOfType<UIChest>().OpenChest(GetComponent<Chests>());
            }
        }
    }
}
