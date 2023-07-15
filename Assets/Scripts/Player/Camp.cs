using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Camp : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject selfHint = null;
    private float hintDelay = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) Отдохнуть");
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
            if (Time.time >= hintDelay)
            {
                SoundManager.GetSound("change").audioSource.Play();

                hintDelay = Time.time + 5f;
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                playerStats.currentHP = playerStats.maxHP;
                playerStats.currentMP = playerStats.maxMP;

                UIPopupMessage uiPopupMessage = FindObjectOfType<UIPopupMessage>();
                uiPopupMessage.ShowSideHint("Здоровье восстановлено");
                uiPopupMessage.ShowSideHint("Магия восстановлена");
                uiPopupMessage.ShowSideHint("Настроение улучшено... Наверное");
            }
        }
    }
}
