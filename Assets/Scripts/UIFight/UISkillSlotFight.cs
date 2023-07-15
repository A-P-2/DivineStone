using UnityEngine;
using System.Timers;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlotFight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public ActiveSkill activeSkill;

    private void OnEnable()
    {
        if (activeSkill.ableToUse())
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<UIFight>().ShowHint(activeSkill.skillName, activeSkill.getDescription(false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<UIFight>().HideHint();
    }
}