using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Generic combat unit. Contains the unit info, stats, skills, statModifiers.
/// </summary>
public class CombatUnit
{
    // STATS

    float baseMovement, baseInitiative, basePrecision, basePhAtt, basePhDef, baseElemDef, baseMentalDef;
    int baseAttNumber, baseWoundsNumber;

    /// <summary> Energy. Points to use special actions </summary>
    public int energy;

    /// <summary> Unit movement per turn </summary>
    public float movement
    {
        set { baseMovement = value; }
        get
        {
            if ((baseMovement + GetStatModification(Stat.movement)) < 0)
                return 0;
            else
                return baseMovement + GetStatModification(Stat.movement);
        }
    }

    /// <summary> Initiative. Determines the action priority in the turn </summary>
    public float initiative
    {
        protected set { baseInitiative = value; }
        get
        {
            if ((baseInitiative + GetStatModification(Stat.initiative)) < 0)
                return 0;
            else
                return baseInitiative + GetStatModification(Stat.initiative);
        }
    }

    /// <summary> Physiscal attack </summary>
    public float phAtt
    {
        protected set { basePhAtt = value; }
        get
        {
            if ((basePhAtt + GetStatModification(Stat.phAtt)) < 0)
                return 0;
            else
                return basePhAtt + GetStatModification(Stat.phAtt);
        }
    }

    /// <summary> Phisical defense. Defends against physiscal attacks </summary>
    public float phDef
    {
        protected set { basePhDef = value; }
        get
        {
            if ((basePhDef + GetStatModification(Stat.phDef)) < 0)
                return 0;
            else
                return basePhDef + GetStatModification(Stat.phDef);
        }
    }

    /// <summary> Neutral elemental defense without applying weaknesses or resistences. 
    /// Defends against elemental and magical attacks </summary>
    public float neutralElemDef
    {
        set { baseElemDef = value; }
        get { return baseElemDef; }
    }

    /// <summary> Mental defense. Defends against phycic attack, typically acting on the unit morale </summary>
    public float mentalStrength
    {
        protected set { baseMentalDef = value; }
        get
        {
            if ((baseMentalDef + GetStatModification(Stat.mentalStrength)) < 0)
                return 0;
            else
                return baseMentalDef + GetStatModification(Stat.mentalStrength);
        }
    }

    /// <summary> Precision. Determines the hit probability of attacks </summary>
    public float precision
    {
        protected set { basePrecision = value; }
        get
        {
            if ((basePrecision + GetStatModification(Stat.precision)) < 0)
                return 0;
            if ((basePrecision + GetStatModification(Stat.precision)) > 100)
                return 100;
            else
                return basePrecision + GetStatModification(Stat.precision);
        }
    }

    /// <summary> Number of attacks. It indicates the number of attacks each soldier can bring about in a turn. </summary>
	public int attNumber
    {
        protected set { baseAttNumber = value; }
        get
        {
            if ((baseAttNumber + GetStatModification(Stat.attNumber)) < 0)
                return 0;
            else
                return baseAttNumber + (int)GetStatModification(Stat.attNumber);
        }
    }

    /// <summary> Mental defense. Defends against phycic attack, typically acting on the unit morale </summary>
	public int woundsNumber
    {
        protected set { baseWoundsNumber = value; }
        get
        {
            if ((baseWoundsNumber + GetStatModification(Stat.woundsNumber)) < 0)
                return 0;
            else
                return baseWoundsNumber + (int)GetStatModification(Stat.woundsNumber);
        }
    }


    // OTHER UNIT INFORMATION

    public string unitName;
    public string description;
    public Faction faction;
    public List<Element> elemResistence = new List<Element>();
    public List<Element> elemWeakness = new List<Element>();
    public List<Skill> skills = new List<Skill>();
    public List<StatModifier> statMods = new List<StatModifier>();
    public float heightWidthRatio;
    public float areaPerSoldier;


    // METHODS

    public float GetStatModification(Stat stat)
    {
        float modification = 0;
        foreach (StatModifier mod in statMods)
            for (int i = 0; i < mod.stats.Count; i++)
                if (mod.stats[i] == stat)
                    modification += mod.values[i];
        return modification;
    }

    /// <summary>Elemental defense of a specific element considering weaknesses and resistences</summary>
    public float elemDef(Element element)
    {
        if (elemWeakness.Contains(element))
            return neutralElemDef / 2.0f;
        else if (elemResistence.Contains(element))
            return neutralElemDef * 2;
        else
            return neutralElemDef;
    }
}


