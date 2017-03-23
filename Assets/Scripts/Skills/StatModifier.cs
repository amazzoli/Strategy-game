using System.Collections.Generic;
using UnityEngine;


public class StatModifier : IModifierSignal {

	public string title { get; set; }
	public int turnsLeft;
    public List<StatModText> mods = new List<StatModText>();
    public string symbolName { get; set; }

	public StatModifier (List<Stat> stats, List<float> values, int nTurns, string title) {
		this.title = title;
        for(int i = 0; i<stats.Count; i++)
            mods.Add(new StatModText(stats[i], values[i]));
		this.turnsLeft = nTurns;
	}


    public StatModifier(Stat stat, float value, int nTurns, string title)
    {
        this.title = title;
        mods.Add(new StatModText(stat, value));
        this.turnsLeft = nTurns;
    }


    public string description
    {
        get
        {
            string myText = "";
            foreach (StatModText modText in mods)
                myText += modText.text + ", ";
            myText = myText.Remove(myText.Length - 2);
            return myText + ". " + turnsLeft + " turns left.";
        }
    }


    public List<Stat> stats
    {
        get
        {
            List<Stat> statsList = new List<Stat>();
            foreach (StatModText modText in mods)
                statsList.Add(modText.stat);
            return statsList;
        }
    }


    public List<float> values
    {
        get
        {
            List<float> valuesList = new List<float>();
            foreach (StatModText modText in mods)
                valuesList.Add(modText.modification);
            return valuesList;
        }
    }
}
