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


    public override void InitSkill(ArmyController caster)
    {
        base.InitSkill(caster);

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


public class Bazooking : OneTargetDamageSkill
{
    float range = 30;
    float attackStrength = 4;
    float maxReduction = 0.3f;
    List<ArmyController> enemiesInSight;
    FieldOfView fieldOfView;


    public Bazooking()
    {
        atDistance = true;
        inCombat = false;
        name = "Bazooking";
        energyCost = 0;
        description = "Bazooka distance attack. Each soldier shots a strength <b>" + attackStrength + " <i>physical attack</i></b> ";
        description += "with a <b><i>precision</i></b> decreasing with the enemy distance. The attack consumes half of the unit <b><i>";
        description += "movement</i></b> and cannot be performed if the unit has consumed more than an half of its movement.";
    }


    protected override void TargetChoice()
    {
        if (caster.body.GetComponent<MovementController>().movementLeft < caster.army.movement / 2.0f)
        {
            errorPanel.LaunchErrorText("The unit has already moved more than the half of its movement");
            return;
        }

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
        descPanel.SetArmyChioce(enemiesInSight, UseSkill);
    }


    protected override void SetStats()
    {
        nHits = caster.army.nSoldiers * Mathf.RoundToInt(caster.army.attNumber);
        precision = ComputePrecision();
        attack = attackStrength;
        defense = target.army.phDef;
        damageProb = BattleF.GetHitProbability(attack, target.army.phDef);
    }


    protected override void ModifyDescPanel()
    {
        UIInfo precisionInfo = descPanel.myTexts[2].gameObject.AddComponent<UIInfo>();
        precisionInfo.title = "Precision";
        precisionInfo.infoText = "Base <b><i>precision</i></b> = <b>" + caster.army.precision.ToString("0.00") + "</b>.\n";
        precisionInfo.infoText += "Malus due to the distance = <b>" + (caster.army.precision - precision).ToString("0.00") + "</b>";
        precisionInfo.topDown = true;
    }


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


    float ComputePrecision()
    {
        float distance = Vector3.Distance(caster.transform.position, target.transform.position) - (caster.body.length + target.body.length)/2.0f;
        return caster.army.precision - distance / range * maxReduction;
    }
}


public class SteamBomb : AOESkill
{
    float range = 25;
    float attackStrength = 4;
    List<ArmyController> enemiesInSight;
    FieldOfView fieldOfView;


    public SteamBomb()
    {
        atDistance = true;
        inCombat = false;
        name = "Steam bomb";
        energyCost = 1;
        extAreaRadius = 5;
        internalAreaRadius = 2;
        description = "...DESCRIPTION...";
    }


    public override void UseSkill(List<ArmyController> targets)
    {
        this.targets = targets;
    }
}