using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Base class for all the skills that inflict a certain number of hits, with a certain precision and these attacks are 
/// can be defended by the target with a certain damage probability. Both for single and multi targets.
/// </summary>
public abstract class DamageSkill : Skill
{
    // PUBLIC VARS

    /// <summary>If the random generation of the hits on mark has been already performed</summary>
    public bool hitsGenerated = false;

    /// <summary>List of the targets</summary>
    public List<ArmyController> targets = new List<ArmyController>();

    /// <summary>Number of attacks against each target</summary>
    public List<int> nHits = new List<int>();

    /// <summary>List of the precisons at which each target can be hit</summary>
    public List<float> precision = new List<float>();

    /// <summary>Number of hits against each target after the random generation given the precision and the nHits</summary>
    public List<int> nHitsOnMarks = new List<int>();

    /// <summary>List of the attack strength at which each target can be hit</summary>
    public List<float> attack = new List<float>();

    /// <summary>List of the defense of each target</summary>
    public List<float> defense = new List<float>();

    /// <summary>Damage probability at which each target can be injured</summary>
    public List<float> damageProb = new List<float>();

    /// <summary>Number of wounds of each target after the random generation given the damageProb</summary>
    public List<int> nWound = new List<int>();

    /// <summary>Possible stat modification of each target</summary>
    public List<StatModifier> statMods = new List<StatModifier>();


    // METHODS FOR THE TARGET CHOICE, HITS AND WOUNDS GENERATION

    /// <summary>
    /// Skill start. If the hits on mark have been already generated make the panel for the wounds generation, otherwise
    /// it launches the abstract class targetChoice
    /// </summary>
    protected override void StartSkill()
    {
        if (hitsGenerated)
        {
            foreach (ArmyController target in targets)
                target.body.GetComponent<Renderer>().material.color = Color.red;
            descPanels.damagePanel.MakePanel(this);
        }
        else
            TargetChoice();
    }


    /// <summary> 
    /// Method which must launch UseSkill against a chosen set of targets. This can be done directly or throught, for example,
    /// a choice panel
    /// </summary>
    protected abstract void TargetChoice();


    /// <summary>
    /// Skill against a list of targets. It builds the damage panel for the hits and wounds generation.
    /// Each child must implement the skill stats (precision, nHits, ...) throught SetStats and the reset of the target choice action
    /// </summary>
    public void UseSkill(List<ArmyController> defenders)
    {
        targets.Clear();
        targets = defenders;
        if (targets.Count == 0)
        {
            errorPanel.LaunchErrorText("No enemies hit");
            ResetSkill();
            return;
        }
        ResetTargetChoice();
        foreach (ArmyController target in targets)
            target.body.GetComponent<Renderer>().material.color = Color.red;
        SetStats();
        descPanels.damagePanel.MakePanel(this);
    }


    /// <summary>Skill against a single target. It builds the damage panel for the hits generation</summary>
    public void UseSkill(ArmyController defender) { UseSkill(new List<ArmyController>() { defender }); }


    /// <summary>Implements the skill stats</summary>
    protected abstract void SetStats();


    /// <summary>Cleans the variables used in the target choice</summary>
    protected abstract void ResetTargetChoice();


    /// <summary>Method called by the damage panel for the random generation of the hits</summary>
    public void GenerateHits()
    {
        hitsGenerated = true;
        for (int i = 0; i < targets.Count; i++)
        {
            nHitsOnMarks.Add(BattleF.GetSuccessfulEvents(nHits[i], precision[i]));
            descPanels.damagePanel.UpdateTextsAfterHits(this, i);
        }
    }


    /// <summary>
    /// Method called by the damage panel for the random generation of the wounds, applying all the skill effects 
    /// and concluding the skill.
    /// </summary>
    public void GenerateWounds()
    {
        if (hitsGenerated)
        {
            EndSkill();
            for (int i = 0; i < targets.Count; i++)
            {
                if (statMods[i] != null)
                    targets[i].AddStatModifier(statMods[i], caster);
                nWound.Add(BattleF.GetSuccessfulEvents(nHitsOnMarks[i], damageProb[i]));
                targets[i].SetWoundDamage(nWound[i], caster, name);
                targets[i].body.GetComponent<Renderer>().material.color = Color.white;
                descPanels.damagePanel.UpdateTextsAfterWounds(this, i);
            }
            hitsGenerated = false;

            targets.Clear();
            precision.Clear();
            attack.Clear();
            defense.Clear();
            damageProb.Clear();
            nHits.Clear();
            nHitsOnMarks.Clear();
            nWound.Clear();
            statMods.Clear();
        }
        else
            errorPanel.LaunchErrorText("Generate the hits on mark first");
    }


    protected override void ResetSkill()
    {
        base.ResetSkill();
        foreach (ArmyController target in targets)
            target.body.GetComponent<Renderer>().material.color = Color.white;
        ResetTargetChoice();
    }


    // PROPERTIES FOR THE UIINFO TEXT IN THE DAMAGE PANEL

    /// <summary> Description of the skill hits on mark generation.</summary>
    /// <param name="index">Target index.</param>
    public virtual string HitsInfo(int index)
    {
        string text = hitsGenerationText + " <b>" + nHits[index] + "</b>\n";
        text += precisionText + " <b>" + precision[index].ToString("0.00") + "</b>\n";
        text += "Expected hits on mark: <b>" + Mathf.RoundToInt(precision[index] * nHits[index]) + "</b>";
        if (hitsGenerated)
        {
            string hexColor = OtherF.RGBToHex(descPanels.damagePanel.hitsColor);
            text += "\n<color=#" + hexColor + ">Actual hits on mark: <b>" + nHitsOnMarks[index] + "</b></color>";
        }
        return text;
    }


    /// <summary> Description of the skill wound generation.</summary>
    /// <param name="index">Target index.</param>
    public virtual string WoundsInfo(int index)
    {
        string text;
        string hexHitsColor = OtherF.RGBToHex(descPanels.damagePanel.hitsColor);
        if (hitsGenerated)
            text = "<color=#" + hexHitsColor + ">N hits on mark: <b>" + nHitsOnMarks[index] + "</b></color>\n";
        else
            text = "<color=#" + hexHitsColor + ">N hits on mark: <i>not generated yet</i></color>\n";
        text += attackStrengthText + " <b>" + attack[index] + "</b>\n";
        text += defenseText + " <b>" + defense[index] + "</b>\n";
        text += "Damage probability: <b>" + damageProb[0].ToString("0.00") + "</b>";
        if (hitsGenerated || caster.skillUsed)
            text += "\nExpected number of wounds: <b>" + Mathf.RoundToInt(nHitsOnMarks[index] * damageProb[index]) + "</b>";
        if (caster.skillUsed)
        {
            string hexColor = OtherF.RGBToHex(descPanels.damagePanel.woundColor);
            text += "\n<color=#" + hexColor + ">Actual number of wounds: <b>" + nWound[index] + "</b></color>";
        }
        return text;
    }

    protected abstract string hitsGenerationText { get; }

    protected abstract string precisionText { get; }

    protected abstract string attackStrengthText { get; }

    protected abstract string defenseText { get; }
}