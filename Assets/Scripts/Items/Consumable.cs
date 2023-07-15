using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Items
{
    public bool Usable = true;
    public int addHP;
    public int addMP;
    public bool ignorePhysDmg;
    public bool ignoreMagicDmg;

    private void Start()
    {
        if (Usable)
        {
            describtion += "\n";
            if (addHP > 0)
                describtion += "\nВостановление здоровья: " + addHP + " ед.";
            if (addHP < 0)
                describtion += "\nПотеря здоровья: " + System.Math.Abs(addHP) + " ед.";
            if (addMP > 0)
                describtion += "\nВостановление магии: " + addMP + " ед.";
            if (addMP < 0)
                Debug.LogError(name + " зелье не должно отнимать MP!");
            if (ignorePhysDmg)
                describtion += "\nИгнорирование физ. урона";
            if (ignoreMagicDmg)
                describtion += "\nИгнорирование маг. урона";
        }
    }
}
