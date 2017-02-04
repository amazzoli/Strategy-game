using UnityEngine;
using System.Collections;

public class SelectionResizer : MonoBehaviour {

	float width = 0.3f;
	public Transform south, west, east;
	public Transform armyBody;


	void Awake() {
	}
		

	public void HideSelectionDraw() {
		gameObject.SetActive(false);
	}


	public void ShowSelectionDraw() {
		gameObject.SetActive(true);
		ResizeSelection ();
	}


	void ResizeSelection() {
		south.localScale = new Vector3(armyBody.localScale.x - 2 * width, width, 1);
		south.localPosition = new Vector3 (0, 0, (-armyBody.localScale.y + width) / 2.0f);
		west.localScale = new Vector3(width, armyBody.localScale.y, 1);
		west.localPosition = new Vector3 ((-armyBody.localScale.x + width) / 2.0f, 0, 0);
		east.localScale = new Vector3(width, armyBody.localScale.y, 1);
		east.localPosition = new Vector3 ((armyBody.localScale.x - width) / 2.0f, 0, 0);
	}
}
