using UnityEngine;
using System.Collections;

public class ArmyBars : MonoBehaviour {

	public GameObject barsGO;
	bool activeCoroutine;
	GameObject bars;
	RectTransform size, morale, energy;
	Coroutine followArmy;


	void Awake () {
		SelectionController.ArmySelection += SetBars;
		SelectionController.ArmyDeselection += UnsetBars;
	}


	void Start () {
		activeCoroutine = false;
		bars = Instantiate (barsGO, Vector3.zero, Quaternion.identity) as GameObject;
		bars.transform.SetParent (this.transform);
		morale = bars.transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ();
		size = bars.transform.GetChild (1).GetChild (0).GetComponent<RectTransform> ();
		energy = bars.transform.GetChild (2).GetChild (0).GetComponent<RectTransform> ();
		bars.SetActive (false);
	}


	public void SetBars (ArmyController armyCtrl) {
		if (GameController.currentFase == GameFase.battle) {
			bars.SetActive (true);
			activeCoroutine = true;
			morale.localScale = new Vector3 (1, armyCtrl.morale, 1);
			energy.localScale = new Vector3 (1, (float)armyCtrl.army.energy / (float)armyCtrl.startEnergy, 1);
			size.localScale = new Vector3 (1, (float)armyCtrl.army.nSoldiers / (float)armyCtrl.startUnitSize, 1);
            StopAllCoroutines();
			followArmy = StartCoroutine (FollowArmy(armyCtrl.transform));
		}
		
	}


	void UnsetBars (ArmyController armyCtrl) {
		if (activeCoroutine) {
			activeCoroutine = false;
			StopCoroutine (followArmy);
			bars.SetActive (false);
		}
	}


	IEnumerator FollowArmy (Transform army) {
		while (true) {
			Vector3 position = Camera.main.WorldToScreenPoint (army.position);
			bars.transform.GetComponent<RectTransform> ().anchoredPosition = position;
			yield return null;
		}
	}
}
