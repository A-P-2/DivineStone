using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFight : MonoBehaviour
{
    private class ActionText
    {
        public string text;
        public bool player;

        public ActionText(string _text, bool _player)
        {
            text = _text;
            player = _player;
        }
    }
    [SerializeField] private GameObject buttonRun = null;

    [SerializeField] private Transform actionTextPanel = null;
    [SerializeField] private GameObject playerActionText = null;
    [SerializeField] private GameObject enemyActionText = null;
    [SerializeField] private ScrollRect actionScroll = null;
    [SerializeField] private Text nextTurnText = null;
    [SerializeField] private GameObject[] backGround = new GameObject[0];
    [SerializeField] private Button[] buttons = new Button[0];
    [SerializeField] private GameObject hint = null;
    [SerializeField] private GameObject inventory = null;
    [SerializeField] private GameObject skillList = null;

    [Header("Player")]
    [SerializeField] private Text playerLevelText = null;
    [SerializeField] private Text playerHPText = null;
    [SerializeField] private Text playerMPText = null;
    [SerializeField] private Text playerEffectText = null;

    [Header("Enemy")]
    [SerializeField] private Text enemyNameText = null;
    [SerializeField] private Text enemyLevelText = null;
    [SerializeField] private Text enemyHPText = null;
    [SerializeField] private Text enemyEffectText = null;
    [SerializeField] private Image enemySprite = null;

    private FightManager fightManager;

    private bool canUseActionText = false;
    private float actionTextTimer = 0;
    public bool actionTextTimerActive = false;
    private Queue<ActionText> actionTextQueue = new Queue<ActionText>();

    private void Start()
    {
        if (DataTransfer.status == DataTransfer.Status.finalFight) Destroy(buttonRun);

        fightManager = FindObjectOfType<FightManager>();

        canUseActionText = true;

        backGround[DataTransfer.npcFighter.backgroundNumber].SetActive(true);

        playerLevelText.text = DataTransfer.playerStats.level.ToString();
        enemyNameText.text = DataTransfer.enemyName;
        enemyLevelText.text = DataTransfer.npcFighter.level.ToString();
        enemySprite.sprite = DataTransfer.enemySprite;

        inventory.SetActive(false);
        skillList.SetActive(false);
        hint.SetActive(false);
    }

    public void UpdateAllTextFields()
    {
        if (fightManager == null) fightManager = FindObjectOfType<FightManager>();

        playerHPText.text = DataTransfer.playerStats.currentHP + "/" + DataTransfer.playerStats.maxHP;
        playerMPText.text = DataTransfer.playerStats.currentMP + "/" + DataTransfer.playerStats.maxMP;
        playerEffectText.text = fightManager.player.effectName;

        enemyHPText.text = DataTransfer.npcFighter.HP + "/" + DataTransfer.npcFighter.maxHP;
        enemyEffectText.text = fightManager.enemy.effectName;

        nextTurnText.text = "Следующий ход: " + fightManager.nextTurnName;
    }

    public void AddInfoToActionText(string info)
    {
        if (fightManager == null) fightManager = FindObjectOfType<FightManager>();

        info = "[" + fightManager.turnNumber + "] " + info;

        ActionText actionText = new ActionText(info, 
            (fightManager.currentTurnName == "Отшельник" || fightManager.currentTurnName == "") ? true : false);

        actionTextQueue.Enqueue(actionText);
        if (!actionTextTimerActive)
        {
            actionTextTimerActive = true;
            StartCoroutine("AddActionText");
        }
    }

    private IEnumerator AddActionText()
    {
        while (true)
        {
            while (actionTextTimer > Time.time) yield return null;
            while (!canUseActionText) yield return null;

            if (actionTextQueue.Count == 0)
            {
                actionTextTimerActive = false;
                break;
            }

            actionTextTimer = Time.time + 1f;

            GameObject tempText = (actionTextQueue.Peek().player) 
                ? Instantiate(playerActionText) : Instantiate(enemyActionText);
            tempText.transform.SetParent(actionTextPanel);
            tempText.transform.localScale = Vector3.one;
            tempText.GetComponent<Text>().text = actionTextQueue.Dequeue().text;

            yield return null;
            actionScroll.verticalNormalizedPosition = 0;
        }
    }

    public void SetAllButtonActive(bool activate)
    {
        foreach (Button button in buttons)
            if (button != null)
            {
                button.interactable = activate;
            }
    }

    public void PhysAttack()
    {
        fightManager.SimpleAttack(true);
    }

    public void MagAttack()
    {
        fightManager.SimpleAttack(false);
    }

    public void PhysBlock()
    {
        fightManager.PutBlock(true);
    }

    public void MagBlock()
    {
        fightManager.PutBlock(false);
    }

    public void ShowEscapeHint()
    {
        ShowHint("Попытка побега", fightManager.EscapeChanceDescription());
    }

    public void ShowHint(string name, string description)
    {
        foreach (Transform child in hint.transform)
        {
            if (child.gameObject.name == "Title")
            {
                child.gameObject.GetComponent<Text>().text = name;
            }
            else
            {
                child.gameObject.GetComponent<Text>().text = description;
            }
        }

        hint.transform.localPosition = new Vector2(-1000, 1000);
        hint.SetActive(true);
    }

    public void HideHint()
    {
        hint.SetActive(false);
    }

    public void OpenUIInventory()
    {
        inventory.SetActive(true);
    }

    public void CloseUIInventory()
    {
        inventory.SetActive(false);
    }

    public void OpenUISkillList()
    {
        skillList.SetActive(true);
    }

    public void CloseUISkillList()
    {
        skillList.SetActive(false);
    }
}
