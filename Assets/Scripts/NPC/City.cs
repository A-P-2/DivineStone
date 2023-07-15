using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class City : MonoBehaviour
{
    [SerializeField] private MainQuest.Villages cityName = MainQuest.Villages.None;
    [SerializeField] private string golemName = "";
    [SerializeField] private Sprite golemSprite = null;
    [SerializeField] private NPCFighter golemStat = null;

    private bool playerInRange = false;
    private GameObject selfHint = null;
    private float hintDelay = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Атаковать голема");
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
            if (FindObjectOfType<MainQuest>().AlreadyKillGolem(cityName))
            {
                if (Time.time >= hintDelay)
                {
                    hintDelay = Time.time + 5f;
                    FindObjectOfType<UIPopupMessage>().ShowSideHint("Местный голем уже побеждён");
                }
            }
            else if (FindObjectOfType<MainQuest>().CanAttackGolem(cityName))
            {
                StartCoroutine("CheckReadiness");
            }
            else
            {
                if (Time.time >= hintDelay)
                {
                    hintDelay = Time.time + 5f;
                    FindObjectOfType<UIPopupMessage>().ShowSideHint("Отшельник ещё не готов к этой битве!");
                }
            }
        }
    }

    private IEnumerator CheckReadiness()
    {
        UIPopupMessage DM = FindObjectOfType<UIPopupMessage>();
        DM.ShowDialogMessage("Вы готовы начать битву с големом?",
            "Готов!", "Не готов...");
        while (DM.DMAnswer == UIPopupMessage.DMAnswers.None) yield return null;

        if (DM.DMAnswer == UIPopupMessage.DMAnswers.Yes)
        {
            DataTransfer.StartFight(golemName, golemSprite, golemStat);
        }
    }
}
