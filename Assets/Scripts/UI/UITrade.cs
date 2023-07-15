using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrade : MonoBehaviour
{
    public Color defaultButtonColor;
    public Color openPageButtonColor;

    public Dictionary<GameObject, NPCTrader.TradeSlot> tradeData = new Dictionary<GameObject, NPCTrader.TradeSlot>();
    public Dictionary<GameObject, Items> playerData = new Dictionary<GameObject, Items>();

    public GameObject panelTrade;
    public GameObject itemSlot;
    public GameObject gridOfSlots;
    public GameObject fieldAmountOfMoney;

    public Button btnAll;
    public Button btnClose;
    public Button btnBuy;
    public Button btnSale;

    private Button[] buttons;

    NPCTrader npcTrader;

    // Start is called before the first frame update
    void Start()
    {
        if (buttons == null) buttons = new Button[] { btnBuy, btnSale };

        CloseUI();

        // Обработчики кнопок
        btnClose.onClick.AddListener(() =>
        {
            UIPauseMenu.ShowCursor(false);
            OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld;
            CloseUI();
            FindObjectOfType<UIManager>().HideHint();
        });

        // Кнопка "Купить"
        btnBuy.onClick.AddListener(() =>
        {
            ChangeButtonColor(0);
            btnAll.gameObject.SetActive(false);
            UpdateTrade();
        });

        // Кнопка "Продать"
        btnSale.onClick.AddListener(() =>
        {
            ChangeButtonColor(1);
            btnAll.gameObject.SetActive(true);
            UpdateTrade(true);
        });

        btnAll.onClick.AddListener(() =>
        {
            npcTrader.SellAllRubbish();
            UpdateTrade(true);
        });
    }

    public void OpenTrade(NPCTrader npc)
    {
        ChangeButtonColor(0);
        UIPauseMenu.ShowCursor(true);
        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.tradeOrChest;
        npcTrader = npc;
        panelTrade.SetActive(true);
        gridOfSlots.SetActive(true);
        UpdateTrade();
    }

    void UpdateTrade(bool toSell = false)
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        // Обновляем деньги
        fieldAmountOfMoney.GetComponent<Text>().text = inventory.currentMoney.ToString();

        // Очищаем сетку и очищаем данные
        foreach (Transform child in gridOfSlots.transform)
        {
            Destroy(child.gameObject);
            tradeData.Remove(child.gameObject);
        }

        List<NPCTrader.TradeSlot> list = npcTrader.GetTradeList(toSell);
        

        foreach (NPCTrader.TradeSlot el in list)
        {
            GameObject newSlot = Instantiate(itemSlot, gridOfSlots.transform) as GameObject; // Создание новое объекта
            newSlot.name = el.item.itemName; // Название слотов = имени предмета

            // Расположение элементов
            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newSlot.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            Button tempButton = newSlot.GetComponent<Button>();
            tempButton.onClick.AddListener(() =>
            {
                if (toSell)
                {
                    FindObjectOfType<UIManager>().HideHint();
                    npcTrader.SellItem(el.item.ID);
                    UpdateTrade(true);
                }
                else
                {
                    FindObjectOfType<UIManager>().HideHint();
                    npcTrader.BuyItem(el.item.ID);
                    UpdateTrade();
                }
            });

            if (!toSell)
            {
                if (el.cost > FindObjectOfType<Inventory>().currentMoney) tempButton.interactable = false;
            }

            tradeData.Add(newSlot, el);

            if (el.amount > 0) // Пишем количество предметов, если кол-во > 1
            {
                newSlot.GetComponentInChildren<Text>().text = el.amount.ToString();
            }
            else
            {
                newSlot.GetComponentInChildren<Text>().text = "∞";
            }
            newSlot.GetComponent<Image>().sprite = el.item.sprite; // Вставляем картинку  
            newSlot.transform.Find("Cost").GetComponent<Text>().text = el.cost.ToString(); // Вставка цены
        }
    }

    void CloseUI()
    {
        panelTrade.SetActive(false);
        gridOfSlots.SetActive(false);
        btnAll.gameObject.SetActive(false);
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
