using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class StatPanel : UnitPanel {

	public RectTransform modBar;
	public RectTransform modSignal;
	public RectTransform moraleBar, sizeBar, energyBar;
	public Text moraleText, sizeTextr, energyText;
	public Button enlargeButton;

	float height0 = 145, height1 = 285;
	float modDistance = 2.5f;
	bool isEnlarged = false;
	List<RectTransform> modsSignals = new List<RectTransform> ();


	protected override void Awake () {
		base.Awake ();
		enlargeButton.onClick.AddListener (() => ExpandContract ());
	}
		

	public override void MakePanel (ArmyController armyCtrl) {
		base.MakePanel (armyCtrl);
		SetTextStats (armyCtrl);
		SetModifiers (armyCtrl);
	}


	public void SetTextStats (ArmyController armyCtrl) {
		myTexts [0].text = armyCtrl.army.unitName;
		myTexts [1].text = armyCtrl.army.armyClass.ToString ();
		myTexts [0].GetComponent<UIInfo> ().title = armyCtrl.army.unitName;
		myTexts [0].GetComponent<UIInfo> ().infoText = "Unit of <i>" + armyCtrl.army.faction.ToString () + "</i>'s <i><b>" + myTexts [1].text + "</b></i>, belonging to <i>" + armyCtrl.player.playerName + "</i>.";
		myTexts [1].GetComponent<UIInfo> ().title = armyCtrl.army.armyClass.ToString ();
		myTexts [1].GetComponent<UIInfo> ().infoText = armyCtrl.army.description;

		myTexts [2].text = "<i><b>Movement</b></i>: " + armyCtrl.army.movement.ToString("0.#");
		myTexts [2].GetComponent<UIInfo> ().title = "Movement";
		myTexts [2].GetComponent<UIInfo> ().infoText = "Determines the distance which the unit can cover in a single turn.";
		myTexts [3].text = "<i><b>Precision</b></i>: " +armyCtrl.army.precision.ToString("0.#") + "%";
		myTexts [3].GetComponent<UIInfo> ().title = "Precision";
		myTexts [3].GetComponent<UIInfo> ().infoText = "Determines the hit probability of physical attacks.";
		myTexts [4].text = "<i><b>Initiative</b></i>: " + armyCtrl.army.initiative.ToString("0.#");
		myTexts [4].GetComponent<UIInfo> ().title = "Initiative";
		myTexts [4].GetComponent<UIInfo> ().infoText = "Determines the army action order in each round.";
		myTexts [5].text = "<i><b>Phys att</b></i>: " + armyCtrl.army.phAtt.ToString("0.#");
		myTexts [5].GetComponent<UIInfo> ().title = "Physical attack";
		myTexts [5].GetComponent<UIInfo> ().infoText = "Strength of the physical attacks.";
		myTexts [6].text = "<i><b>Phys def</b></i>: " + armyCtrl.army.phDef.ToString("0.#");
		myTexts [6].GetComponent<UIInfo> ().title = "Physical defense";
		myTexts [6].GetComponent<UIInfo> ().infoText = "Defence against the physical attacks.";
		myTexts [7].text = "<i><b>Ment def</b></i>: " + armyCtrl.army.mentalStrength.ToString("0.#");
		myTexts [7].GetComponent<UIInfo> ().title = "Mental strength";
		myTexts [7].GetComponent<UIInfo> ().infoText = "Determines the defense against morale damages and mental attacks. Some skills base their strength on this stat.";
		myTexts [8].text = "<i><b>Elem def</b></i>: " + armyCtrl.army.neutralElemDef.ToString("0.#");
		myTexts [8].GetComponent<UIInfo> ().title = "Elemental defense";
		myTexts [8].GetComponent<UIInfo> ().infoText = "Defense against elemental attacks.";
		myTexts [9].text = "<i><b>N attacks</b></i>: " + armyCtrl.army.attNumber;
		myTexts [9].GetComponent<UIInfo> ().title = "Attack number";
		myTexts [9].GetComponent<UIInfo> ().infoText = "Number of attacks of each soldier in the army / number of attack of the hero.";
		myTexts [10].text = "<i><b>N wounds</b></i>: " + armyCtrl.army.woundsNumber;
		myTexts [10].GetComponent<UIInfo> ().title = "Number of wounds";
		myTexts [10].GetComponent<UIInfo> ().infoText = "Number of wounds of each soldier in the army / number of wounds of the hero.";

		moraleText.text = "Morale: " + (armyCtrl.morale * 100).ToString("0.#") + "%";
		moraleBar.transform.parent.GetComponent<UIInfo> ().title = "Morale";
		moraleBar.transform.parent.GetComponent<UIInfo> ().infoText = "Unit morale. If it reaches 0, the units is defeated.";
		sizeTextr.text = "Soldiers: " + armyCtrl.army.nSoldiers + "/" + armyCtrl.startUnitSize;
		sizeBar.transform.parent.GetComponent<UIInfo> ().title = "N Soldiers";
		sizeBar.transform.parent.GetComponent<UIInfo> ().infoText = "Number of soldiers left in the unit.";
		energyText.text = "Energy: " + armyCtrl.army.energy + "/" + armyCtrl.startEnergy;
		energyBar.transform.parent.GetComponent<UIInfo> ().title = "Unit energy";
		energyBar.transform.parent.GetComponent<UIInfo> ().infoText = "Unit energy. Can be used for special skills and actions.";
		moraleBar.localScale = new Vector2 (1, armyCtrl.morale);
		sizeBar.localScale = new Vector2 (1, (float)armyCtrl.army.nSoldiers / (float)armyCtrl.startUnitSize);
		energyBar.localScale = new Vector2 (1, (float)armyCtrl.army.energy / (float)armyCtrl.startEnergy);
	}


	void SetModifiers (ArmyController armyCtrl) {
		List<StatModifier> mods = armyCtrl.army.statMods;
		float yButton = modSignal.GetComponent<RectTransform> ().sizeDelta.y;
		if (mods.Count > 0) {
			modBar.gameObject.SetActive (true);
			modBar.sizeDelta = new Vector2 (modBar.sizeDelta.x, modDistance + mods.Count * (modDistance + yButton));
			for (int i = 0; i < mods.Count; i++) {
				RectTransform newModSignal = (RectTransform)Instantiate (modSignal, Vector3.zero, Quaternion.identity);
				newModSignal.transform.SetParent (transform.FindChild("Modifier Bar").transform);
				newModSignal.anchoredPosition = new Vector2 (0, - modDistance - i * (yButton + modDistance));
				newModSignal.GetComponent<UIInfo> ().title = mods [i].title;
				newModSignal.GetComponent<UIInfo> ().infoText = mods [i].description + ". <i>" + mods[i].turnsLeft + "</i> turns left.";
				modsSignals.Add (newModSignal);
			}
		} else {
			modBar.gameObject.SetActive (false);
		}
	}


	void ExpandContract () {
		if (isEnlarged) {
			isEnlarged = false;
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetComponent<RectTransform> ().sizeDelta.x, height0);
			enlargeButton.transform.GetChild (0).GetComponent<Text> ().text = "Show stats";
		} else {
			isEnlarged = true;
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetComponent<RectTransform> ().sizeDelta.x, height1);
			enlargeButton.transform.GetChild (0).GetComponent<Text> ().text = "Hide stats";
		}
		myTexts [2].transform.parent.gameObject.SetActive (isEnlarged);
	}


	public override void HidePanel (ArmyController armyCtrl) {
		if (gameObject.activeSelf) {
			foreach (RectTransform modButton in modsSignals) {
				Destroy (modButton.gameObject);
			}
			modsSignals.Clear ();
			gameObject.SetActive (false);
		}
	}
}