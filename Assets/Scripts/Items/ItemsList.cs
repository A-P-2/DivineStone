using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsList : MonoBehaviour
{
    private Dictionary<string, GameObject> listing = new Dictionary<string, GameObject>();

    private void dictAdd(GameObject child)
    {
        string Identificator;
        GameObject objectGame;

        objectGame = child.gameObject;
        Identificator = objectGame.GetComponent<Items>().ID;
        if (listing.ContainsKey(Identificator))
        {
            Debug.LogError("Предметный ID \"" + Identificator + "\" повторяется несколько раз!");
        }
        else
        {
            listing.Add(Identificator, objectGame);
        }
    }

    private void Awake()
    {
        listing.Clear();
        foreach (Transform child in transform)
        {
            dictAdd(child.gameObject);
        }
    }

    public Items getCommonInfo(string ID)
    {
        if (!(listing.ContainsKey(ID)))
        {
            Debug.LogError("Предмета с ID " + ID + " нет в списке!");
        }

        return listing[ID].GetComponent<Items>();
    }

    public Vector2Int getArmorInfo(string ID)
    {
        if (!(listing.ContainsKey(ID)))
        {
            Debug.LogError("Предмета с ID " + ID + " нет в списке!");
        }

        Items.Type typeInfo = listing[ID].GetComponent<Items>().type;

        if (typeInfo != Items.Type.armor)
        {
            Debug.LogError("Предмет с ID " + ID + " не является броней!");
        }

        Armory armor = listing[ID].GetComponent<Armory>();
        int physDefInfo = armor.physDmgResist;
        int magicDefInfo = armor.magicDmgResist;
        return new Vector2Int(physDefInfo, magicDefInfo);
    }

    public Vector2Int getWeaponInfo(string ID)
    {
        if (!(listing.ContainsKey(ID)))
        {
            Debug.LogError("Предмета с ID " + ID + " нет в списке!");
        }

        Items.Type typeInfo = listing[ID].GetComponent<Items>().type;

        if (typeInfo != Items.Type.weapon)
        {
            Debug.LogError("Предмет с ID " + ID + " не является оружием!");
        }

        Weapons weapon = listing[ID].GetComponent<Weapons>();
        int minPhysInfo = weapon.minPhysDmg;
        int maxPhysInfo = weapon.maxPhysDmg;
        int minMagicInfo = weapon.minMagicDmg;
        int maxMagicInfo = weapon.maxMagicDmg;
        int critInfo = weapon.critChance;
        int actualPhysDmg = Random.Range(minPhysInfo, maxPhysInfo+1);
        int actualMagicDmg = Random.Range(minMagicInfo, maxMagicInfo+1);
        int randomizeCrit = Random.Range(1, 101);

        if (randomizeCrit <= critInfo)
        {
                actualPhysDmg *= 2;
                actualMagicDmg *= 2;
        }

        return new Vector2Int(actualPhysDmg, actualMagicDmg);
    }

    public bool UseConsumable(string ID)
    {
        if (!(listing.ContainsKey(ID)))
        {
            Debug.LogError("Предмета с ID " + ID + " нет в списке!");
        }

        Items.Type typeInfo = listing[ID].GetComponent<Items>().type;

        if (typeInfo != Items.Type.consumable)
        {
            Debug.LogError("Предмет с ID " + ID + " не является расходником!");
        }

        Consumable thing = listing[ID].GetComponent<Consumable>();
        bool flag = thing.Usable;
        int hpInfo = thing.addHP;
        int mpInfo = thing.addMP;
        bool ignorePhysInfo = thing.ignorePhysDmg;
        bool ignoreMagicInfo = thing.ignoreMagicDmg;

        if (flag)
        {
            PlayerStats playerStats;
            if (DataTransfer.status == DataTransfer.Status.openWorld)
            {
                playerStats = FindObjectOfType<PlayerStats>();
            }
            else
            {
                playerStats = DataTransfer.playerStats;
            }

            if (hpInfo != 0)
            {
                playerStats.changeHP(hpInfo);
            }

            if (mpInfo != 0)
            {
                playerStats.changeMP(mpInfo);
            }

            if (ignorePhysInfo)
            {
                playerStats.ignorePhysDmg = true;
            }

            if (ignoreMagicInfo)
            {
                playerStats.ignoreMagicDmg = true;
            }
        }
        return flag;
    }
}