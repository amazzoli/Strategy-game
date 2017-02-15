using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeleeAttack : DamageSkill
{
    List<ArmyController> engagedEnemies;


    public MeleeAttack()
    {
        atDistance = false;
        inCombat = true;
        energyCost = 0;
        name = "Melee Attack";
        description = "Each soldier performs <i><b>N attacks</b></i> against the defender. ";
        description += "Each hit has a probability to strike equal to the army <i><b>precision</b></i>, ";
        description += "and every succesful hit can causes wound according to the unit <i><b>physical attack</b></i> ";
        description += "and the enemy <i><b>physical defence</b></i>.";
    }


    protected override void TargetChoice()
    {
        engagedEnemies = caster.enemiesEngaged.list;
        if (engagedEnemies.Count > 1)
        {
            descPanels.choicePanel.MakePanel(engagedEnemies, UseSkill);
            foreach (ArmyController armyCtrl in engagedEnemies)
                armyCtrl.body.MarkAsAttackable(caster, UseSkill, false);
        }
        else
            UseSkill(engagedEnemies[0]);
    }


    protected override void SetStats()
    {
        nHits.Add(caster.army.nSoldiers * Mathf.RoundToInt(caster.army.attNumber));
        precision.Add(caster.army.precision);
        attack.Add(caster.army.phAtt);
        defense.Add(targets[0].army.phDef);
        damageProb.Add(BattleF.GetHitProbability(caster.army.phAtt, targets[0].army.phDef));
    }


    protected override void ResetTargetChoice()
    {
        foreach (ArmyController armyCtrl in engagedEnemies)
            armyCtrl.body.UnmarkAsAttackable();
    }


    protected override string hitsGenerationText
    {
        get
        {
            string text = "N soldiers: <b>" + caster.army.nSoldiers + "</b>\n";
            text += "N attacks per soldier: <b>" + caster.army.attNumber + "</b>\n";
            return text + "Total hits:";
        }
    }

    protected override string precisionText { get { return "Attack precision:"; } }

    protected override string attackStrengthText { get { return "Physical attack:"; } }

    protected override string defenseText { get { return "Target physical defense:"; } }
}
