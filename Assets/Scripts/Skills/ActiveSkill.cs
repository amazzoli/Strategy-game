using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// Base class for active skills, which can be launched during the battle by the action panel
/// </summary>
public abstract class ActiveSkill
{
    // PUBLIC VARS

    /// <summary>If the skill can be casted outside the melee combat</summary>
    public bool atDistance;

    /// <summary>If the skill can be casted during the melee combat</summary>
    public bool inCombat;

    /// <summary>Skill name</summary>
    public string name;

    /// <summary>Skill description. Must contain all the essential information</summary>
    public string description;

    /// <summary>Skill caster</summary>
    public ArmyController caster;


    // PRTECTED VARS

    protected int energyCost;
    protected DescPanels descPanels;
    protected ErrorPanel errorPanel;


    // SKILL METHODS

    /// <summary>
    /// The skill starts with this method, where the caster and the panels are initialized, and there is a check if the skill can
    /// be casted. The actual action is implemented in the abstract function StartSkill()
    /// </summary>
    public void InitSkill(ArmyController caster)
    {
        this.caster = caster;
        if (descPanels == null)
            descPanels = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Description Panels").GetComponent<DescPanels>();
        if (errorPanel == null)
            errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();

        if (IsCastable())
        {
            GameController.ResetAction();
            GameController.ResetAction += ResetSkill;
            GameController.actionInProg = true;
            StartSkill();
        }
    }


    /// <summary>Abstract method for the skill implementation</summary>
    protected abstract void StartSkill();


    /// <summary>
    /// Generic conditions for the skill castability: if the unit has already casted a skill and if the energy is sufficient.
    /// Other specific conditions can be set in the virtual method OtherCastablilityConditions()
    /// </summary>
    bool IsCastable()
    {
        if (caster.skillUsed)
        {
            errorPanel.LaunchErrorText("Skill already used");
            return false;
        }
        if (caster.army.energy < energyCost)
        {
            errorPanel.LaunchErrorText("Not enough energy left");
            return false;
        }
        return OtherCastablilityConditions();
    }


    /// <summary>Specific skill castability conditions</summary>
    protected virtual bool OtherCastablilityConditions() { return true; }


    /// <summary>
    /// Reset the skill without concluding it. The method is called typically by the GameController delegate ResetAction, 
    /// launched at the beginning of other actions or to specific key inputs. It's virtual and can be extended in the childs.
    /// </summary>
    protected virtual void ResetSkill()
    {
        GameController.ResetAction -= ResetSkill;
        descPanels.HidePanel(null);
    }


    /// <summary>
    /// Skill conclusion. It's virtual and can be extended in the childs.
    /// </summary>
    protected virtual void EndSkill()
    {
        GameController.ResetAction -= ResetSkill;
        GameController.actionInProg = false;
        caster.SetEnergyConsuption(energyCost);
        caster.skillUsed = true;
    }
}