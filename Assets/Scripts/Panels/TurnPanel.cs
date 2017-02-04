using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TurnPanel : MonoBehaviour {

	public Text turnText;
	public Button turnButton;
	public GameController gc;


	void Awake () {
		GameController.DeploymentTurn += SetDeplFaseText;
		GameController.ArmyTurn += SetTurnText;
		//GameController.ActionInProgress += HidePanel;
	}


	void SetDeplFaseText(Player player)
    {
		turnText.text = player.playerName + "'s deployment fase";
    }


	void SetTurnText(ArmyController armyCtrl)
	{
		turnText.text = armyCtrl.army.unitName + "'s turn";
	}

	void HidePanel(Action action){
		if (action == Action.nothing) gameObject.SetActive (true);
		else gameObject.SetActive (false);
	}	
}
