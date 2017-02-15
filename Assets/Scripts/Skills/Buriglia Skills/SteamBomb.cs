using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SteamBomb : AOEDamageSkill
{
    float range = 25;
    float attackStrength = 4;
    int soldiersPerHit = 4;
    FieldOfView fieldOfView;


    public SteamBomb()
    {
        atDistance = true;
        inCombat = false;
        name = "Steam bomb";
        energyCost = 1;
        nSubAreas = 2;
        extAreaRadius = 5;
        description = "...DESCRIPTION...";
    }


    protected override void MakeTheAreaMove()
    {
        GameObject fieldOfViewGO = caster.GetComponent<AttackController>().fieldOfViewGO;
        fieldOfView = new FieldOfView(caster.transform, fieldOfViewGO, 1000, range, false);
        fieldOfView.DrawArea();
        area.field = fieldOfView;
        area.StartMyMovement(UseSkill, x => fieldOfView.IsInFieldOfView(x.transform.position), "Area not in the field of view");
    }


    protected override bool OtherCastablilityConditions()
    {
        if (caster.body.GetComponent<MovementController>().movementLeft < caster.army.movement / 2.0f)
        {
            errorPanel.LaunchErrorText("The unit has already moved more than the half of its movement");
            return false;
        }
        else return true;
    }


    protected override void ResetTargetChoice()
    {
        base.ResetTargetChoice();
        fieldOfView.DestroyArea();
    }


    protected override void EndSkill()
    {
        base.EndSkill();
        caster.body.GetComponent<MovementController>().MovementLeftReduction(caster.army.movement / 2.0f);
    }


    protected override int GetNHitsInSubArea(int subAreaIndex, int targetIndex)
    {
        return Mathf.RoundToInt((subAreaIndex + 1) * caster.army.nSoldiers / 5f);
    }


    protected override float GetPrecisionInSubArea(int subAreaIndex, int targetIndex)
    {
        float distance = Vector3.Distance(caster.transform.position, area.transform.position);
        return BattleF.ComputePrecision(caster.army.precision, distance);
    }


    protected override float GetAttackInSubArea(int subAreaIndex, int targetIndex) { return attackStrength; }


    protected override float GetDefenseInSubArea(int subAreaIndex, int targetIndex) { return targets[targetIndex].army.elemDef; }


    protected override StatModifier GetStatModInSubArea(int subAreaIndex, int targetIndex)
    {
        return new StatModifier(Stat.phAtt, -1, 1, "Steam bomb malus");
    }


    protected override string hitsGenerationText
    {
        get
        {
            string text = "N bazookers: <b>" + caster.army.nSoldiers + "</b>\n";
            text += "the skill causes one hit within the external area and two hits in the internal one for each " + soldiersPerHit + " soldiers .";
            return text + "\nTotal N hits:";
        }
    }

    protected override string precisionText
    {
        get
        {
            string text = "Soldier precision: <b>" + caster.army.precision.ToString("0.00") + "</b>\n";
            text += "Distance precision malus: <b>" + (caster.army.precision - precision[0]).ToString("0.00") + "</b>\n";
            return text + "Effective precision:";
        }
    }

    protected override string attackStrengthText { get { return "Shot strength (elemental):"; } }

    protected override string defenseText { get { return "Target elemental defense:"; } }
}