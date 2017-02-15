using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public bool onTheRight, topDown;
	public string title;
	public string infoText;
	public float secondsToWait;
    public bool show = true;
	InfoPanel infoPanel;
	bool isPanelActive = false;


	public void Start () {
		if (infoPanel == null)
			infoPanel = GameObject.FindGameObjectWithTag ("Info Panel").GetComponent<InfoPanel> ();
        show = true;
    }


	public void OnPointerEnter (PointerEventData eventData) {
        if (show)
            StartCoroutine(WaitCoroutine());
	}


	public void OnPointerExit (PointerEventData eventData) {
        if (show)
        {
            StopAllCoroutines();
            infoPanel.HidePanel();
            isPanelActive = false;
        }
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
