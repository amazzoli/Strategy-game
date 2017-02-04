using UnityEngine;
using System.Collections;

public class FlagColors : MonoBehaviour {

	public GameObject[] flagComponents;
	public GameObject front;
	public GameObject back;


	void Start() {
		foreach (GameObject component in flagComponents) {
			component.GetComponent<Renderer> ().material.color = transform.parent.GetComponent<ArmyController>().player.color;
		}
	}


	public void SetFlag(Texture symbol) {
		front.GetComponent<Renderer> ().material.mainTexture = symbol;
		back.GetComponent<Renderer> ().material.mainTexture = symbol;
	}

}
