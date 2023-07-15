using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogs : MonoBehaviour
{
    private Queue<Dialog> dialog; // Текущий диалог
    public GameObject dialogPanel;
    public Text nameText;
    public Text dialogText;

    // Start is called before the first frame update
    void Start()
    {
        dialog = new Queue<Dialog>();
        dialogPanel.SetActive(false);
    }

    public void StartDialog(Queue<Dialog> dlg)
    {
        UIPauseMenu.ShowCursor(true);
        dialog = dlg; // Сохранить полученный диалог у себя в атрибутах
        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.dialogRun;
        dialogPanel.SetActive(true);
        NextLine();
    }

    public void NextLine()
    {
        if (dialog.Count > 0)
        {
            Dialog d = dialog.Dequeue();
            nameText.text = d.name;
            StopAllCoroutines();
            StartCoroutine(TypeLine(d.line));
        }
        else
        {
            dialogPanel.SetActive(false);
            UIPauseMenu.ShowCursor(false);
            OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.dialogEnd;
        }
    }

    IEnumerator TypeLine (string str)
    {
        dialogText.text = "";
        foreach (char letter in str.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForFixedUpdate();
        }
    }
}
