using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInvFight : MonoBehaviour
{
    public Color defaultButtonColor;
    public Color openPageButtonColor;

    public Dictionary<GameObject, KeyValuePair<Items, int>> inventoryData = new Dictionary<GameObject, KeyValuePair<Items, int>>(); // Слот-предмет
    Items.Type itemsType = Items.Type.weapon; // Тип предметов для инвентаря, который будет открыт

    public GameObject itemSlot;
    public GameObject gridOfSlots;

    public Button btnWeapon;
    public Button btnArmor;
    public Button btnСonsumable;

    public Sprite emptySprite;
    public Button btnActiveWeapon;
    public Button btnActiveArmor;

    private Button[] buttons;

    void Start()
    {
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
        btnСonsumable.onClick.AddListener(() =>
        {
            ChangeButtonColor(2);
            UpdateInventory(Items.Type.consumable);
        });

        btnActiveWeapon.onClick.AddListener(() =>
        {
            FindObjectOfType<UIFight>().HideHint();
            btnActiveWeapon.GetComponent<Image>().sprite = emptySprite;
            FindObjectOfType<FightManager>().EquipWeapon("");
        });
        btnActiveArmor.onClick.AddListener(() =>
        {
            FindObjectOfType<UIFight>().HideHint();
            btnActiveArmor.GetComponent<Image>().sprite = emptySprite;
            FindObjectOfType<FightManager>().EquipArmor("");
        });
    }

    private void OnEnable()
    {
        if (buttons == null) buttons = new Button[] { btnWeapon, btnArmor, btnСonsumable };
        ChangeButtonColor(0);
        UpdateInventory(itemsType);
    }

    public void UpdateInventory(Items.Type requestedType)
    {
        Inventory inventory = DataTransfer.inventory;

        foreach (Transform child in gridOfSlots.transform)
        {
            Destroy(child.gameObject);
            inventoryData.Remove(child.gameObject);
        }

        Dictionary<Items, int> itemsList = inventory.getItemsList(requestedType);

        foreach (KeyValuePair<Items, int> el in itemsList)
        {
            GameObject newSlot = Instantiate(itemSlot, gridOfSlots.transform) as GameObject;
            newSlot.name = el.Key.itemName;

            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);
            rt.localScale = new Vector3(1, 1, 1);
            newSlot.GetComponentInChildren<RectTransform>().localScale = new Vector3(1, 1, 1);

            Button tempButton = newSlot.GetComponent<Button>();
            tempButton.onClick.AddListener(() =>
            {
                FindObjectOfType<UIFight>().HideHint();
                UseItem(newSlot);
            });

            inventoryData.Add(newSlot, el);

            if (el.Value > 1)
            {
                newSlot.GetComponentInChildren<Text>().text = el.Value.ToString();
            }
            else
            {
                newSlot.GetComponentInChildren<Text>().text = "";
            }
            newSlot.GetComponent<Image>().sprite = el.Key.sprite;
        }
    }

    public void UseItem(GameObject item)
    {
        FightManager fightManager = FindObjectOfType<FightManager>();

        KeyValuePair<Items, int> el = inventoryData[item];
        switch (el.Key.type)
        {
            case Items.Type.weapon:
                {
                    fightManager.EquipWeapon(el.Key.ID);
                    break;
                }
            case Items.Type.armor:
                {
                    fightManager.EquipArmor(el.Key.ID);
                    break;
                }
            case Items.Type.consumable:
                {
                    if (DataTransfer.inventory.UseConsumable(el.Key.ID, false))
                    {
                        fightManager.UseConsumable(el.Key.ID);
                    }
                    break;
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
}