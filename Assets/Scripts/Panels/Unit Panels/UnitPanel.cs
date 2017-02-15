using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


// Base class of stat panel, action panel, combat panel. All the panels related to a unit
public class UnitPanel : MonoBehaviour {

	protected RectTransform myTrans;
	public List<Text> myTexts;


	protected virtual void Awake () {
		SelectionController.ArmySelection += MakePanel;
		SelectionController.ArmyDeselection += HidePanel;
		myTrans = GetComponent<RectTransform> ();
		gameObject.SetActive(false);
	}


	public virtual void MakePanel (ArmyController armyCtrl) {
		if (!gameObject.activeSelf) 
			gameObject.SetActive(true);
		SetColors (armyCtrl.player.color);
	}


	public virtual void HidePanel (ArmyController armyCtrl) {
		if (gameObject.activeSelf) {
			gameObject.SetActive (false);
		}
	}


	void SetColors (Color playerColor) {
		Color panelColor = playerColor;
		panelColor.a = 0.66f;
		GetComponent<Image>().color = panelColor;
		Color textColor = Color.black;
		if (panelColor.b + panelColor.g + panelColor.r < 1.5f) textColor = Color.white;
		foreach (Text text in myTexts)
			text.color = textColor;
	}
}
