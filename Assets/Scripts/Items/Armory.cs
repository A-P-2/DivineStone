using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armory : Items
{
    [Range(0, 100)]
    public int physDmgResist;
    [Range(0, 100)]
    public int magicDmgResist;

    private void Start()
    {
        describtion += "\n";
        describtion += "\nСопротивляемость физ. урону: " + physDmgResist + "%";
        describtion += "\nСопротивляемость маг. урону: " + magicDmgResist + "%";
    }
}
