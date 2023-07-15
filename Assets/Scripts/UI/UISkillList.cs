using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillList : MonoBehaviour
{
    public GameObject currentHP;
    public GameObject maxHP;
    public GameObject currentMP;
    public GameObject maxMP;
    public GameObject statPoints; // Очки улучшений
    public GameObject skillPoints; // Очки навыков

    public GameObject strength;
    public GameObject intelligence;
    public GameObject agility;

    // Обновление всех текстовых полей в окне навыков
    void OnEnable()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        currentHP.GetComponent<Text>().text = playerStats.currentHP.ToString();
        maxHP.GetComponent<Text>().text = playerStats.maxHP.ToString();
        currentMP.GetComponent<Text>().text = playerStats.currentMP.ToString();
        maxMP.GetComponent<Text>().text = playerStats.maxMP.ToString();
        statPoints.GetComponent<Text>().text = playerStats.statPoints.ToString();
        skillPoints.GetComponent<Text>().text = playerStats.skillPoints.ToString();

        strength.GetComponent<Text>().text = playerStats.strength.ToString();
        intelligence.GetComponent<Text>().text = playerStats.intelligence.ToString();
        agility.GetComponent<Text>().text = playerStats.agility.ToString();
    }
}
