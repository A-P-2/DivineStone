using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPage = null;
    [SerializeField] private GameObject loadPage = null;

    private void Start()
    {
        SoundManager.PlayBGMusic();

        OpenMainPage();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadGame(int slotNumber = 0)
    {
        if (slotNumber == 0) DataTransfer.dataFileName = "";
        else DataTransfer.dataFileName = "SlotSave" + slotNumber;

        DataTransfer.status = DataTransfer.Status.openWorld;
        SceneManager.LoadScene(1);
    }
    
    public void OpenMainPage()
    {
        mainPage.SetActive(true);
        loadPage.SetActive(false);
    }

    public void OpenLoadGamePage()
    {
        mainPage.SetActive(false);
        loadPage.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
