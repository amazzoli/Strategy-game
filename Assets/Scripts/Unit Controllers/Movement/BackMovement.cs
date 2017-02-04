using UnityEngine;
using System.Collections;


public class BackMovement : InGameMovement
{

    Quaternion rotationAfterMov;


    public BackMovement(ArmyController armyCtrl, GameObject movAreaGO, float mov) :
    base(armyCtrl, movAreaGO, mov, 360, 180, 2)
    {
        rotationAfterMov = Quaternion.AngleAxis(180, Vector3.up);
    }


    public override void UpdateMovement()
    {
        base.UpdateMovement();
        armyT.rotation *= rotationAfterMov;
    }


    protected override void UpdateArmyCollider(float linearMovement)
    {
        BoxCollider armyCollider = armyBody.GetComponent<BoxCollider>();
        if (linearMovement > armyBody.length)
        {
            float dilatation = linearMovement / armyBody.length;
            armyCollider.size = new Vector3(1, dilatation, 1);
            armyCollider.center = new Vector3(0, (dilatation - 1) / 2.0f, 0);
        }
        else
        {
            armyCollider.size = new Vector3(1, 1, 1);
            armyCollider.center = Vector3.zero;
        }
    }
}
