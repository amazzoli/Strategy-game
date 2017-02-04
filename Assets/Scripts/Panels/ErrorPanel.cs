using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ErrorPanel : MonoBehaviour {

	public Text errorText;
	public float secondOfDuration;


	void Start () {
		gameObject.SetActive (false);
	}

	public void LaunchErrorText(string text) {
		StopAllCoroutines ();
		gameObject.SetActive (true);
		StartCoroutine (PanelDuration());
		errorText.text = text;
	}


	public void ClosePanel() {
		gameObject.SetActive (false);
		StopCoroutine (PanelDuration());
	}


	IEnumerator PanelDuration () {
		yield return new WaitForSeconds (secondOfDuration);
		ClosePanel ();
	}
}
