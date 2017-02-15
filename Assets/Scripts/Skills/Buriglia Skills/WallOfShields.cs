using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WallOfShields : Skill
{
    public WallOfShields()
    {
        atDistance = true;
        inCombat = true;
        name = "Wall of shields";
        energyCost = 1;
        description = "Solid defensive formations which increases the unit <i><b>physical defense</b></i> and";
        description += "<i><b>mental strength</b></i> by <b>1</b> each. It consumes <b>1</b> <b><i><color=#FF4500>";
        description += "energy</color></i></b>. After the action the unit cannot move anymore.";
    }


    protected override void StartSkill()
    {
        foreach (StatModifier statMod in caster.army.statMods)
            if (statMod.title == name)
            {
                statMod.turnsLeft++;
                base.EndSkill();
                return;
            }
        List<Stat> stats = new List<Stat>() { Stat.mentalStrength, Stat.phDef };
        List<float> values = new List<float>() { 1, 1 };
        caster.AddStatModifier(stats, values, 2, name, caster);
        caster.body.GetComponent<MovementController>().movementLeft = 0;
        caster.skillUsed = true;
        base.EndSkill();
    }
}