using UnityEngine;
using System.Collections.Generic;


public abstract class OneTargetDamageSkill: OneTargetSkill
{
    public bool hitsGenerated = false;
    public float precision, attack, defense, damageProb;
    public int nHits, nHitsOnMarks, nWound;


    public override void InitSkill(ArmyController caster)
    {
        base.InitSkill(caster);
        if (hitsGenerated)
        {
            target.body.GetComponent<Renderer>().material.color = Color.red;
            descPanel.SetDamagePanel(this, true, false, target.army.unitName);
        }
        else
            TargetChoice();
    }


    public override void UseSkill(ArmyController defender)
    {
        target = defender;
        ResetTargetChoice();
        target.body.GetComponent<Renderer>().material.color = Color.red;
        SetStats();
        descPanel.SetDamagePanel(this, false, false, target.army.unitName);
        ModifyDescPanel();
    }


    public void GenerateHits()
    {
        nHitsOnMarks = BattleF.GetSuccessfulEvents(nHits, precision);
        descPanel.SetDamagePanel(this, true, false, target.army.unitName);
        hitsGenerated = true;
    }


    public void GenerateWounds()
    {
        nWound = BattleF.GetSuccessfulEvents(nHitsOnMarks, damageProb);
        target.SetWoundDamage(nWound, caster, name);
        caster.skillUsed = true;
        hitsGenerated = false;
        target.body.GetComponent<Renderer>().material.color = Color.white;
        EndSkill();
    }


    protected override void ResetSkill()
    {
        base.ResetSkill();
        if (target != null)
            target.body.GetComponent<Renderer>().material.color = Color.white;
        ResetTargetChoice();
    }


    /// <summary>
    /// Function which must launch UseSkill(ArmyController)
    /// </summary>
    protected abstract void TargetChoice();

    /// <summary>
    /// Must implement the skill stats
    /// </summary>
    protected abstract void SetStats();

    protected virtual void ModifyDescPanel() { }

    /// <summary>
    /// Cleans the variables needed for the target choice
    /// </summary>
    protected abstract void ResetTargetChoice();
}