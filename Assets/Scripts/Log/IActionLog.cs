using UnityEngine;
using System.Collections;


/// <summary>
/// Interface for the battle action which will be stored and processed in the battle log
/// </summary>
public interface IActionLog
{
    /// <summary>
    /// Action text of the log panel
    /// </summary>
    string actionText { get; }

    ///// <summary>
    ///// Name of the action, shown in the info panel
    ///// </summary>
    //string actionName { get; }

    ///// <summary>
    ///// Action description, shown in the info panel
    ///// </summary>
    //string actionDescription { get; }

    /// <summary>
    /// Get the total modification caused by the action at the given end stat
    /// </summary>
    /// <param name="stat">Modified stat</param>
    /// <returns>Modfication value</returns>
    float GetEndStatMod(EndStat stat);
}


/// <summary>
/// Modification of an EndStat (morale, nSoldiers, energy)
/// </summary>
public struct EndStatModText
{
    public EndStat stat { get; private set; }
    public float damage { get; private set; }

    public EndStatModText(EndStat stat, float damage)
    {
        this.stat = stat;
        this.damage = damage;
    }

    public string text
    {
        get
        {
            if (stat == EndStat.morale)
                return "<color=blue>" + (damage * 100).ToString("0") + "% <b><i>morale</i></b></color> damage";
            else if (stat == EndStat.nSoldiers)
                return "the loss of <color=red>" + damage.ToString("0") + "</color> <color=red><b><i>soldiers</i></b></color>";
            else
                return "- <color=orange>" + damage.ToString("0") + "<b><i>energy</i></b></color> points";
        }
    }
}


/// <summary>
/// Modification of a Stat
/// </summary>
public struct StatModText
{
    public Stat stat { get; private set; }
    public float modification { get; private set; }

    public StatModText(Stat stat, float modification)
    {
        this.stat = stat;
        this.modification = modification;
    }

    public string text
    {
        get
        {
            string myText;
            if (modification >= 0)
                myText = "+ ";
            else
                myText = "- ";
            if (stat == Stat.attNumber || stat == Stat.woundsNumber)
                myText += "<b>" + Mathf.Abs(modification) + " <i>" + StatToString(stat) + "</i></b>";
            else if (stat == Stat.movement)
                myText += "<b>" + Mathf.Abs(modification).ToString("0.#") + " <i>" + StatToString(stat) + "</i></b>";
            else if (stat == Stat.precision)
                myText += "<b>" + Mathf.Abs(modification).ToString("0.#") + "% <i>" + StatToString(stat) + "</i></b>";
            else
                myText += "<b>" + Mathf.Abs(modification).ToString("0.##") + " <i>" + StatToString(stat) + "</i></b>";
            return myText;
        }
    }


    string StatToString(Stat stat)
    {
        if (stat == Stat.attNumber)
            return "Number of attacks";
        else if (stat == Stat.elemDef)
            return "Elemental defense";
        else if (stat == Stat.initiative)
            return "Initiative";
        else if (stat == Stat.mentalStrength)
            return "Mental strength";
        else if (stat == Stat.movement)
            return "Movement";
        else if (stat == Stat.phAtt)
            return "Physical attack";
        else if (stat == Stat.phDef)
            return "Physical defense";
        else if (stat == Stat.precision)
            return "Precision";
        else
            return "Number of wounds";
    }
}