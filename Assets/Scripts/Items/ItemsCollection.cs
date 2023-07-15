using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemsCollection : MonoBehaviour
{
    public string ID;
    public int amount = 1;

    private bool inRange = false;
    private GameObject selfHint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            string itemName = FindObjectOfType<ItemsList>().getCommonInfo(ID).itemName;
            selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Взять \"" + itemName + "\"");
            inRange = true;
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
            ItemsList item = FindObjectOfType<ItemsList>();
            FindObjectOfType<Inventory>().receiveItem(ID, amount);
            Destroy(gameObject);
        }
    }

}
