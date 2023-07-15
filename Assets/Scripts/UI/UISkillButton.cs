using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject skill;

    void Start()
    {
        // Обработка нажатия на навык
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (skill.GetComponent<ActiveSkill>() != null)
            {
                skill.GetComponent<ActiveSkill>().unlockSkill();
            }

            if (skill.GetComponent<PassiveSkill>() != null)
            {
                skill.GetComponent<PassiveSkill>().unlockSkill();
            }

            GameObject skillPanel = GameObject.Find("Skills");
            skillPanel.SetActive(false);
            skillPanel.SetActive(true);
        });
    }

    void OnEnable()
    {
        // Если есть активка
        if (skill.GetComponent<ActiveSkill>() != null)
        {
            if (skill.GetComponent<ActiveSkill>().unlocked)
            {
                gameObject.GetComponent<Button>().interactable = false; // кнопка активированного навыка

                ColorBlock colors = gameObject.GetComponent<Button>().colors;
                colors.disabledColor = new Color32(50, 255, 83, 217);
                gameObject.GetComponent<Button>().colors = colors;
            }
            else
            {
                if (skill.GetComponent<ActiveSkill>().ableToUnlock())
                {
                    gameObject.GetComponent<Button>().interactable = true;
                }
                else
                {
                    gameObject.GetComponent<Button>().interactable = false;

                    ColorBlock colors = gameObject.GetComponent<Button>().colors;
                    colors.disabledColor = new Color32(200, 200, 200, 170);
                    gameObject.GetComponent<Button>().colors = colors;
                }
            }
        }
        else // пассивный
        {
            if (skill.GetComponent<PassiveSkill>().unlocked)
            {
                gameObject.GetComponent<Button>().interactable = false; // кнопка активированного навыка

                //gameObject.GetComponent<Image>().color = new Color(50, 255, 83, 217);
                ColorBlock colors = gameObject.GetComponent<Button>().colors;
                colors.disabledColor = new Color32(50, 255, 83, 217);
                gameObject.GetComponent<Button>().colors = colors;
            }
            else
            {
                if (skill.GetComponent<PassiveSkill>().ableToUnlock())
                {
                    gameObject.GetComponent<Button>().interactable = true;
                }
                else
                {
                    gameObject.GetComponent<Button>().interactable = false;

                    ColorBlock colors = gameObject.GetComponent<Button>().colors;
                    colors.disabledColor = new Color32(200, 200, 200, 170);
                    gameObject.GetComponent<Button>().colors = colors;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Вызов подсказки
        RectTransform rt = GetComponent<RectTransform>();
        if (skill.GetComponent<ActiveSkill>() != null)
        {
            ActiveSkill activeSkill = skill.GetComponent<ActiveSkill>();
            FindObjectOfType<UIManager>().ShowHint(activeSkill.skillName, activeSkill.getDescription());
        }
        else
        {
            PassiveSkill passiveSkill = skill.GetComponent<PassiveSkill>();
            FindObjectOfType<UIManager>().ShowHint(passiveSkill.skillName, passiveSkill.getDescription());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIManager>().HideHint();
    }
}
