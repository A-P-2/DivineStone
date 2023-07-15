using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemsAmount
{
    public string ID;
    public int amount;
}

public class Items : MonoBehaviour
{
    public string ID;
    public string itemName;
    [TextArea]
    public string describtion;
    public Sprite sprite;
    public int cost;
    public Type type;
    
    public enum Type
    {
        armor,
        weapon,
        questItem,
        rubbish,
        consumable
    }
}
