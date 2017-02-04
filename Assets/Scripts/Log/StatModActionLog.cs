using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StatModActionLog: IActionLog
{
    ArmyController caster, target;
    string name;
    List<StatModText> mods = new List<StatModText>();
    int nTurns;


    public StatModActionLog(ArmyController caster, ArmyController target, StatModifier statMod)
    {
        this.caster = caster;
        this.target = target;
        for (int i = 0; i < statMod.stats.Count; i++)
            mods.Add(new StatModText(statMod.stats[i], statMod.values[i]));
        this.name = statMod.title;
        this.nTurns = statMod.turnsLeft;
    }


    public string actionText
    {
        get
        {
            string myText = "";
            Color casterColor = caster.player.color;
            myText += "<b><color=#" + OtherF.RGBToHex(casterColor) + ">" + caster.army.unitName + "</color></b>";

            myText += " <b>" + name + "</b>: ";
            foreach (StatModText modText in mods)
                myText += modText.text + ", ";
            myText = myText.Remove(myText.Length - 2);

            if (caster != target)
            {
                Color targetColor = target.player.color;
                myText += " to <b><color=#" + OtherF.RGBToHex(targetColor) + ">" + target.army.unitName + "</color></b>";
            }
            myText += " for " + nTurns.ToString() + " rounds.";
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
