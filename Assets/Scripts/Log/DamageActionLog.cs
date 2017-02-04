using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DamageActionLog: IActionLog
{
    ArmyController attacker, defender;
    EndStatModText endStatMod;
    string name;


    public DamageActionLog(ArmyController attacker, ArmyController defender, string actionName, EndStat stat, float damage)
    {
        this.attacker = attacker;
        this.defender = defender;
        name = actionName;
        endStatMod = new EndStatModText(stat, damage);
    }


    public string actionText
    {
        get
        {
            string myText = "The <b>" + name + "</b> of ";

            Color attackerColor = attacker.player.color;
            myText += "<b><color=#" + OtherF.RGBToHex(attackerColor) + ">" + attacker.army.unitName + "</color></b>";

            myText += " against ";

            Color defenderColor = defender.player.color;
            myText += " <b><color=#" + OtherF.RGBToHex(defenderColor) + ">" + defender.army.unitName + "</color></b>";

            myText += " causes " + endStatMod.text + ".";

            return myText;
        }
    }


    //public string actionName
    //{
    //    get { return name; }
    //}


    //public string actionDescription
    //{
    //    get
    //    {
    //        string myText = "";

    //        if (endStatMod.stat == EndStat.morale)
    //        {
    //            myText += "The action causes a base moral damage of <b>";
    //            myText += BattleF.moraleDmgCharge.ToString("0.00") + "</i> against the enemy, which leads to an effective damage of ";
    //            myText += endStatMod.stat;
    //        }
    //        if (endStatMod.stat == EndStat.nSoldiers)
    //        {

    //        }

           
    //        myText += BattleF.moraleDmgCharge.ToString("0.00") + "</i></b> against the enemy, which defends with ";

    //        return myText;
    //    }
    //}


    public float GetEndStatMod (EndStat stat)
    {
        if (stat == endStatMod.stat)
            return endStatMod.damage;
        else
            return 0;
    }
}
