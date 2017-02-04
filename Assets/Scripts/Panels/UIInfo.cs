using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public bool onTheRight, topDown;
	public string title;
	public string infoText;
	public float secondsToWait;
	InfoPanel infoPanel;
	bool isPanelActive = false;

	public void Start () {
		if (infoPanel == null)
			infoPanel = GameObject.FindGameObjectWithTag ("Info Panel").GetComponent<InfoPanel> ();
	}


	public void OnPointerEnter (PointerEventData eventData) {
		StartCoroutine (WaitCoroutine());
		//infoPanel.MakePanel (GetComponent<RectTransform> (), title, infoText, onTheRight, topDown);
	}


	public void OnPointerExit (PointerEventData eventData) {
		StopAllCoroutines();
		infoPanel.HidePanel ();
		isPanelActive = false;
	}


	IEnumerator WaitCoroutine () {
		float t0 = Time.time;
		while (true) {
			if (!isPanelActive && (Time.time - t0) > secondsToWait) {
				infoPanel.MakePanel (GetComponent<RectTransform> (), title, infoText, onTheRight, topDown);
				isPanelActive = true;
			}
			yield return true;
		}
	}
}
