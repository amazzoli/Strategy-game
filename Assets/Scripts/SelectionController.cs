using UnityEngine;
using System.Collections;



/// <summary>
/// Manages the selection of the armies.
/// There are two kind of selection: the normal army selection, and the selection during specific actions
/// (action selection) which typically consists in choosing the action target.
/// </summary>
public class SelectionController : MonoBehaviour {

	ArmyController currentSelected;

    public static event ArmyControllerDlg ArmyDeselection, ArmySelection;


    void Awake() {
		GameController.DeploymentTurn += UnsetSelectionByTurn;
		GameController.ArmyTurn += SetSelection;
        ArmyDeselection += UnsetSelection;
        currentSelected = null;
    }


    void Update() {
		if (!GameController.actionInProg) {
			if (Input.GetButtonDown ("Cancel") || Input.GetButtonDown ("Fire2")) {
				ArmyDeselection (currentSelected);
			}
		}
    }


	public void SetSelection(ArmyController armyCtrl) { 
		if (!GameController.actionInProg) {
			ArmyDeselection (currentSelected);
			ArmySelection (armyCtrl);
			armyCtrl.transform.FindChild ("Selection").GetComponent<SelectionResizer> ().ShowSelectionDraw ();
			currentSelected = armyCtrl;
		}
    }


	void UnsetSelection(ArmyController armyCtrl) {
		if (currentSelected == armyCtrl && armyCtrl != null) {
			armyCtrl.transform.FindChild ("Selection").GetComponent<SelectionResizer> ().HideSelectionDraw ();
            currentSelected = null;
        }
    }
		

	void UnsetSelectionByTurn(Player p) {
		ArmyDeselection (currentSelected);
	}

	void DestroyMovArea() {
		GameObject area = GameObject.FindGameObjectWithTag ("Movement Area");
		if (area!= null) Destroy (area);
	}
}
