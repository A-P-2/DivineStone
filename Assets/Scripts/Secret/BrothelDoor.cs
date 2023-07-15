using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BrothelDoor : MonoBehaviour
{
    public bool visitedPreviously = false;
    private bool inRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //>>Интерфейс - всплывающие подсказки
            Debug.Log("[Выводим над объектом надпись «(E)Посетить заведение»]");
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //>>Интерфейс - всплывающие подсказки
            Debug.Log("[Убираем надпись]");
            inRange = false;
        }
    }

    private void Update()
    {
        if ((inRange == true) && (Input.GetKey(KeyCode.E)))
        {
            FindObjectOfType<SecretAchievment>().addVisit(visitedPreviously);
        }
    }
}
