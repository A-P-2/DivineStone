using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChest : MonoBehaviour
{
    public Dictionary<GameObject, Items> chestData = new Dictionary<GameObject, Items>();

    public GameObject itemSlot;
    public GameObject gridOfSlots;
    public GameObject panelChest;

    public Button btnAll;
    public Button btnClose;

    Chests chest;

    // Start is called before the first frame update
    void Start()
    {
        panelChest.SetActive(false);

        // Обработчики кнопок
        btnAll.onClick.AddListener(() =>
        {
            chest.seizeAllItems();
            UpdateChest();
        });

        btnClose.onClick.AddListener(() =>
        {
            UIPauseMenu.ShowCursor(false);
            OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld;
            panelChest.SetActive(false);
            FindObjectOfType<UIManager>().HideHint();
        });
    }

    public void OpenChest(Chests ch)
    {
        UIPauseMenu.ShowCursor(true);
        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.tradeOrChest;
        chest = ch;
        panelChest.SetActive(true);
        UpdateChest();
    }

    void UpdateChest()
    {
        Dictionary<Items, int> list = chest.getItemsList(); // Получаем список элементов

        // Очищаем сетку и очищаем данные
        foreach (Transform child in gridOfSlots.transform)
        {
            Destroy(child.gameObject);
            chestData.Remove(child.gameObject);
        }

        foreach (KeyValuePair<Items, int> el in list)
        {
            GameObject newSlot = Instantiate(itemSlot, gridOfSlots.transform) as GameObject; // Создание новое объекта
            newSlot.name = el.Key.itemName; // Название слотов = имени предмета

            // Расположение элементов
            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newSlot.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            Button tempButton = newSlot.GetComponent<Button>();
            tempButton.onClick.AddListener(() =>
            {
                FindObjectOfType<UIManager>().HideHint();
                chest.seizeItem(el.Key.ID);
                UpdateChest();
            });

            chestData.Add(newSlot, el.Key);

            if (el.Value > 1) // Пишем количество предметов, если кол-во > 1
            {
                newSlot.GetComponentInChildren<Text>().text = el.Value.ToString();
            }
            else
            {
                newSlot.GetComponentInChildren<Text>().text = "";
            }
            newSlot.GetComponent<Image>().sprite = el.Key.sprite; // Вставляем картинку   
        }
    }
}
