using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitPointer : MonoBehaviour {

	public float startHeight, amplitude, frequency;
	List<Transform> pointers = new List<Transform> ();
	float t0;


	void Awake () {
		GameController.ArmyTurn += SetPointers;
		pointers.Add(transform.GetChild (0).transform);
		pointers.Add(transform.GetChild (1).transform);
		pointers.Add(transform.GetChild (2).transform);
		pointers.Add(transform.GetChild (3).transform);
		foreach (Transform pointer in pointers)
			pointer.GetComponent<MeshRenderer> ().material.renderQueue = 3101;
		gameObject.SetActive (false);
	}


	void Start () {
		t0 = Time.fixedTime;
	}


	void Update () {
		transform.localPosition = new Vector3(0, Heigth(t0 - Time.fixedTime), 0);
	}


	public void SetPointers (ArmyController armyCtrl) {
		if (transform.parent == armyCtrl.transform) {
			if (!gameObject.activeSelf)
				gameObject.SetActive (true);
		} else if (gameObject.activeSelf)
			gameObject.SetActive (false);
	}


	float Heigth (float time) {
		return amplitude * Mathf.Sin (time * 2 * Mathf.PI * frequency) + startHeight;
	}
}
