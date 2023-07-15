using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatButton : MonoBehaviour
{
    public enum Type
    {
        strength,
        intelligence,
        agility
    }
    public Type type;

    void OnEnable()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        // Активация (блокировка) кнопки
        gameObject.GetComponent<Button>().interactable = false;
        if (playerStats.statPoints > 0)
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public void OnClick()
    {
        switch (type)
        {
            case Type.strength:
                FindObjectOfType<PlayerStats>().increaseSTR();
                break;

            case Type.intelligence:
                FindObjectOfType<PlayerStats>().increaseINT();
                break;

            case Type.agility:
                FindObjectOfType<PlayerStats>().increaseAGI();
                break;
        }

        GameObject skillPanel = GameObject.Find("Skills");
        skillPanel.SetActive(false);
        skillPanel.SetActive(true);
    }
}
