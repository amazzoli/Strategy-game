using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeAttack : OneTargetDamageSkill
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
            descPanel.SetArmyChioce(engagedEnemies, UseSkill);
            foreach (ArmyController armyCtrl in engagedEnemies)
                armyCtrl.body.MarkAsAttackable(caster, UseSkill, false);
        }
        else
            UseSkill(engagedEnemies[0]);
    }


    protected override void SetStats()
    {
        nHits = caster.army.nSoldiers * Mathf.RoundToInt(caster.army.attNumber);
        precision = caster.army.precision;
        attack = caster.army.phAtt;
        defense = target.army.phDef;
        damageProb = BattleF.GetHitProbability(caster.army.phAtt, target.army.phDef);
    }


    protected override void ResetTargetChoice()
    {
        foreach (ArmyController armyCtrl in engagedEnemies)
            armyCtrl.body.UnmarkAsAttackable();
    }
}
