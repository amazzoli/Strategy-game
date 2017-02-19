using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class BubbleCannon : AOEDamageSkill
{
    float range = 25;
    float attackStrength = 2;
    float precisionDebuffExternal = 5, movementDebuffExternal = 2.5f;
    float precisionDebuffInternal = 10, movementDebuffInternal = 5;
    int debuffTurns = 2;
    FieldOfView fieldOfView;


    public BubbleCannon()
    {
        atDistance = true;
        inCombat = false;
        name = "Bubble cannon";
        energyCost = 1;
        nSubAreas = 2;
        extAreaRadius = 5;
        description =  "A bubble waterfall fall from above, creating confusion and desease among the opponents. AOE distant attack ";
        description += "inflicting <b>" + attackStrength.ToString("0.##") + " <i>water elemental damage</i></b> per bazooker shot. ";
        description += "The enemies in the internal area suffer a <b>" + precisionDebuffInternal.ToString("0.#") + "% <i>precision</i></b> debuff and a <b>";
        description += movementDebuffInternal.ToString("0.#") + " <i>movement</i></b> debuff for " + debuffTurns + " turns. ";
        description += "In the external subarea the <b><i>precision</i></b> reduces by <b>" + precisionDebuffExternal.ToString("0.#");
        description += "%</b> and <b><i>movement</i></b> by <b>" + movementDebuffExternal.ToString("0.#") + "</b>.";
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
        return caster.army.nSoldiers;
    }


    protected override float GetPrecisionInSubArea(int subAreaIndex, int targetIndex)
    {
        float distance = Vector3.Distance(caster.transform.position, area.transform.position);
        return BattleF.ComputePrecision(caster.army.precision, distance);
    }


    protected override float GetAttackInSubArea(int subAreaIndex, int targetIndex) { return attackStrength; }


    protected override float GetDefenseInSubArea(int subAreaIndex, int targetIndex)
    {
        return targets[targetIndex].army.elemDef(Element.water);
    }


    protected override StatModifier GetStatModInSubArea(int subAreaIndex, int targetIndex)
    {
        if (subAreaIndex == 0)
        {
            List<Stat> stats = new List<Stat> { Stat.movement, Stat.precision };
            List<float> damages = new List<float> { -movementDebuffExternal, -precisionDebuffExternal };
            StatModifier mod = new StatModifier(stats, damages, debuffTurns, name);
            return mod;
        }
        else if (subAreaIndex == 1)
        {
            List<Stat> stats = new List<Stat> { Stat.movement, Stat.precision };
            List<float> damages = new List<float> { -movementDebuffInternal, -precisionDebuffInternal };
            StatModifier mod = new StatModifier(stats, damages, debuffTurns, name);
            return mod;
        }
        else return null;
    }


    protected override string hitsGenerationText
    {
        get
        {
            string text = "N bazookers: <b>" + caster.army.nSoldiers + "</b>\n";
            text += "N shots per soldier: <b>" + caster.army.attNumber + "</b>\n";
            return text + "Total N hits:";
        }
    }

    protected override string precisionText
    {
        get
        {
            string text = "Soldier precision: <b>" + caster.army.precision.ToString("0.#") + "%</b>\n";
            text += "Distance precision malus: <b>" + (caster.army.precision - precision[0]).ToString("0.#") + "%</b>\n";
            return text + "Effective precision:";
        }
    }

    protected override string attackStrengthText { get { return "Hit strength (water element):"; } }

    protected override string defenseText
    {
        get
        {
            return "Target water defense:";
        }
    }
}