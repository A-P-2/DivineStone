using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIDeathScrean : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title = null;
    [SerializeField] private GameObject deathScreanPanel = null;

    private void Start()
    {
        deathScreanPanel.SetActive(false);
    }

    public void LoadGame(int slotNumber = 0)
    {
        if (slotNumber == 0) DataTransfer.dataFileName = "";
        else DataTransfer.dataFileName = "SlotSave" + slotNumber;

        DataTransfer.status = DataTransfer.Status.openWorld;
        SceneManager.LoadScene(1);
    }

    public void OpenDeathScrean()
    {
        if (Random.Range(0, 10) == 0) title.text = "ПОТРАЧЕНО";
        else title.text = "Отшельник умер";

        deathScreanPanel.SetActive(true);
        ShowCursor(true);
        Time.timeScale = 0;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    static public void ShowCursor(bool show)
    {
        if (show)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
