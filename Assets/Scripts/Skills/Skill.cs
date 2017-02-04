using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Skill
{

    public bool atDistance, inCombat;
    public string name, description;

    protected int energyCost;
    protected ArmyController caster;
    protected DescriptionPanel descPanel;
    protected ErrorPanel errorPanel;


    public virtual void InitSkill(ArmyController caster)
    {
        this.caster = caster;
        if (descPanel == null)
            descPanel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Description Panel").GetComponent<DescriptionPanel>();
        if (errorPanel == null)
            errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();
        if (caster.skillUsed)
        {
            errorPanel.LaunchErrorText("Skill already used");
            return;
        }
        if (caster.army.energy < energyCost)
        {
            errorPanel.LaunchErrorText("Not enough energy left");
            return;
        }
        GameController.ResetAction();
        GameController.ResetAction += ResetSkill;
        GameController.actionInProg = true;
    }

    protected virtual void ResetSkill()
    {
        GameController.ResetAction -= ResetSkill;
        descPanel.HidePanel(null);
    }

    protected virtual void EndSkill()
    {
        GameController.ResetAction -= ResetSkill;
        GameController.actionInProg = false;
        caster.SetEnergyConsuption(energyCost);
    }
}


public abstract class OneTargetSkill : Skill
{
    protected ArmyController target;


    public abstract void UseSkill(ArmyController target);
}


public abstract class AOESkill : Skill
{
    protected List<ArmyController> targets;
    protected AOEArea area;
    protected float extAreaRadius = 5, internalAreaRadius = 2.5f;
    Coroutine areaMovement;


    public override void InitSkill(ArmyController caster)
    {
        base.InitSkill(caster);
        AOEArea areaModel = GameObject.FindGameObjectWithTag("GameController").GetComponent<Resources>().area;
        area = GameObject.Instantiate(areaModel, new Vector3(0.0f, 0.03f, 0.0f), Quaternion.identity);
        area.externalRadius = extAreaRadius;
        area.internalRadiusProportion = internalAreaRadius / extAreaRadius;
        area.StartMyMovement();
    }


    public abstract void UseSkill(List<ArmyController> targets);


    protected override void ResetSkill()
    {
        base.ResetSkill();
        area.StartMyMovement();
        GameObject.Destroy(area.gameObject);
    }


    protected override void EndSkill()
    {
        base.ResetSkill();
        area.StartMyMovement();
        GameObject.Destroy(area.gameObject);
    }

}