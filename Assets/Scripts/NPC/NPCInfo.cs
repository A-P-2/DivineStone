using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    public string NPCName;
    public bool isAggressive = false;
    public bool canRespawn = false;

    [Header("Reward for killing")]
    public int money;
    public ItemsAmount[] items;
    public MainQuest.Villages killingPoint;

    [Header("Quests manipulation for killing")]
    public string[] questsStart;
    public string[] questsNextStage;
    public string[] questsComplite;
    public string[] questsFail;

    private bool canSpeak = false;
    private bool canFight = false;
    private bool isTrader = false;

    private bool playerInRange = false;

    private GameObject selfHint = null;

    private void Start()
    {
        if (GetComponent<DialogSystem>() != null) canSpeak = true;
        if (GetComponent<NPCFighter>() != null) canFight = true;
        if (GetComponent<NPCTrader>() != null) isTrader = true;

        if (canSpeak && isTrader)
            Debug.LogError("NPC \"" + NPCName + "\" " + gameObject.transform.position + " может одновременно и говорить и торговать!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!(isAggressive && canFight))
            {
                string hint = "";
                if (canSpeak) hint += "(E) Говорить\n";
                if (isTrader && !isAggressive) hint += "(E) Торговать\n";
                hint += "(Q) Атаковать";

                selfHint = FindObjectOfType<UIPopupMessage>().ShowObjectHint(transform.position, hint);
            }
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FindObjectOfType<UIPopupMessage>().HideHint(selfHint);
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && OpenWorldManager.GameStatusIsOpenWorld())
        {
            if (isAggressive && canFight)
            {
                if (canSpeak) GetComponent<DialogSystem>().StartDialog(true, true);
                else
                {
                    DataTransfer.StartFight(GetComponent<NPCInfo>().NPCName, GetComponent<SpriteRenderer>().sprite, GetComponent<NPCFighter>());
                }
            }
            else if (isTrader && Input.GetKeyDown(KeyCode.E) && !isAggressive)
            {
                FindObjectOfType<UITrade>().OpenTrade(GetComponent<NPCTrader>());
            }
            else if (canSpeak && Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<DialogSystem>().StartDialog(isAggressive, canFight);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                if (transform.parent != null)
                {
                    Village village = transform.parent.GetComponent<Village>();
                    if (village != null) village.ChangeNPCStatus(true);
                }

                if (canFight)
                {
                    DataTransfer.StartFight(GetComponent<NPCInfo>().NPCName, GetComponent<SpriteRenderer>().sprite, GetComponent<NPCFighter>());
                }
                else Die();
            }
        }
    }

    public void Die()
    {
        SoundManager.GetSound("kill").audioSource.Play();
        SoundManager.PauseMusic();
        SoundManager.PlayBGMusic(5f);

        Inventory inventory = FindObjectOfType<Inventory>();
        QuestList questList = FindObjectOfType<QuestList>();

        inventory.changeMoney(money);
        foreach (ItemsAmount item in items)
            inventory.receiveItem(item.ID, item.amount);

        foreach (string questID in questsStart)
            questList.GetQuest(questID).StartQuest();
        foreach (string questID in questsNextStage)
            questList.GetQuest(questID).NextStage();
        foreach (string questID in questsComplite)
            questList.GetQuest(questID).CompliteQuest();
        foreach (string questID in questsFail)
            questList.GetQuest(questID).FailQuest();

        if (canFight)
            FindObjectOfType<PlayerStats>().addExp(GetComponent<NPCFighter>().exp);

        FindObjectOfType<MainQuest>().AddPoint(killingPoint, false);

        if (canRespawn) gameObject.SetActive(false);
        else Destroy(gameObject);
    }
}
