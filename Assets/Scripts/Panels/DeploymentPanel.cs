using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class DeploymentPanel : MonoBehaviour {

	public Button armyButton;
    public float width, offsetBtwButt = 20, buttHeight = 30, textHeight = 30;


    void Awake ()
    {
		GameController.StartGame += SelfDestruction;
		//GameController.ActionInProgress += HidePanel;
    }


	public void MakePanel(Deployment dpl, ArmyTextures textures) {
        foreach (Transform child in gameObject.transform) if (child.tag == "Button") Destroy(child.gameObject);
		GetComponentInChildren<Text>().text = dpl.playerArmyList[0].player.playerName + "'s army list.";
		int count = 0;
		for (int i = 0; i <  dpl.playerArmyList.Count; i++) {
			AddButton ( dpl.playerArmyList[i], count, dpl, textures);
			count++;
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2(width, count * (offsetBtwButt + buttHeight) + 30);
    }


	void AddButton(DeploymentStats army, int i, Deployment dpl, ArmyTextures textures) {
		float x = transform.position.x - GetComponent<RectTransform>().sizeDelta.x / 2.0f;
		float y = transform.position.y - textHeight - i * (offsetBtwButt + buttHeight);
		Button newButton = (Button)Instantiate(armyButton, new Vector3(x, y, 0), Quaternion.identity);
		newButton.transform.SetParent(transform);
		newButton.onClick.AddListener(() => dpl.addArmy(army));
		newButton.GetComponentInChildren<Text>().text = army.myName;
		newButton.transform.GetChild(1).GetComponent<Image> ().overrideSprite = textures.GetSprite(army.myClass);
	}


	void HidePanel(Action action){
		if (action == Action.nothing) gameObject.SetActive (true);
		else gameObject.SetActive (false);
	}	


    void SelfDestruction() { 
		//GameController.ActionInProgress -= HidePanel;
		Destroy(gameObject); 
	}
}
