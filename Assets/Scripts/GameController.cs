using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public delegate void PlayerDlg(Player player);
public delegate void ArmyControllerDlg(ArmyController armyCtrl);
public delegate void BaseDlg();


/// <summary>
/// Game controller.
/// Controls the game progression: deployment fase for each player and battle.
/// </summary>
public class GameController : MonoBehaviour
{


    // STATIC VARIABLES

    /// <summary> The current game fase: army deployment or battle </summary>
    public static GameFase currentFase { get; private set; }

    /// <summary> The current player turn during the deployment fase </summary>
    public static Player currentPlayer { get; private set; }

    /// <summary> The current active army during the battle fase </summary>
    public static ArmyController currentArmy { get; private set; }

    /// <summary> Number of rounds since the battle has begun. A round consists in the action of each army in game </summary>
    public static int currentRound { get; private set; }

    /// <summary> If there are action in progress such as movement, charge, or skills </summary>
    public static bool actionInProg;

    /// <summary> Log of the current battle </summary>
    public static BattleLog battleLog;


    // EVENTS
    /// <summary> Occurs when the deployment turn of the player starts </summary>
    public static event PlayerDlg DeploymentTurn;

    /// <summary> Occurs when the army turn starts during the battle  </summary>
    public static event ArmyControllerDlg ArmyTurn;

    /// <summary> Beginning of the battle  </summary>
    public static event BaseDlg StartGame;

    /// <summary> Reset the action in progress </summary>
    public static BaseDlg ResetAction;


    // PUBLIC VARIABLES

    /// <summary> All the information to initialize the battle (n. armies, players, ecc..). 
    /// Must be passed through the inspector </summary>
    public BattleData data;

    /// <summary> The army list panel class. </summary>
    public ArmyListPanel armyListPanel;


    // PRIVATE VARIABLES

    /// <summary> The class for the army deployment of a player </summary>
    Deployment deployment;

    /// <summary> Index of the active army referred to the inGameArmies list </summary>
    int currentArmyIndex = 0;

    /// <summary> List of the armies in battle, sorted by initiative </summary>
    List<ArmyController> inGameArmies = new List<ArmyController>();


    // MONOBEHAVIOUR METHODS

    void Awake()
    {
        StartGame += FirstTurn;
        DeploymentTurn += SetPlayer;
        ResetAction += ResetActionInProgress;
    }

    void Start()
    {
        actionInProg = false;
        currentFase = GameFase.deployment;
        DeploymentTurn(data.players[0]);
        deployment = new Deployment(data);
        battleLog = new BattleLog();
    }

    void Update()
    {
        if (GameController.actionInProg)
        {
            if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Fire2"))
            {
                ResetAction();
            }
        }
    }


    // PUBLIC METHODS

    /// <summary> Switch to the next turn both in the battle or in the deploment fase. </summary>
    public void NextTurn()
    {
        if (!actionInProg)
        {
            if (currentFase == GameFase.deployment)
            {
                if (deployment.CheckIfDeplIsComplete())
                {
                    foreach (ArmyController armyCtrl in deployment.inGameArmies)
                        inGameArmies.Add(armyCtrl);
                    if ((deployment.playerIndex + 1) == data.players.Length)
                    { // Assuming that the player are ordered by index
                        SortArmiesByInitiative();
                        armyListPanel.MakePanel(inGameArmies, "Army order");
                        StartGame();
                    }
                    else
                    {
                        DeploymentTurn(data.players[deployment.playerIndex + 1]);
                        deployment = new Deployment(data);
                    }
                }
            }
            else if (currentFase == GameFase.battle)
            {
                if (currentArmyIndex >= (inGameArmies.Count - 1))
                {
                    currentRound += 1;
                    currentArmyIndex = 0;
                    SortArmiesByInitiative();
                    armyListPanel.MakePanel(inGameArmies, "Army order");
                }
                else
                    currentArmyIndex++;
                currentArmy.EndTurn();
                currentArmy = inGameArmies[currentArmyIndex];
                ArmyTurn(currentArmy);
                GetComponent<SelectionController>().SetSelection(currentArmy);
            }
        }
    }

    /// <summary> Remove the defeated army from the game </summary>
    /// <param name="armyCtrl"> Army controller of the defeated army </param>
    public void ArmyDefeated(ArmyController armyCtrl)
    {
        inGameArmies.Remove(armyCtrl);
        armyListPanel.MakePanel(inGameArmies, "Army order");
        armyListPanel.SetHighlighter(currentArmy);
    }


    // PRIVATE METHODS

    void FirstTurn()
    {
        if (currentFase == GameFase.deployment) currentFase = GameFase.battle;
        currentRound = 1;
        currentArmyIndex = 0;
        currentArmy = inGameArmies[currentArmyIndex];
        ArmyTurn(currentArmy);
        GetComponent<SelectionController>().SetSelection(currentArmy);
    }

    void SetPlayer(Player player)
    {
        currentPlayer = player;
    }

    void ResetActionInProgress()
    {
        actionInProg = false;
    }

    void SortArmiesByInitiative()
    {
        List<ArmyController> ranking = new List<ArmyController>();

        ranking.Add(inGameArmies[0]); // ------------------------------- Sorting by initiative
        for (int i = 1; i < inGameArmies.Count; i++)
        {
            for (int j = 0; j < ranking.Count; j++)
            {
                if (inGameArmies[i].army.initiative > ranking[j].army.initiative)
                {
                    ranking.Insert(j, inGameArmies[i]);
                    break;
                }
            }
            if (ranking.Count == i) ranking.Add(inGameArmies[i]);
        }

        List<int> indexesWithSameRank = new List<int> { 0 }; // ------------- Armies with the same initiative are shuffled
        for (int i = 1; i < ranking.Count; i++)
        {
            if (ranking[i].army.initiative == ranking[i - 1].army.initiative)
            {
                indexesWithSameRank.Add(i);
            }
            else
            {
                if (indexesWithSameRank.Count > 1) ranking = ShuffleIndexes(ranking, indexesWithSameRank);
                indexesWithSameRank.Clear();
                indexesWithSameRank.Add(i);
            }
        }
        if (indexesWithSameRank.Count > 1) ranking = ShuffleIndexes(ranking, indexesWithSameRank);

        //foreach (ArmyController armyCtrl in ranking) Debug.Log (armyCtrl.army.armyName + " " + armyCtrl.army.initiative);
        inGameArmies.Clear();
        inGameArmies = ranking;
    }

    List<ArmyController> ShuffleIndexes(List<ArmyController> list, List<int> indexesToShuffle)
    {
        for (int i = (indexesToShuffle.Count - 1); i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            ArmyController aux = list[indexesToShuffle[i]];
            list[indexesToShuffle[i]] = list[indexesToShuffle[j]];
            list[indexesToShuffle[j]] = aux;
        }
        return list;
    }
}
