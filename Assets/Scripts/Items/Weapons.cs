using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : Items
{
    public int minPhysDmg;
    public int maxPhysDmg;
    public int minMagicDmg;
    public int maxMagicDmg;
    [Range(0, 100)]
    public int critChance;

    private void Start()
    {
        describtion += "\n";
        if (maxPhysDmg != 0)
            describtion += "\nФизический урон: " + minPhysDmg + "-" + maxPhysDmg;
        if (maxMagicDmg != 0)
            describtion += "\nМагический урон: " + minMagicDmg + "-" + maxMagicDmg;
        describtion += "\nШанс крит. урона: " + critChance + "%";
    }
}
