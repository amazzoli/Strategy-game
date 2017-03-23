﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Controlls the army movement. Must be attached to the army body.
/// It manages both the in game movement and the deployment movement
/// </summary>
public class MovementController : MonoBehaviour
{

    public float movementLeft;
    public GameObject movementArea;
    public bool canCharge;
    public bool movActive = false, movCondstructed = false;
    /// <summary> Number of army overlaps in the game </summary>
    public static int nOverlaps = 0;

    Movement movement;
    ArmyController armyCtrl;
    ErrorPanel errorPanel;
    ActionPanel actionPanel;
    DescPanels descPanels;
    int nOverlapsOnMe = 0;
    List<string> allowedColliderTags = new List<string>() { "Aux Collider", "AOEArea", "Wood", "Inner Collider" };


    void Awake()
    {
        GameController.ArmyTurn += ResetMovVar;
        GameController.ResetAction += ResetMovement;
        GameController.StartGame += ResetVar;
    }


    void Start()
    {
        armyCtrl = transform.parent.GetComponent<ArmyController>();
        errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();
        actionPanel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Action Panel").GetComponent<ActionPanel>();
        descPanels = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Description Panels").GetComponent<DescPanels>();

        movement = new DeploymentMovement(transform.parent.transform);
    }


    public void MovementLeftReduction(float reductionValue)
    {
        if (GameController.currentFase == GameFase.battle)
        {
            if (movementLeft < reductionValue)
                reductionValue = movementLeft;
            movementLeft -= reductionValue;
            armyCtrl.textSpawner.SpawnMovementReduction(reductionValue, armyCtrl);
        }
    }

    // Called by the action panel button
    public void StartInGameMovement(bool back)
    {
        if (movementLeft > 0.5f)
        {
            GameController.ResetAction();
            GameController.actionInProg = true;
            if (back)
                movement = new BackMovement(armyCtrl, movementArea, movementLeft);
            else
                movement = new ForwardMovement(armyCtrl, movementArea, movementLeft);
        }
        else
            errorPanel.LaunchErrorText("Not enough movement left");
    }


    public void ChooseEscapeDirection()
    {
        if (armyCtrl.enemiesEngaged.list.Count >= 4)
        {
            errorPanel.LaunchErrorText("Escape not possible");
        }
        else
        {
            GameController.ResetAction();
            GameController.actionInProg = true;
            List<Directions> freeSides = armyCtrl.enemiesEngaged.freeSides;
            if (freeSides.Count == 1)
                StartEscape(freeSides[0]);
            else
                descPanels.choicePanel.MakePanel(freeSides, StartEscape, "Choose the escape direction:");
        }
    }


    public void StartEscape(Directions dir)
    {
        movement = new EscapeMovement(armyCtrl, movementArea, movementLeft, dir);
    }


    void OnMouseDown()
    {
        if (!movActive && movement.movConstructed)
        {
            if (movement.IsAllowed())
            {
                movActive = true;
                movement.InitMovement();
                StartCoroutine(MovementCoroutine());
            }
        }
    }


    IEnumerator MovementCoroutine()
    {
        float startTime = Time.time;
        while (true)
        {
            movement.UpdateMovement();
            if ((Time.time - startTime) > 0.1)
                StopMovement();
            yield return null;
        }
    }


    void StopMovement()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (nOverlaps > 0)
            {
                errorPanel.LaunchErrorText("Not allowed position");
                return;
            }
            errorPanel.ClosePanel();
            movActive = false;
            GameController.actionInProg = false;
            StopAllCoroutines();
            MovementLeftReduction(movementLeft - movement.GetMovementLeft());
            movement.StopMovement();
            canCharge = false;
            actionPanel.MakePanel(armyCtrl);
            //Camera.main.GetComponent<CameraMover>().ArmyAlignment(armyCtrl);
        }
    }


    void ResetMovement()
    {
        if (movement.movConstructed)
        {
            movement.movConstructed = false;
            StopAllCoroutines();
            movement.ResetMovement();
            movActive = false;
        }
        descPanels.HidePanel(null);
    }


    void OnTriggerEnter(Collider other)
    {
        if (!allowedColliderTags.Contains(other.tag))
        {
            nOverlaps++;
            nOverlapsOnMe++;
            GetComponent<Body>().MarkAsOverlapped();
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (!allowedColliderTags.Contains(other.tag))
        {
            nOverlaps--;
            nOverlapsOnMe--;
            if (nOverlapsOnMe == 0)
                GetComponent<Body>().UnmarkAsOverlapped();
        }
    }


    void ResetMovVar(ArmyController currentArmy)
    {
        if (currentArmy.army.unitName == armyCtrl.army.unitName)
        {
            movementLeft = transform.parent.GetComponent<ArmyController>().army.movement;
            canCharge = true;
        }
    }


    void ResetVar()
    {
        movement.movConstructed = false;
    }
}
