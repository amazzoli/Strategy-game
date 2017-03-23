using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DazzlingShells : MeleeAttack
{
    float precisionDamage = 15;
    int malusTurns = 2;


    public DazzlingShells()
    {
        atDistance = false;
        inCombat = true;
        name = "Dazzling shells";
        energyCost = 1;
        description = "The fighters shells are filled with light energy, trapped in their light cristals.";
        description += "In addition to the melee attack damage, the skill reduces the enemy <b><i>precision</i></b> by ";
        description += "<b>" + precisionDamage + "%</b> for " + malusTurns + " turns";

    }


    protected override void SetStats()
    {
        base.SetStats();
        statMods.Add(new StatModifier(Stat.precision, -precisionDamage, malusTurns, name));
    }
}