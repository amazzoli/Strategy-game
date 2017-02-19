using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;




// Controller for the deployment of the armies of a player. Called by the game controller
public class Deployment
{


    BattleData data;
    DeploymentArea playerDeploymentArea;
    GameObject armies; // ----------------------------- Empty game object which stores the armies
    SelectionController sc;
    ErrorPanel errorPanel;

    public List<DeploymentStats> playerArmyList = new List<DeploymentStats>(); // -- List of the army stat to instantiate
    public int playerIndex;
    public List<ArmyController> inGameArmies = new List<ArmyController>();



    public Deployment(BattleData data)
    {
        this.data = data;
        this.playerIndex = GameController.currentPlayer.index;
        SetArmiesEmptyObject();
        SetPlayerArmyList();
        SetPanel();
        SetPlayerDeploymentArea();
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SelectionController>();
        errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();
    }


    // Instntiate the armies, called by the deployment panel buttons
    public void addArmy(DeploymentStats armyStats)
    {
        if (!GameController.actionInProg)
        {
            if (armyStats.isInstatiated == true)
            {
                foreach (ArmyController armyCtrl in inGameArmies)
                    if (armyCtrl.name == armyStats.myName) sc.SetSelection(armyCtrl);
            }
            else
            if (MovementController.nOverlaps > 0)
            {
                errorPanel.LaunchErrorText("Remove the army overlaps");
            }
            else
            {
                armyStats.isInstatiated = true;

                Vector3 armyPosition = playerDeploymentArea.transform.position;
                Quaternion armyRotation = playerDeploymentArea.transform.rotation;
                GameObject newArmy = GameObject.Instantiate(data.armyPrefab, armyPosition, armyRotation) as GameObject;

                SetArmyStats(newArmy, armyStats);
                inGameArmies.Add(newArmy.GetComponent<ArmyController>());
                newArmy.transform.parent = armies.transform;
            }
        }
    }


    public bool CheckIfDeplIsComplete()
    {
        if (MovementController.nOverlaps > 0)
        {
            errorPanel.LaunchErrorText("Some armies are not correctly placed");
            return false;
        }
        for (int i = 0; i < playerArmyList.Count; i++)
            if (playerArmyList[i].isInstatiated == false)
            {
                errorPanel.LaunchErrorText("One or more armies are not deployed yet");
                return false;
            }
        return true;
    }


    void SetArmiesEmptyObject()
    {
        armies = GameObject.Find("Armies");
        if (armies == null) armies = new GameObject("Armies");
    }


    void SetPlayerArmyList()
    {
        foreach (DeploymentStats stats in data.armyList)
        {
            if (stats.player.index == playerIndex)
            {
                playerArmyList.Add(stats);
            }
        }
        if (playerArmyList.Count == 0) Debug.Log("No armies for player " + playerIndex);
    }


    void SetPanel()
    {
        Transform panel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Deployment Panel");
        panel.GetComponent<DeploymentPanel>().MakePanel(this, data.textures);
    }


    void SetPlayerDeploymentArea()
    {
        foreach (DeploymentArea area in data.deploymentAreas)
        {
            area.SetVisible(playerIndex);
            if (area.player.index == playerIndex)
            {
                playerDeploymentArea = area;
            }
        }
        if (playerArmyList.Count == 0) Debug.Log("No deployment area for player " + playerIndex);
    }


    void SetArmyStats(GameObject army, DeploymentStats stats)
    {
        SetArmyClass(army, stats.myClass);
        ArmyController armyCtrl = army.GetComponent<ArmyController>();
        armyCtrl.army.InitalizeSoldierList(stats.nSoldiers);
        armyCtrl.army.unitName = stats.myName;
        armyCtrl.player = stats.player;
    }


    void SetArmyClass(GameObject army, Class aClass)
    {
        if (aClass == Class.Lancers)
        {
            army.GetComponent<ArmyController>().army = new Lancers();
            army.GetComponent<ArmyController>().spriteSymbol = data.textures.lancerSprite;
            army.GetComponent<ArmyController>().textureSymbol = data.textures.lancerTexture;
        }
        if (aClass == Class.Macers)
        {
            army.GetComponent<ArmyController>().army = new Macers();
            army.GetComponent<ArmyController>().spriteSymbol = data.textures.macerSprite;
            army.GetComponent<ArmyController>().textureSymbol = data.textures.macerTexture;
        }
        if (aClass == Class.Shellfighters)
        {
            army.GetComponent<ArmyController>().army = new Shellfighters();
            army.GetComponent<ArmyController>().spriteSymbol = data.textures.shellfighterSprite;
            army.GetComponent<ArmyController>().textureSymbol = data.textures.shellfighterTexture;
        }
        if (aClass == Class.Spongezookers)
        {
            army.GetComponent<ArmyController>().army = new Spongezookers();
            army.GetComponent<ArmyController>().spriteSymbol = data.textures.spongezookerSprite;
            army.GetComponent<ArmyController>().textureSymbol = data.textures.spongezookerTexture;
        }
    }
}