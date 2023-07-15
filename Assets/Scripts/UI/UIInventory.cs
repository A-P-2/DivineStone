using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour
{
    public Color defaultButtonColor;
    public Color openPageButtonColor;

    public Dictionary<GameObject, KeyValuePair<Items, int>> inventoryData = new Dictionary<GameObject, KeyValuePair<Items, int>>(); // Слот-предмет
    Items.Type itemsType = Items.Type.weapon; // Тип предметов для инвентаря, который будет открыт

    public GameObject itemSlot;
    public GameObject gridOfSlots;
    public GameObject fieldAmountOfMoney;

    public EventSystem es;

    // 5 кнопок на виды предметов
    public Button btnWeapon;
    public Button btnArmor;
    public Button btnQuestItem;
    public Button btnСonsumable;
    public Button btnRubbish;

    public Sprite emptySprite;
    public Button btnActiveWeapon;
    public Button btnActiveArmor;

    private Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        // Обработчики кнопок
        btnWeapon.onClick.AddListener(() =>
        {
            ChangeButtonColor(0);
            UpdateInventory(Items.Type.weapon);
        });
        btnArmor.onClick.AddListener(() =>
        {
            ChangeButtonColor(1);
            UpdateInventory(Items.Type.armor);
        });
        btnQuestItem.onClick.AddListener(() =>
        {
            ChangeButtonColor(2);
            UpdateInventory(Items.Type.questItem);
        });
        btnСonsumable.onClick.AddListener(() =>
        {
            ChangeButtonColor(3);
            UpdateInventory(Items.Type.consumable);
        });
        btnRubbish.onClick.AddListener(() =>
        {
            ChangeButtonColor(4);
            UpdateInventory(Items.Type.rubbish);
        });

        btnActiveWeapon.onClick.AddListener(() => {
            ChangeButtonColor(0);
            FindObjectOfType<Inventory>().equipWeapon();
            btnActiveWeapon.GetComponent<Image>().sprite = emptySprite;
            UpdateInventory(Items.Type.weapon);
        });
        btnActiveArmor.onClick.AddListener(() => {
            ChangeButtonColor(1);
            FindObjectOfType<Inventory>().equipArmor();
            btnActiveArmor.GetComponent<Image>().sprite = emptySprite;
            UpdateInventory(Items.Type.armor);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (buttons == null) buttons = new Button[] { btnWeapon, btnArmor, btnQuestItem, btnСonsumable, btnRubbish };
        ChangeButtonColor(0);
        UpdateInventory(itemsType);
    }

    // Обновление инвентаря
    public void UpdateInventory(Items.Type requestedType)
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        // Обновляем деньги
        fieldAmountOfMoney.GetComponent<Text>().text = inventory.currentMoney.ToString();

        // Обновляем активные слоты
        //Предметы в слотах активного оружия и брони хранятся в виде строк с их ID.
        UpdateUsedItems();
        //Чтобы узнать данные предмета по его ID нужно обратиться к классу ItemsList. Например, команда:

        // Очищаем сетку и очищаем данные
        foreach (Transform child in gridOfSlots.transform)
        {
            Destroy(child.gameObject);
            inventoryData.Remove(child.gameObject);
        }

        Dictionary<Items, int> itemsList = inventory.getItemsList(requestedType); // Получаем список элементов по типу

        foreach (KeyValuePair<Items, int> el in itemsList)
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
                UseItem(newSlot); // Перетаскивание объекта
            });

            inventoryData.Add(newSlot, el);

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

        //Debug.Log("intentory " + inventoryData.Count);
       // Debug.Log("DB " + itemsList.Count);
    }

    public void UseItem(GameObject item)
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        KeyValuePair<Items, int> el = inventoryData[item];
        switch (el.Key.type)
        {
            case Items.Type.weapon:
                {
                    inventory.equipWeapon(el.Key.ID);
                    break;
                }
            case Items.Type.armor:
                {
                    inventory.equipArmor(el.Key.ID);
                    break;
                }
            case Items.Type.consumable:
                {
                    inventory.UseConsumable(el.Key.ID);
                    break;
                }
        }
        UpdateInventory(el.Key.type);
    }

    void UpdateUsedItems()
    {
        foreach (KeyValuePair<GameObject, KeyValuePair<Items, int>> el in inventoryData)
        {
            if (FindObjectOfType<Inventory>().usedWeaponID == el.Value.Key.ID)
            {
                btnActiveWeapon.GetComponent<Image>().sprite = el.Value.Key.sprite; // Вставляем картинку   
            }
            if (FindObjectOfType<Inventory>().usedArmorID == el.Value.Key.ID)
            {
                btnActiveArmor.GetComponent<Image>().sprite = el.Value.Key.sprite; // Вставляем картинку   
            }
        }
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

    /*
     * Text myText = GameObject.Find("Title").GetComponent<Text>();
        myText.text = "ff";


   // Добавление элемента инвентаря
   public void AddItem(int idSlot, Item item, int count)
   {
       items[idSlot].id = item.id;
       items[idSlot].count = count;
       items[idSlot].itemGameObj.GetComponent<Image>().sprite = item.img;

       // Показываем количество
       if (count > 1 && idSlot != 0)
       {
           items[idSlot].itemGameObj.GetComponentInChildren<Text>().text = count.ToString();
       }
       else
       {
           items[idSlot].itemGameObj.GetComponentInChildren<Text>().text = "";
       }
   }
   
    // Добавление элемента инвентаря
    public void AddInventoryItem(int id, ItemInventory invItem)
    {
        items[id].id = invItem.id;
        items[id].count = invItem.count;
        items[id].itemGameObj.GetComponent<Image>().sprite = itemsElem[invItem.id].img; // Забирает фото из бд

        if (invItem.count > 1 && invItem.id != 0)
        {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = invItem.count.ToString();
        }
        else
        {
            items[id].itemGameObj.GetComponentInChildren<Text>().text = "";
        }
    }

    public void AddGraphics()
    {
        for (int i = 0; i < maxCount; i++) // Графическое отображение ячеек
        {
            GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform) as GameObject; // slot в grid
            newItem.name = i.ToString(); // название слотов 0 1 2 3...

            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = newItem;

            // Расположение элементов
            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newItem.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            Button tempButton = newItem.GetComponent<Button>();
            tempButton.onClick.AddListener(delegate { SelectObject(); }); // Перетаскивание объекта

            items.Add(ii); // добавление в поля инверторя слота
        }
    }

    /*
    // Соединение предметов
    public void SearchForSameItem(Item item, int count)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (items[i].id == item.id)
            {
                if (items[i].count < 100) // Максимальное количество предметов в ячейке
                {
                    items[i].count += count;
                    if (items[i].count > 100)
                    {
                        count = items[i].count - 100;
                        items[i].count = 64; // ?
                    }
                    else
                    {
                        count = 0;
                        i = maxCount;
                    }

                }
            }

        }

        if (count > 0)
        {
            for (int i = 0; i < maxCount; i++)
            {
                if (items[i].id == item.id)
                {
                    AddItem(i, item, count);
                    i = maxCount;
                }
            }
        }
    }

    public void SelectObject() // Когда выбирается ячейка
    {
        if (currentId == -1) // Пустая ячейка
        {
            currentId = int.Parse(es.currentSelectedGameObject.name);
            currentItem = CopyInventoryItem(items[currentId]);
            movingObject.gameObject.SetActive(true);
            movingObject.GetComponent<Image>().sprite = itemsElem[currentItem.id].img; // Картинка предмета

            AddItem(currentId, itemsElem[0], 0);
        }
        else
        {
            ItemInventory II = items[int.Parse(es.currentSelectedGameObject.name)];

            if (currentItem.id != II.id)
            {
                AddInventoryItem(currentId, II);
                AddInventoryItem(int.Parse(es.currentSelectedGameObject.name), currentItem);
            }
            else
            {
                if (II.count + currentItem.count <= 100)
                {
                    II.count += currentItem.count;
                }
                else
                {
                    AddItem(currentId, itemsElem[II.id], II.count + currentItem.count - 100);
                    II.count = 100;
                }

                II.itemGameObj.GetComponentInChildren<Text>().text = II.count.ToString();
            }
            currentId = -1;

            movingObject.gameObject.SetActive(false);
        }
    }

    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z;
        movingObject.position = cam.ScreenToWorldPoint(pos);
    }

    public ItemInventory CopyInventoryItem(ItemInventory old) // Копируем содержимое ячейки
    {
        ItemInventory New = new ItemInventory();

        New.id = old.id;
        New.itemGameObj = old.itemGameObj;
        New.count = old.count;

        return New;
    }

    void Awake()
    {
        Debug.Log("Awake");
        // Обращаемся к другому классу
        Inventory inventory = FindObjectOfType<Inventory>();
        Items.Type itemsType = Items.Type.questItem;

        Dictionary<Items, int> itemsList = inventory.getItemsList(itemsType); // Получаем список элементов
        //Debug.Log(itemsList.Count);
    } */
}
