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

    /// <summary>Movimento compiuto per avanzamento</summary>
    protected float straightMov;

    /// <summary>Diagonale dell'armata</summary>
    protected float diagonal;

    /// <summary>Angolo tra l'armata e l'area di movimento</summary>
    protected float armyAreaAngle;

    /// <summary>Overall friction during the movement. Friction = 2 means that the movement depletes two times faster</summary>
    protected float friction;

    // Testi vari usati nel description panel
    protected Text rotMovText, strMovText, movLeftText, movDone;

    protected Body armyBody;


    public InGameMovement(ArmyController armyCtrl, GameObject movAreaGO, float mov, float angleOfView, float armyAreaAngle, float friction)
    {
        movement = mov;
        armyT = armyCtrl.transform;
        this.armyAreaAngle = armyAreaAngle;
        this.friction = friction;
        movArea = new MovementArea(armyCtrl.transform, movAreaGO, 500, movement / friction, angleOfView, armyAreaAngle);
        movArea.DrawArea();
        armyCtrl.transform.FindChild("Cylinder").gameObject.SetActive(true);
        startPosition = armyT.position;
        startForward = armyT.forward;
        armyBody = armyT.GetChild(0).GetComponent<Body>();
        diagonal = Mathf.Sqrt(Mathf.Pow(armyBody.width, 2) + Mathf.Pow(armyBody.length, 2));

        Transform descPanel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Description Panel");
        descPanel.GetComponent<DescriptionPanel>().SetMovementPanel(movement);
        SetTexts(descPanel.gameObject);

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
            rotationMov = BattleF.RotationMovement(angle, diagonal);           
        }

        SetMovDoneText(rotationMov, straightMov, movement);
        armyT.rotation = Quaternion.Euler(0, armyT.rotation.eulerAngles.y, 0);
    }


    public override void StopMovement()
    {
        movArea.DestroyArea();
        armyT.GetComponent<ArmyController>().transform.FindChild("Cylinder").gameObject.SetActive(false);
        SetMovDoneText(0, 0, GetMovementLeft());
        UpdateArmyCollider(0);
        movConstructed = false;
    }


    public override float GetMovementLeft()
    {
        return movement - friction * (rotationMov + straightMov);
    }


    public override void ResetMovement()
    {
        movArea.DestroyArea();
        armyT.GetComponent<ArmyController>().transform.FindChild("Cylinder").gameObject.SetActive(false);
        armyT.position = startPosition;
        armyT.rotation = Quaternion.LookRotation(startForward);
        armyT.rotation = Quaternion.Euler(0, armyT.rotation.eulerAngles.y, 0);
        SetMovDoneText(0, 0, movement);
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


    protected virtual void SetTexts(GameObject panel)
    {
        strMovText = panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        rotMovText = panel.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        movDone = panel.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>();
        movLeftText = panel.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>();
    }


    protected void SetMovDoneText(float rotationMov, float straightMov, float movLeft)
    {
        if (movLeft <= 0) movLeft = 0;
        if (friction != 1f)
        {
            rotMovText.text = "Rotation movement done = " + friction.ToString("0.0") + "*" + rotationMov.ToString("0.0");
            strMovText.text = "Straight movement done = " + friction.ToString("0.0") + "*" + straightMov.ToString("0.0");
            movDone.text = "Total movement done = " + (friction * (rotationMov + straightMov)).ToString("0.0");
            movLeftText.text = "Movement left = " + movLeft.ToString("0.0");
        }
        else
        {
            rotMovText.text = "Rotation movement done = " + rotationMov.ToString("0.0");
            strMovText.text = "Straight movement done = " + straightMov.ToString("0.0");
            movDone.text = "Total movement done = " + (rotationMov + straightMov).ToString("0.0");
            movLeftText.text = "Movement left = " + movLeft.ToString("0.0");
        }
    }
}
