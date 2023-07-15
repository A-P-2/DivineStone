using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel = null;
    [SerializeField] private GameObject mainPage = null;
    [SerializeField] private GameObject savePage = null;
    [SerializeField] private GameObject loadPage = null;

    [SerializeField] private bool isOpenWorld = true;

    private void Start()
    {
        ClosePauseMenu();
        if (!isOpenWorld) ShowCursor(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && 
            (FindObjectOfType<OpenWorldManager>() == null || OpenWorldManager.GameStatusIsOpenWorld()))
        {
            if (pauseMenuPanel.activeSelf)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    public void LoadGame(int slotNumber = 0)
    {
        if (slotNumber == 0) DataTransfer.dataFileName = "";
        else DataTransfer.dataFileName = "SlotSave" + slotNumber;

        DataTransfer.status = DataTransfer.Status.openWorld;
        SceneManager.LoadScene(1);
    }

    public void SaveGame(int slotNumber)
    {
        DataSaver.SaveGame("SlotSave" + slotNumber);
        OpenMainPage();
        OpenSaveGamePage();
    }

    public void OpenPauseMenu()
    {
        if (Time.timeScale > 0)
        {
            SoundManager.PauseAllSounds(true);

            pauseMenuPanel.SetActive(true);
            if (isOpenWorld) ShowCursor(true);
            OpenMainPage();
            Time.timeScale = 0;
        }
    }

    public void ClosePauseMenu()
    {
        SoundManager.PauseAllSounds(false);

        pauseMenuPanel.SetActive(false);
        if (isOpenWorld) ShowCursor(false);
        Time.timeScale = 1;
    }

    public void OpenMainPage()
    {
        mainPage.SetActive(true);
        if(isOpenWorld) savePage.SetActive(false);
        loadPage.SetActive(false);
    }

    public void OpenSaveGamePage()
    {
        mainPage.SetActive(false);
        if (isOpenWorld) savePage.SetActive(true);
        loadPage.SetActive(false);
    }

    public void OpenLoadGamePage()
    {
        mainPage.SetActive(false);
        if (isOpenWorld) savePage.SetActive(false);
        loadPage.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
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
