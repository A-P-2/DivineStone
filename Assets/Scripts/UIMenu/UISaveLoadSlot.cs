using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISaveLoadSlot : MonoBehaviour
{
    [Range(1, 5)]
    [SerializeField] private int slotNumber = 1;
    [SerializeField] private bool loadSlot = true;

    private void OnEnable()
    {
        string date = DataSaver.GetMetaData("SlotSave" + slotNumber);

        if (date == "")
        {
            if (loadSlot) GetComponent<Button>().interactable = false;
            GetComponentInChildren<TextMeshProUGUI>().text = "Пусто";
        }
        else
        {
            if (loadSlot) GetComponent<Button>().interactable = true;
            GetComponentInChildren<TextMeshProUGUI>().text = date;
        }
    }
}
