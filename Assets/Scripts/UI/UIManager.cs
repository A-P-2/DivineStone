using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public Color defaultButtonColor;
    public Color openPageButtonColor;

    // Ссылка на интерфейс главной панели
    public GameObject interfacePanel;
    public GameObject hint;

    // Ссылки на кнопки в меню
    public Button btnMenuMap;
    public Button btnMenuInventory;
    public Button btnMenuSkills;
    public Button btnMenuTasks;

    // Ссылки на окна в меню
    public GameObject panelMap;
    public GameObject panelInventory;
    public GameObject panelSkills;
    public GameObject panelTasks;

    private Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        buttons = new Button[] { btnMenuMap, btnMenuInventory, btnMenuSkills, btnMenuTasks };

        // Закрытие всех окон меню
        Close();

        // Обработчики кнопок меню
        btnMenuMap.onClick.AddListener(MapOnClick);
        btnMenuInventory.onClick.AddListener(InventoryOnClick);
        btnMenuSkills.onClick.AddListener(SkillsOnClick);
        btnMenuTasks.onClick.AddListener(TasksOnClick);

        //es.SetSelectedGameObject(menuInventory.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            if (Input.GetKeyDown(KeyCode.I) && (OpenWorldManager.GameStatusIsMenu() || OpenWorldManager.GameStatusIsOpenWorld()))
            {
                if (interfacePanel.activeSelf && panelInventory.activeSelf) // Если интерфейс активен
                {
                    Close();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld; // Игровой статус в открытом мире
                }
                else
                {
                    UIPauseMenu.ShowCursor(true);
                    interfacePanel.SetActive(true);
                    InventoryOnClick();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.menu; // Игровой статус в меню
                }
            }

            if (Input.GetKeyDown(KeyCode.M) && (OpenWorldManager.GameStatusIsMenu() || OpenWorldManager.GameStatusIsOpenWorld()))
            {
                if (interfacePanel.activeSelf && panelMap.activeSelf) // Если интерфейс активен
                {
                    Close();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld; // Игровой статус в открытом мире
                }
                else
                {
                    UIPauseMenu.ShowCursor(true);
                    interfacePanel.SetActive(true);
                    MapOnClick();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.menu; // Игровой статус в меню
                }
            }

            if (Input.GetKeyDown(KeyCode.L) && (OpenWorldManager.GameStatusIsMenu() || OpenWorldManager.GameStatusIsOpenWorld()))
            {
                if (interfacePanel.activeSelf && panelTasks.activeSelf) // Если интерфейс активен
                {
                    Close();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld; // Игровой статус в открытом мире
                }
                else
                {
                    UIPauseMenu.ShowCursor(true);
                    interfacePanel.SetActive(true);
                    TasksOnClick();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.menu; // Игровой статус в меню
                }
            }

            if (Input.GetKeyDown(KeyCode.K) && (OpenWorldManager.GameStatusIsMenu() || OpenWorldManager.GameStatusIsOpenWorld()))
            {
                if (interfacePanel.activeSelf && panelSkills.activeSelf) // Если интерфейс активен
                {
                    Close();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld; // Игровой статус в открытом мире
                }
                else
                {
                    UIPauseMenu.ShowCursor(true);
                    interfacePanel.SetActive(true);
                    SkillsOnClick();
                    OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.menu; // Игровой статус в меню
                }
            }
        }
    }

    void Close()
    {
        UIPauseMenu.ShowCursor(false);
        interfacePanel.SetActive(false);
        hint.SetActive(false);
        panelMap.SetActive(false);
        panelInventory.SetActive(false);
        panelSkills.SetActive(false);
        panelTasks.SetActive(false);
    }

    void MapOnClick()
    {
        ChangeButtonColor(0);

        panelInventory.SetActive(false);
        panelSkills.SetActive(false);
        panelTasks.SetActive(false);
        panelMap.SetActive(true);
        hint.SetActive(false);
    }
    void InventoryOnClick()
    {
        ChangeButtonColor(1);

        panelMap.SetActive(false);
        panelSkills.SetActive(false);
        panelTasks.SetActive(false);
        panelInventory.SetActive(true);
        hint.SetActive(false);
    }
    void SkillsOnClick()
    {
        ChangeButtonColor(2);

        panelMap.SetActive(false);
        panelInventory.SetActive(false);
        panelTasks.SetActive(false);
        panelSkills.SetActive(true);
        hint.SetActive(false);
    }
    void TasksOnClick()
    {
        ChangeButtonColor(3);

        panelMap.SetActive(false);
        panelInventory.SetActive(false);
        panelSkills.SetActive(false);
        panelTasks.SetActive(true);
        hint.SetActive(false);
    }

    // Функции подсказок
    public void ShowHint(string name, string description)
    {
        foreach (Transform child in hint.transform)
        {
            if (child.gameObject.name == "Title")
            {
                child.gameObject.GetComponent<Text>().text = name;
            }
            else
            {
                child.gameObject.GetComponent<Text>().text = description;
            }
        }

        hint.transform.localPosition = new Vector2(-1000, 1000);
        hint.SetActive(true);
    }

    public void HideHint()
    {
        hint.SetActive(false);
    }

    private void ChangeButtonColor(int index)
    {
        ColorBlock colors;
        foreach (Button button in buttons)
        {
            colors = button.colors;
            colors.normalColor = defaultButtonColor;
            button.colors = colors;
        }
        colors = buttons[index].colors;
        colors.normalColor = openPageButtonColor;
        buttons[index].colors = colors;
    }
}
