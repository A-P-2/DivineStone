using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Demon : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject selfHint = null;
    private float hintDelay = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Призвать демона");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FindObjectOfType<UIPopupMessage>().HideHint(selfHint);
            playerInRange = false;
        }
    }

    private void Update()
    {
        if ((playerInRange == true) && (Input.GetKeyDown(KeyCode.E)) && OpenWorldManager.GameStatusIsOpenWorld())
        {
            Inventory inventory = FindObjectOfType<Inventory>();
            if (inventory.haveItems("GolemHeartWyvhelm") > 0
                && inventory.haveItems("GolemHeartEghal") > 0
                && inventory.haveItems("GolemHeartRoothearth") > 0)
                StartCoroutine("CheckReadiness");
            else
            {
                if (Time.time >= hintDelay)
                {
                    hintDelay = Time.time + 5f;
                    FindObjectOfType<UIPopupMessage>().ShowSideHint("Лучше не злить его лишний раз...");
                }
            }
        }
    }

    private IEnumerator CheckReadiness()
    {
        UIPopupMessage DM = FindObjectOfType<UIPopupMessage>();
        DM.ShowDialogMessage("Пришла пора получить свою награду! Готов ли я начать новую жизнь? Назад дороги не будет...",
            "Ещё как готов!", "У меня ещё есть дела...");
        while (DM.DMAnswer == UIPopupMessage.DMAnswers.None) yield return null;

        if (DM.DMAnswer == UIPopupMessage.DMAnswers.Yes)
        {
            FindObjectOfType<MainQuest>().StartLastBossFight();
        }
    }
}
