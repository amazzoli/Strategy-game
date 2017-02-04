using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DefeatActionLog: IActionLog
{
    ArmyController subject, attacker;


    public DefeatActionLog(ArmyController subject, ArmyController attacker)
    {
        this.subject = subject;
        this.attacker = attacker;
    }


    public string actionText
    {
        get
        {
            string myText;
            Color subjectColor = subject.player.color;
            myText = "<b><color=#" + OtherF.RGBToHex(subjectColor) + ">" + subject.army.unitName + "</color></b>";

            myText += " has been defeated by ";

            Color attackerColor = attacker.player.color;
            myText += "<b><color=#" + OtherF.RGBToHex(attackerColor) + ">" + attacker.army.unitName + "</color></b>";
            return myText;
        }
    }

    //public string actionName
    //{
    //    get
    //    {
    //        return "None";
    //    }
    //}


    //public string actionDescription
    //{
    //    get
    //    {
    //        return "";
    //    }
    //}


    public float GetEndStatMod (EndStat stat)
    {
        return 0;
    }
}
