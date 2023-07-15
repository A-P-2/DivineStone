using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIQuest : MonoBehaviour
{
    public Color defaultButtonColor;
    public Color openPageButtonColor;

    public Dictionary<GameObject, Quest> questData = new Dictionary<GameObject, Quest>();
    Quest.Status startStatus = Quest.Status.running;

    public GameObject itemQuest;
    public GameObject contentQuest;

    public EventSystem es;

    // Кнопки разделения квестов
    public Button btnActive;
    public Button btnDone;
    public Button btnFailed;

    private Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        // Обработчики кнопок
        btnActive.onClick.AddListener(() =>
        {
            ChangeButtonColor(0);
            UpdateQuest(startStatus);
        });
        btnDone.onClick.AddListener(() =>
        {
            ChangeButtonColor(1);
            UpdateQuest(Quest.Status.completed);
        });
        btnFailed.onClick.AddListener(() =>
        {
            ChangeButtonColor(2);
            UpdateQuest(Quest.Status.failed);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (buttons == null) buttons = new Button[] { btnActive, btnDone, btnFailed };
        ChangeButtonColor(0);
        UpdateQuest(startStatus);
    }

    // Обновление инвентаря
    public void UpdateQuest(Quest.Status status)
    {
        // Список квестов по статусу
        List<Quest> listQuests = FindObjectOfType<QuestList>().GetListByStatus(status);

        // Очищаем сетку и очищаем данные
        foreach (Transform child in contentQuest.transform)
        {
            Destroy(child.gameObject);
            questData.Remove(child.gameObject);
        }

        foreach (Quest q in listQuests)
        {
            GameObject newSlot = Instantiate(itemQuest, contentQuest.transform) as GameObject; // Создание новое объекта
            newSlot.name = q.name; // Название слотов = имени квеста

            // Расположение элементов
            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newSlot.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            questData.Add(newSlot, q);
            newSlot.GetComponentInChildren<Text>().text = q.questName; // Выводим название  
        }


        /*
        

        Dictionary<Items, int> itemsList = inventory.getItemsList(requestedType); // Получаем список элементов по типу

        foreach (KeyValuePair<Items, int> el in itemsList)
        {
            // Расположение элементов
          

            Button tempButton = newSlot.GetComponent<Button>();
            tempButton.onClick.AddListener(() =>
            {
                FindObjectOfType<UIManager>().HideHint();
                UseItem(newSlot); // Перетаскивание объекта
            });

            inventoryData.Add(newSlot, el);
            
        */
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
