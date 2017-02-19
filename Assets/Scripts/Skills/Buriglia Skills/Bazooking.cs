using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Bazooking : DamageSkill
{
    float range = 30;
    float attackStrength = 4;
    List<ArmyController> enemiesInSight;
    FieldOfView fieldOfView;


    public Bazooking()
    {
        atDistance = true;
        inCombat = false;
        name = "Bazooking";
        energyCost = 0;
        description = "Bazooka distance attack. Each soldier shots a strength <b>" + attackStrength.ToString("0.##") + " <i>physical attack</i></b> ";
        description += "with a <b><i>precision</i></b> decreasing with the enemy distance. The attack consumes half of the unit <b><i>";
        description += "movement</i></b> and cannot be performed if the unit has consumed more than an half of its movement.";
    }


    protected override void TargetChoice()
    {
        GameObject fieldOfViewGO = caster.GetComponent<AttackController>().fieldOfViewGO;
        fieldOfView = new FieldOfView(caster.transform, fieldOfViewGO, 1000, range, false);
        fieldOfView.DrawArea();

        enemiesInSight = new List<ArmyController>();
        foreach (InSightArmy army in fieldOfView.armiesInSight)
            if (!army.isAllied)
            {
                army.armyCtrl.body.MarkAsAttackable(army.armyCtrl, UseSkill, false);
                enemiesInSight.Add(army.armyCtrl);
            }
        descPanels.choicePanel.MakePanel(enemiesInSight, UseSkill);
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


    protected override void SetStats()
    {
        nHits.Add(caster.army.nSoldiers * Mathf.RoundToInt(caster.army.attNumber));
        float distance = Vector3.Distance(caster.transform.position, targets[0].transform.position) - (caster.body.length + targets[0].body.length) / 2.0f;
        precision.Add(BattleF.ComputePrecision(caster.army.precision, distance));
        attack.Add(attackStrength);
        defense.Add(targets[0].army.phDef);
        damageProb.Add(BattleF.GetHitProbability(attack[0], targets[0].army.phDef));
    }


    //protected override void ModifyDescPanel()
    //{
    //    UIInfo precisionInfo = descPanel.myTexts[2].gameObject.AddComponent<UIInfo>();
    //    precisionInfo.title = "Precision";
    //    precisionInfo.infoText = "Base <b><i>precision</i></b> = <b>" + caster.army.precision.ToString("0.00") + "</b>.\n";
    //    precisionInfo.infoText += "Malus due to the distance = <b>" + (caster.army.precision - precision).ToString("0.00") + "</b>";
    //    precisionInfo.topDown = true;
    //}


    protected override void ResetTargetChoice()
    {
        foreach (ArmyController armyCtrl in enemiesInSight)
            armyCtrl.body.UnmarkAsAttackable();
        fieldOfView.DestroyArea();
        enemiesInSight.Clear();
    }


    protected override void EndSkill()
    {
        base.EndSkill();
        caster.body.GetComponent<MovementController>().MovementLeftReduction(caster.army.movement / 2.0f);
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

    protected override string attackStrengthText { get { return "Shot attack strength:"; } }

    protected override string defenseText { get { return "Target physical defense:"; } }
}