using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainQuest : MonoBehaviour
{
    [System.Serializable]
    public enum Villages
    {
        None,
        Wyvhelm,
        Eghal,
        Roothearth
    }

    [Header("All text files")]
    [SerializeField] private Object intro = null;
    [SerializeField] private Object startDialog = null;
    [SerializeField] private Object endDialogNoBrainwash = null;
    [SerializeField] private Object endDialogFewBrainwash = null;
    [SerializeField] private Object endDialogManyBrainwash = null;
    [SerializeField] private Object epilogHero = null;
    [SerializeField] private Object epilogBrainwasher = null;
    [SerializeField] private Object epilogKiller = null;

    [Header("Parametrs")]
    public int questPointsWyvhelm = 0;
    public int killingPointsWyvhelm = 0;
    public int pointsNeededWyvhelm = 0;

    public int questPointsEghal = 0;
    public int killingPointsEghal = 0;
    public int pointsNeededEghal = 0;

    public int questPointsRoothearth = 0;
    public int killingPointsRoothearth = 0;
    public int pointsNeededRoothearth = 0;

    public int brainwashPoints = 0;

    [Header("Other")]
    [SerializeField] private Image blackScreen = null;
    [SerializeField] private NPCFighter weakDemon = null;
    [SerializeField] private NPCFighter toughDemon = null;
    [SerializeField] private NPCFighter strongDemon = null;
    [SerializeField] private Sprite demonSprite = null;
    [SerializeField] private SpriteRenderer demonImage = null;

    public void StartMainQuest()
    {
        StartCoroutine("StartingDialog");
    }

    private IEnumerator StartingDialog()
    {
        SoundManager.PlaySpecialMusic("DemonTheme");

        DialogSystem dialogSystem = FindObjectOfType<DialogSystem>();
        UIDialogs uiDialogs = FindObjectOfType<UIDialogs>();

        Queue<Dialog> dialog = dialogSystem.GetDialog(intro);
        uiDialogs.StartDialog(dialog);

        while (OpenWorldManager.GameStatusIsDialogRun()) yield return null;

        StartCoroutine("HideBlackScreen");

        dialog = dialogSystem.GetDialog(startDialog);
        uiDialogs.StartDialog(dialog);

        StartCoroutine("ShowDemon");

        while (OpenWorldManager.GameStatusIsDialogRun()) yield return null;
        OpenWorldManager.gameStatus = OpenWorldManager.GameStatus.openWorld;

        QuestList questList = FindObjectOfType<QuestList>();
        questList.GetQuest("MainQuestWyvhelm").StartQuest();
        questList.GetQuest("MainQuestEghal").StartQuest();
        questList.GetQuest("MainQuestRoothearth").StartQuest();

        StartCoroutine("HideDemon");

        SoundManager.PlayBGMusic();
    }

    private IEnumerator HideBlackScreen()
    {
        blackScreen.color = new Color32(0, 0, 0, 1);
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            if (alpha < 0) alpha = 0;

            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.gameObject.SetActive(false);
    }

    private IEnumerator HideDemon()
    {
        demonImage.color = new Color(1, 1, 1, 1);
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            if (alpha < 0) alpha = 0;

            demonImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        demonImage.gameObject.SetActive(false);
    }

    private IEnumerator ShowDemon()
    {
        demonImage.color = new Color(1, 1, 1, 0);
        demonImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * 2;
            if (alpha > 1) alpha = 1;

            demonImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
    }

    public void AddPoint(Villages village, bool isQuestPoint)
    {
        QuestList questList = FindObjectOfType<QuestList>();
        UIPopupMessage uiPopupMessage = FindObjectOfType<UIPopupMessage>();

        if (village == Villages.Wyvhelm)
        {
            if (isQuestPoint) questPointsWyvhelm++;
            else killingPointsWyvhelm++;

            if (questPointsWyvhelm * 2 + killingPointsWyvhelm == pointsNeededWyvhelm)
            {
                questList.GetQuest("MainQuestWyvhelm").NextStage();
                uiPopupMessage.ShowSideHint("Отшельник готов сразиться с големом Виверн!");
            }
        }
        else if (village == Villages.Eghal)
        {
            if (isQuestPoint) questPointsEghal++;
            else killingPointsEghal++;

            if (questPointsEghal * 2 + killingPointsEghal == pointsNeededEghal)
            {
                questList.GetQuest("MainQuestEghal").NextStage();
                uiPopupMessage.ShowSideHint("Отшельник готов сразиться с големом Сфинксиансов!");
            }
        }
        else if (village == Villages.Roothearth)
        {
            if (isQuestPoint) questPointsRoothearth++;
            else killingPointsRoothearth++;

            if (questPointsRoothearth * 2 + killingPointsRoothearth == pointsNeededRoothearth)
            {
                questList.GetQuest("MainQuestRoothearth").NextStage();
                uiPopupMessage.ShowSideHint("Отшельник готов сразиться с големом Эльфов!");
            }
        }
    }

    public void StartLastBossFight()
    {
        StartCoroutine("FinalDialog");
    }

    private IEnumerator FinalDialog()
    {
        SoundManager.PlaySpecialMusic("DemonTheme");

        DialogSystem dialogSystem = FindObjectOfType<DialogSystem>();
        UIDialogs uiDialogs = FindObjectOfType<UIDialogs>();
        Inventory inventory = FindObjectOfType<Inventory>();

        inventory.loseItem("GolemHeartWyvhelm", 1, false);
        inventory.loseItem("GolemHeartEghal", 1, false);
        inventory.loseItem("GolemHeartRoothearth", 1, false);

        Queue<Dialog> dialog;
        NPCFighter demon;
        if (brainwashPoints < 2)
        {
            dialog = dialogSystem.GetDialog(endDialogNoBrainwash);
            demon = weakDemon;
        }
        else if (brainwashPoints < 6)
        {
            dialog = dialogSystem.GetDialog(endDialogFewBrainwash);
            demon = toughDemon;
        }
        else
        {
            dialog = dialogSystem.GetDialog(endDialogManyBrainwash);
            demon = strongDemon;
        }

        StartCoroutine("ShowDemon");

        uiDialogs.StartDialog(dialog);

        while (OpenWorldManager.GameStatusIsDialogRun()) yield return null;

        DataTransfer.StartFight("Архидемон", demonSprite, demon, true);
    }

    public void StartEpilog()
    {
        blackScreen.gameObject.SetActive(true);
        blackScreen.color = new Color(0, 0, 0, 1);

        StartCoroutine("ShowEpilog");
    }

    private IEnumerator ShowEpilog()
    {
        DialogSystem dialogSystem = FindObjectOfType<DialogSystem>();
        UIDialogs uiDialogs = FindObjectOfType<UIDialogs>();

        int killCount = killingPointsEghal + killingPointsRoothearth + killingPointsWyvhelm;
        int questCount = questPointsEghal + questPointsRoothearth + questPointsWyvhelm;

        Queue<Dialog> dialog;
        if (questCount > brainwashPoints * 2 && questCount > killCount)
        {
            SoundManager.PlaySpecialMusic("HeroTheme");
            dialog = dialogSystem.GetDialog(epilogHero);
        }
        else if (brainwashPoints > killCount)
        {
            SoundManager.PlaySpecialMusic("DemonTheme");
            dialog = dialogSystem.GetDialog(epilogBrainwasher);
        }
        else
        {
            SoundManager.PlaySpecialMusic("KillerTheme");
            dialog = dialogSystem.GetDialog(epilogKiller);
        }

        uiDialogs.StartDialog(dialog);
        while (OpenWorldManager.GameStatusIsDialogRun()) yield return null;

        SceneManager.LoadScene(0);
    }

    public bool CanAttackGolem(Villages cityName)
    {
        if (cityName == Villages.Wyvhelm)
        {
            return (questPointsWyvhelm * 2 + killingPointsWyvhelm >= pointsNeededWyvhelm);
        }
        else if (cityName == Villages.Eghal)
        {
            return (questPointsEghal * 2 + killingPointsEghal >= pointsNeededEghal);
        }
        else if (cityName == Villages.Roothearth)
        {
            return (questPointsRoothearth * 2 + killingPointsRoothearth >= pointsNeededRoothearth);
        }
        return false;
    }

    public bool AlreadyKillGolem(Villages cityName)
    {
        if (cityName == Villages.Wyvhelm)
        {
            return (FindObjectOfType<Inventory>().haveItems("GolemHeartWyvhelm") > 0);
        }
        else if (cityName == Villages.Eghal)
        {
            return (FindObjectOfType<Inventory>().haveItems("GolemHeartEghal") > 0);
        }
        else if (cityName == Villages.Roothearth)
        {
            return (FindObjectOfType<Inventory>().haveItems("GolemHeartRoothearth") > 0);
        }
        return false;
    }
}
