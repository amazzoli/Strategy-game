using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// Base class for passive skills
/// </summary>
public abstract class PassiveSkill: IModifierSignal
{
    // PUBLIC VARS

    /// <summary>Skill name</summary>
    public string title { get; protected set; }

    /// <summary>Skill description. Must contain all the essential information</summary>
    public string description { get; protected set; }

    /// <summary>Skill caster</summary>
    public ArmyController caster;

    public string symbolName { get; set; }


    // SKILL METHODS

    public virtual void DamageSkillStatModifier(DamageSkill skill, int targetIndex) { }

    public virtual string DamageSkillStatModText(DamageSkill skill, int targetIndex) { return ""; }

    public virtual bool[] ModifyAttack(AttackController attackCtrl) { return new bool[3] { false, false, false }; }
}


public class PSkillProva: PassiveSkill
{
    public PSkillProva()
    {
        title = "wela";
        description = "culo";
    }
}