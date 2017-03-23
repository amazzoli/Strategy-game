using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExplosiveShells : MeleeAttack
{
    float attackIncrement = 1.5f;
    float autoDamageStrenght = 3, autoDamagePrecision = 20;


    public ExplosiveShells()
    {
        atDistance = false;
        inCombat = true;
        name = "Explosive shells";
        energyCost = 1;
        description = "The fighter shells are filled with gunpowder and create dangerous explosions while attacking. ";
        description += "The attack strenght increases by <b>" + attackIncrement + "</b> causing fire damage.";
        description += "However, this is a risky move, indeed the unit receive a fire damage due to the runaway explosions ";
        description += "of strength <b>" + autoDamageStrenght + "</b> and precision <b>" + autoDamagePrecision + "</b>";

    }


    protected override void SetStats()
    {
        nHits.Add(caster.army.nSoldiers * Mathf.RoundToInt(caster.army.attNumber));
        precision.Add(caster.army.precision);
        attack.Add(caster.army.phAtt + autoDamageStrenght);
        defense.Add(targets[0].army.elemDef(Element.fire));
        damageProb.Add(BattleF.GetHitProbability(attack[0], defense[0]));
    }


    public override void GenerateHitsAndWounds()
    {
        int nHitsOnMe = BattleF.GetSuccessfulEvents(caster.army.nSoldiers, autoDamagePrecision * 0.01f);
        float damageProbOnMe = BattleF.GetHitProbability(autoDamageStrenght, caster.army.elemDef(Element.fire));
        int nWoundsOnMe = BattleF.GetSuccessfulEvents(nHitsOnMe, damageProbOnMe);
        caster.SetWoundDamage(nWoundsOnMe, caster, name);
        base.GenerateHitsAndWounds();
    }

    protected override string attackStrengthText { get { return "Fire attack strength:"; } }


    protected override string defenseText { get { return "Target elemental defense (fire):"; } }
}