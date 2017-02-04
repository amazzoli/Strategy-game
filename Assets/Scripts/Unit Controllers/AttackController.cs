using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the attack action: detects the armies in sight, checks if they can be
/// approached, and moves the army in conctact with the enemy.
/// </summary>
public class AttackController : MonoBehaviour
{

    public GameObject fieldOfViewGO;
    public float contactOffset; // Distance between the attecker and the defender when engaged

    ErrorPanel errorPanel;
    ActionPanel actionPanel;
    DescriptionPanel descriptionPanel;
    FieldOfView fieldOfView; // Class which casts and controls the army field of view
    AuxCollider auxCollider, enemyCollider; // Auxiliary colliders for the detection of obstacles during the approach
    ArmyController armyCtrl, defender;
    List<ArmyController> enemiesInSight = new List<ArmyController> ();
    Directions side; // Attacked side of the defender
    Vector3 piontToAttack;
    Coroutine attackCoroutine;
    bool initialized = false, attacking = false, charging;


    void Awake()
    {
        GameController.ResetAction += DeleteFieldOfView;
    }

    void Start()
    {
        armyCtrl = GetComponent<ArmyController>();
        auxCollider = transform.GetChild(1).GetComponent<AuxCollider>();
        errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();
        descriptionPanel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Description Panel").GetComponent<DescriptionPanel>();
    }


    /// <summary> Starts the attack procedure. </summary>
    /// <param name="canCharge">If set to <c>true</c> the army can charge.</param>
    public void StartAttack(bool canCharge)
    {
        if (armyCtrl.body.GetComponent<MovementController>().movementLeft > (armyCtrl.body.length / 2.0f))
        {
            GameController.ResetAction();
            GameController.actionInProg = true;
            CastFieldOfView();
            charging = canCharge;
            initialized = true;
        }
        else
        {
            errorPanel.LaunchErrorText("Not enough movement left");
        }
    }

    /// <summary> Called by army controller if all the conditions for the attack are satisfied
    /// Draws the fied of view and detects the armies in sight. Those aremies are marked as attackable (Body method)
    /// </summary>
    void CastFieldOfView()
    {
        float movement = armyCtrl.transform.GetChild(0).GetComponent<MovementController>().movementLeft;
        fieldOfView = new FieldOfView(armyCtrl.transform, fieldOfViewGO, 1000, movement + armyCtrl.body.length / 2.0f, true);
        fieldOfView.DrawArea();
        attackCoroutine = StartCoroutine(AttackCoroutine());

        actionPanel = GameObject.FindGameObjectWithTag("Action Panel").GetComponent<ActionPanel>();

        enemiesInSight.Clear();
        foreach (InSightArmy army in fieldOfView.armiesInSight)
            if (!army.isAllied)
            {
                army.armyCtrl.body.MarkAsAttackable(armyCtrl, Attack, true);
                enemiesInSight.Add(army.armyCtrl);
            }
        descriptionPanel.SetArmyChioce(enemiesInSight, Attack);
    }

    /// <summary> 
    /// Start the attack of a chosen enemy in sight.
    /// The enemies marked as attackble can be clicked in the body class. The click calls this metohd.
    /// Sets the defender and the attack side, check if the enemy at that side is not engaged, set the
    /// point to approach, casts the aux colliders alogn the path between the enemy and the unit.
    /// In order to work, the aux colliders needs at least that one frame lasts, attacking true makes that possible.
    /// After this time the CheckAndAtack method is called.
    /// </summary>
    /// <param name="index"> Index of the defender in the armiesInSight list </param>
    public void Attack(ArmyController defender)
    {
        if (!enemiesInSight.Contains(defender))
            return;

        bool defenderFound = false;
        for (int i = 0; i < fieldOfView.armiesInSight.Count; i++)
            if (fieldOfView.armiesInSight[i].armyCtrl == defender)
            {
                side = fieldOfView.armiesInSight[i].armySide;
                this.defender = defender;
                defenderFound = true;
            }
        if (!defenderFound)
            return;

        if (defender.enemiesEngaged.GetEnemy(side) != null)
        {
            errorPanel.LaunchErrorText("Unit already engaged");
            return;
        }
        SetPointToAttack();
        CastMyAuxCollider();
        CastEnemyAuxCollider();
        attacking = true;
    }


    IEnumerator AttackCoroutine()
    {
        while (true)
        {
            if (attacking)
            {
                yield return new WaitForSeconds(2 * Time.deltaTime);
                CheckAndAttack();
            }
            yield return null;
        }
    }


    // Check if there are obstacles in the path to the enemy.
    // If not the attack is carried out
    void CheckAndAttack()
    {
        List<GameObject> obstacles = new List<GameObject>();
        List<GameObject> allowedObstacles = new List<GameObject>() {
            armyCtrl.body.gameObject,
            defender.body.gameObject
        };
        foreach (ArmyController engagedEnemy in defender.enemiesEngaged.list)
            allowedObstacles.Add(engagedEnemy.body.gameObject);

        foreach (Collider collider in auxCollider.collidersInsideMe)
            if (!allowedObstacles.Contains(collider.gameObject))
                obstacles.Add(collider.gameObject);
        foreach (Collider collider in enemyCollider.collidersInsideMe)
            if (!allowedObstacles.Contains(collider.gameObject) && !obstacles.Contains(collider.gameObject))
                obstacles.Add(collider.gameObject);

        if (obstacles.Count > 0)
        {
            errorPanel.LaunchErrorText("There are obstacles on the path");
            foreach (GameObject go in obstacles)
                Debug.Log(go);
        }
        else
            ConcludeAttack();

        attacking = false;
        ResetColliders();
    }


    void ConcludeAttack()
    {
        ApproachTheEnemy();
        DeleteFieldOfView();
        armyCtrl.Engage(defender, side);

        if (side == Directions.east || side == Directions.west)
            defender.SetMoraleDamage(BattleF.moraleDmgLateralAttack, armyCtrl, "Lateral attack");

        if (side == Directions.south)
            defender.SetMoraleDamage(BattleF.moraleDmgBackAttack, armyCtrl, "Back attack");

        if (charging)
        {
            armyCtrl.AddStatModifier(Stat.phAtt, 1, 1, "Charge", armyCtrl);
            defender.SetMoraleDamage(BattleF.moraleDmgCharge, armyCtrl, "Charge");
        }
        //GameController.battleLog.Add(new ChargeActionLog(armyCtrl, charging, defender, side, chargeDamage, sideDamage));
        actionPanel.MakePanel(armyCtrl);
    }


    void DeleteFieldOfView()
    {
        if (initialized)
        {
            initialized = false;
            GameController.actionInProg = false;
            StopCoroutine(attackCoroutine);
            foreach (InSightArmy seenArmy in fieldOfView.armiesInSight)
            {
                seenArmy.armyCtrl.body.UnmarkAsAttackable();
            }
            fieldOfView.DestroyArea();
            descriptionPanel.HidePanel(null);
        }
    }


    void SetPointToAttack()
    {
        if (side == Directions.north)
        {
            if (armyCtrl.body.width >= defender.body.width)
                piontToAttack = defender.transform.position + defender.transform.forward * defender.body.GetEffectiveLength() / 2.0f;
            else
                piontToAttack = defender.transform.position + defender.transform.forward * defender.body.length / 2.0f;
        }
        else if (side == Directions.east)
        {
            if (armyCtrl.body.width >= defender.body.length)
                piontToAttack = defender.transform.position + defender.transform.right * defender.body.GetEffectiveWidth() / 2.0f;
            else
                piontToAttack = defender.transform.position + defender.transform.right * defender.body.width / 2.0f;
        }
        else if (side == Directions.south)
        {
            if (armyCtrl.body.width >= defender.body.width)
                piontToAttack = defender.transform.position - defender.transform.forward * defender.body.GetEffectiveLength() / 2.0f;
            else
                piontToAttack = defender.transform.position - defender.transform.forward * defender.body.length / 2.0f;
        }
        else if (side == Directions.west)
        {
            if (armyCtrl.body.width >= defender.body.length)
                piontToAttack = defender.transform.position - defender.transform.right * defender.body.GetEffectiveWidth() / 2.0f;
            else
                piontToAttack = defender.transform.position - defender.transform.right * defender.body.width / 2.0f;
        }
    }


    void CastMyAuxCollider()
    {
        auxCollider.gameObject.SetActive(true);
        auxCollider.ResetPosition();
        float height = armyCtrl.body.length;
        auxCollider.Elongate(Vector3.Distance(transform.position, piontToAttack) - height / 2.0f, piontToAttack - transform.position);
    }


    void CastEnemyAuxCollider()
    {
        enemyCollider = defender.transform.GetChild(1).GetComponent<AuxCollider>();
        enemyCollider.gameObject.SetActive(true);
        enemyCollider.ResetPosition();
        enemyCollider.PutOnArmySide(armyCtrl.body.width + contactOffset, armyCtrl.body.length, 0, side);
    }


    void ResetColliders()
    {
        auxCollider.gameObject.SetActive(false);
        auxCollider.ResetPosition();
        enemyCollider.gameObject.SetActive(false);
        enemyCollider.ResetPosition();
    }


    void ApproachTheEnemy()
    {
        Vector3 startForward = transform.forward, startPosition = transform.position;
        if (side == Directions.north)
            transform.rotation = Quaternion.LookRotation(-defender.transform.forward);
        if (side == Directions.east)
            transform.rotation = Quaternion.LookRotation(-defender.transform.right);
        if (side == Directions.south)
            transform.rotation = Quaternion.LookRotation(defender.transform.forward);
        if (side == Directions.west)
            transform.rotation = Quaternion.LookRotation(defender.transform.right);
        transform.position = piontToAttack - transform.forward * (GetComponent<ArmyController>().body.length + contactOffset) / 2.0f;

        float coveredStraightDistance = Vector3.Distance(startPosition, transform.position);
        float diagonal = Mathf.Sqrt(Mathf.Pow(armyCtrl.body.width, 2) + Mathf.Pow(armyCtrl.body.length, 2));
        float coveredRotationDistance = BattleF.RotationMovement(GeomF.YAngleWithSign(startForward, piontToAttack - startPosition), diagonal);
        armyCtrl.body.gameObject.GetComponent<MovementController>().MovementLeftReduction(coveredStraightDistance + coveredRotationDistance);
    }
}


