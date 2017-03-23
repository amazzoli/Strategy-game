using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary> Controls the movement during the battle </summary>
public class InGameMovement : Movement
{

    /// <summary>Forward dell'armata successivo ad una eventuale rotazione gratuita prima del movimento (usato in escape)</summary>
	//protected Vector3 initForward;

    /// <summary>Mesh area di movimento gestita dall'apposita classe</summary>
	protected MovementArea movArea;

    /// <summary>Movimento disponibile</summary>
    protected float movement;

    /// <summary>Movimento compiuto per rotazione</summary>
    protected float rotationMov;

    /// <summary>Movimento compiuto per avanzamento (no terrain penalities)</summary>
    protected float straightMov;

    /// <summary>Movimento compiuto per avanzamento in bosco</summary>
    protected float movInWood;

    /// <summary>Diagonale dell'armata</summary>
    protected float diagonal;

    /// <summary>Angolo tra l'armata e l'area di movimento</summary>
    protected float armyAreaAngle;

    /// <summary>Overall friction during the movement. Friction = 2 means that the movement depletes two times faster</summary>
    protected float friction;

    MovPanel movPanel;

    protected Body armyBody;


    public InGameMovement(ArmyController armyCtrl, GameObject movAreaGO, float mov, float angleOfView, float armyAreaAngle, float friction)
    {
        movement = mov;
        armyT = armyCtrl.transform;
        this.armyAreaAngle = armyAreaAngle;
        this.friction = friction;
        movArea = new MovementArea(armyCtrl.transform, movAreaGO, movement / friction, angleOfView, armyAreaAngle);
        armyCtrl.transform.FindChild("Cylinder").gameObject.SetActive(true);
        startPosition = armyT.position;
        startForward = armyT.forward;
        armyBody = armyT.GetChild(0).GetComponent<Body>();
        diagonal = Mathf.Sqrt(Mathf.Pow(armyBody.width, 2) + Mathf.Pow(armyBody.length, 2));

        movPanel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Description Panels").GetChild(0).GetComponent<MovPanel>();
        movPanel.MakePanel(movement);

        movConstructed = true;
    }


    public override bool IsAllowed()
    {
        if (GameController.currentArmy == armyT.GetComponent<ArmyController>() &&
            GameController.currentFase == GameFase.battle)
            return true;
        return false;
    }


    public override void InitMovement() { }


    public override void UpdateMovement()
    {
        Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector3 mousePosition = GeomF.ToYPlane(mouseScreenToWorld, Camera.main.transform.position, 0);

        float angle = (GeomF.YAngleWithSign(startForward, mousePosition - startPosition) + 360 - armyAreaAngle) % 360; // Angle between mouse and mov area
        straightMov = Vector3.Distance(startPosition, mousePosition);
        movInWood = movArea.MovementDoneInWood(angle, straightMov);
        rotationMov = BattleF.RotationMovement(angle, diagonal);

        if (straightMov <= movArea.GetMovAreaRadius(angle)) // Mouse inside the movement area
        {
            UpdateArmyCollider(straightMov);
            armyT.position = mousePosition;
            if (mousePosition != startPosition)
                armyT.rotation = Quaternion.LookRotation(mousePosition - startPosition);
        }
        else // Mouse outside the movement area
        {
            if (angle > movArea.GetLimitAngles()[0] && angle <= 180f) // Max rotation reached (right side)
                angle = movArea.GetLimitAngles()[0];

            else if (angle < movArea.GetLimitAngles()[1] && angle > 180f) // Max rotation reached (left side)
                angle = movArea.GetLimitAngles()[1];

            UpdateArmyCollider(movArea.GetMovAreaRadius(angle));
            Quaternion lookAtAngle = Quaternion.LookRotation(GeomF.DirFromAngleY(angle + armyAreaAngle));
            armyT.rotation = lookAtAngle * Quaternion.LookRotation(startForward);
            armyT.position = movArea.GetMovAreaBoundary(angle);
            straightMov = movArea.GetMovAreaRadius(angle);
            movInWood = movArea.MovementDoneInWood(angle, straightMov);
            //rotationMov = BattleF.RotationMovement(angle, diagonal);           
        }

        movPanel.SetTexts(movement, rotationMov, straightMov, movInWood, friction);
        armyT.rotation = Quaternion.Euler(0, armyT.rotation.eulerAngles.y, 0);
    }


    public override void StopMovement()
    {
        movArea.DestroyArea();
        armyT.GetComponent<ArmyController>().transform.FindChild("Cylinder").gameObject.SetActive(false);
        movPanel.MakePanel(GetMovementLeft());
        UpdateArmyCollider(0);
        movConstructed = false;
    }


    public override float GetMovementLeft()
    {
        float movLeft = movement - friction * (rotationMov + straightMov + movInWood);
        if (movLeft < 0)
            movLeft = 0;
        return movLeft;
    }


    public override void ResetMovement()
    {
        movArea.DestroyArea();
        armyT.GetComponent<ArmyController>().transform.FindChild("Cylinder").gameObject.SetActive(false);
        armyT.position = startPosition;
        armyT.rotation = Quaternion.LookRotation(startForward);
        armyT.rotation = Quaternion.Euler(0, armyT.rotation.eulerAngles.y, 0);
        movPanel.MakePanel(movement);
        UpdateArmyCollider(0);

    }


    protected virtual void UpdateArmyCollider(float linearMovement)
    {
        BoxCollider armyCollider = armyBody.GetComponent<BoxCollider>();
        if (linearMovement > armyBody.length)
        {
            float dilatation = linearMovement / armyBody.length;
            armyCollider.size = new Vector3(1, dilatation, 1);
            armyCollider.center = new Vector3(0, -(dilatation - 1) / 2.0f, 0);
        }
        else
        {
            armyCollider.size = new Vector3(1, 1, 1);
            armyCollider.center = Vector3.zero;
        }
    }
}
