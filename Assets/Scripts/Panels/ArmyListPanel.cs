using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ArmyListPanel : MonoBehaviour {

    public Button armyButton;
	public RectTransform highlighter;
    public float topSpace, offsetBtwButt, bottomSpace, width;

	float buttonHeight;
	List<Button> myButtons = new List<Button> ();
	List<ArmyController> armyList = new List<ArmyController> ();


    void Awake () {
		GameController.ArmyTurn += SetHighlighter;
    }


	public void MakePanel(List<ArmyController> armies, string title) {
		transform.GetChild (1).GetComponent<Text> ().text = title;
		DeleteCurrentButtons ();
		armyList = armies;
		gameObject.SetActive (true);
		buttonHeight = armyButton.GetComponent<RectTransform> ().sizeDelta.y;
		GetComponent<RectTransform>().sizeDelta = new Vector2(width, topSpace + bottomSpace + armyList.Count * (buttonHeight+ offsetBtwButt));
		for (int i = 0; i < armyList.Count; i++)
			AddButton (armies [i], i, buttonHeight);
    }


	public void SetHighlighter (ArmyController armyCtrl) {
		int rank = armyList.IndexOf (armyCtrl);
		float y = - topSpace - 0.5f * buttonHeight - rank * (offsetBtwButt + buttonHeight);
		highlighter.anchoredPosition = new Vector2(0, y);
		highlighter.sizeDelta = new Vector2 (highlighter.sizeDelta.x, buttonHeight + offsetBtwButt);
	}


	void AddButton (ArmyController army, int i, float buttonHeight) {
		Button newButton = (Button)Instantiate(armyButton, Vector3.zero, Quaternion.identity);
		RectTransform rTrans = newButton.GetComponent<RectTransform> ();
		PanelF.SetButtonColors (newButton, army);
		newButton.transform.SetParent(transform);
		rTrans.anchoredPosition = new Vector2 (0, - topSpace - i * (buttonHeight + offsetBtwButt));
		newButton.GetComponentInChildren<Text>().text = army.army.unitName;
		newButton.transform.GetChild(1).GetComponent<Image> ().overrideSprite = army.spriteSymbol;
		newButton.onClick.AddListener (() => army.body.transform.GetComponent<Body> ().Select ());
		myButtons.Add (newButton);

		newButton.gameObject.AddComponent<UIInfo> ();
		newButton.GetComponent<UIInfo> ().topDown = true;
		newButton.GetComponent<UIInfo> ().secondsToWait = 0.5f;
		newButton.GetComponent<UIInfo> ().title = army.army.unitName;
		newButton.GetComponent<UIInfo> ().infoText = "<i><b>Initiative</b></i>: " + army.army.initiative;
	}


	void HidePanel(Action action){
		if (action == Action.nothing) gameObject.SetActive (true);
		else gameObject.SetActive (false);
	}


    void DeleteCurrentButtons()
    {
        foreach (Button butt in myButtons)
        {
            butt.onClick.RemoveAllListeners();
            Destroy(butt.gameObject);
        }
        myButtons.Clear();
    }
}
