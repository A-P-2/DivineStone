using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMessage : MonoBehaviour
{
    [Header("Objects Hints")]
    [SerializeField] private Transform objectsHint = null;
    [SerializeField] private GameObject hint = null;

    [Header("Side Hints")]
    [SerializeField] private Transform sideBar = null;
    [SerializeField] private GameObject sideHint = null;

    [SerializeField] private float sideHintTimer = 4f;
    [SerializeField] private float sideHintDelay = 0.5f;
    [SerializeField] private int sideBarCapacity = 10;

    [Header("Dialog Message")]
    [SerializeField] private GameObject dialogMessage = null;

    private float newSideHintTimer = 0;
    private bool sideBarActive = false;
    private Queue<string> sideHintsQueue = new Queue<string>();

    public enum DMAnswers
    {
        Yes,
        No,
        None
    }

    public DMAnswers DMAnswer = DMAnswers.None;

    private void Start()
    {
        dialogMessage.SetActive(false);
    }

    public GameObject ShowObjectHint(Vector2 pos, string text)
    {
        GameObject _hint = Instantiate(hint, objectsHint);
        _hint.GetComponent<RectTransform>().localPosition = pos;
        _hint.GetComponent<TextMeshProUGUI>().text = text;
        return _hint;
    }

    public void ShowSideHint(string text)
    {
        sideHintsQueue.Enqueue(text);
        if (!sideBarActive)
        {
            sideBarActive = true;
            StartCoroutine("AddSideHint");
        }
    }

    private IEnumerator AddSideHint()
    {
        while (true)
        {
            while (newSideHintTimer > Time.time) yield return null;
            while (sideBar.childCount == sideBarCapacity) yield return null;
            while (!OpenWorldManager.GameStatusIsOpenWorld()) yield return null;

            if (sideHintsQueue.Count == 0)
            {
                sideBarActive = false;
                break;
            }

            newSideHintTimer = Time.time + sideHintDelay;
            GameObject _hint = Instantiate(sideHint, sideBar);
            _hint.GetComponentInChildren<TextMeshProUGUI>().text = sideHintsQueue.Dequeue();
            Destroy(_hint, sideHintTimer);
        }
    }
        
    public void HideHint(GameObject _hint)
    {
        Destroy(_hint);
    }

    public void ShowDialogMessage(string question, string yesButtonText = "Да", string noButtonText = "Нет")
    {
        UIPauseMenu.ShowCursor(true);

        DMAnswer = DMAnswers.None;
        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.menu;
        dialogMessage.SetActive(true);
        dialogMessage.transform.Find("Question").GetComponent<TextMeshProUGUI>().text = question;
        dialogMessage.transform.Find("Yes Button").GetComponentInChildren<TextMeshProUGUI>().text = yesButtonText;
        dialogMessage.transform.Find("No Button").GetComponentInChildren<TextMeshProUGUI>().text = noButtonText;
    }

    public void GetAnswerFromDM(bool answer)
    {
        UIPauseMenu.ShowCursor(false);

        dialogMessage.SetActive(false);
        if (answer) DMAnswer = DMAnswers.Yes;
        else DMAnswer = DMAnswers.No;
        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld;
    }
}
