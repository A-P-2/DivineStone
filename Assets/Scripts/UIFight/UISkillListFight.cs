using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillListFight : MonoBehaviour
{
    [SerializeField] private GameObject skillSlot = null;
    [SerializeField] private Transform contentPanel = null;

    private void Start()
    {
        foreach(var skill in DataTransfer.activeSkillList)
        {
            GameObject tempSkill = Instantiate(skillSlot);
            tempSkill.transform.SetParent(contentPanel);
            tempSkill.transform.localScale = Vector3.one;
            tempSkill.GetComponentInChildren<Text>().text = skill.Key;

            tempSkill.GetComponent<UISkillSlotFight>().activeSkill = skill.Value;

            tempSkill.GetComponent<Button>().onClick.AddListener(() =>
            {
                FindObjectOfType<FightManager>().UseSkill(skill.Key);
                FindObjectOfType<UIFight>().HideHint();
            });

            tempSkill.SetActive(true);
        }
    }
}
