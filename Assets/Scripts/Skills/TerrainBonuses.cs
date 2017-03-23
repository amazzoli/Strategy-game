using UnityEngine;
using System.Collections.Generic;
using System.Collections;


class WoodBonus: PassiveSkill
{
    float precisionReduction = 10;


    public WoodBonus()
    {
        title = "Wood bonus";
        symbolName = "wood";
        description = "All the attacks against the army reduces their precision by " + precisionReduction + "%. ";
        description += "Moreover if the army is charged, it  does not suffer any morale damage and the enemy physical attack bonus is denied.";
    }


    public override void DamageSkillStatModifier(DamageSkill skill, int targetIndex)
    {
        skill.precision[targetIndex] -= precisionReduction;;
        if (skill.precision[targetIndex] < 0)
            skill.precision[targetIndex] = 0;
    }


    public override string DamageSkillStatModText(DamageSkill skill, int targetIndex)
    {
        return "\n\n<b><color=green>Precision reduced by " + precisionReduction.ToString("0.#") + "% because the target is in a wood.</color></b>";
    }


    public override bool[] ModifyAttack(AttackController attackCtrl)
    {
        return new bool[3] { false, false, true };
    }
}
