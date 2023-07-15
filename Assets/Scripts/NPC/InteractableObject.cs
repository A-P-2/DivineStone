using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    [SerializeField] string action = "";

    private DialogSystem dialogSystem;
    private bool playerInRange = false;
    private GameObject selfHint = null;

    private void Start()
    {
        dialogSystem = GetComponent<DialogSystem>();
        if (dialogSystem == null) Debug.LogError("Интерактивный объект " + gameObject.name + " не может существовать без системы диалогов!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, "(E) " + action);
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
            dialogSystem.StartDialog();
        }
    }
}
