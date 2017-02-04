using UnityEngine;
using System.Collections;using System;


/// <summary> Controls the movement during the deployment fase </summary>
public class DeploymentMovement: Movement {


	Vector3 offset;
	Quaternion objectRotation, offsetRotation;
	float xMin, xMax, zMin, zMax; // Boudaries  
	bool isRotating = false;


	public DeploymentMovement (Transform armyT) {
		this.armyT = armyT;
		SetBoundaries (FindDeplArea (armyT.GetComponent<ArmyController> ()).transform);
		movConstructed = true;
	}


	public override bool IsAllowed () {
		if (GameController.currentPlayer == armyT.GetComponent<ArmyController> ().player &&
			GameController.currentFase == GameFase.deployment &&
			!GameController.actionInProg) {
			return true;
		}
		return false;
	}


	public override void InitMovement() {
		GameController.actionInProg = true;
		Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
		offset = GeomF.ToYPlane (mouseScreenToWorld, Camera.main.transform.position, 0) - armyT.position;
		offsetRotation = Quaternion.FromToRotation (offset, Vector3.forward);
		objectRotation = Quaternion.FromToRotation (Vector3.forward + new Vector3 (0.01f, 0f, 1f), armyT.forward);
		startPosition = armyT.position;
		startForward = armyT.forward;
	}


	// In the coroutine loop of the movement Controller
	public override void UpdateMovement() {
		
		if (Input.GetButton("Ctrl")) isRotating = true;
		else isRotating = false;
		if (Input.GetButtonUp("Ctrl")) InitMovement();

		Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
		Vector3 mousePosition = GeomF.ToYPlane(mouseScreenToWorld, Camera.main.transform.position, 0);

		if (isRotating == true) {
			Quaternion newRotation = Quaternion.FromToRotation(Vector3.forward, mousePosition - armyT.position);
			armyT.rotation = objectRotation * offsetRotation * newRotation;
			armyT.rotation = Quaternion.Euler (0, armyT.rotation.eulerAngles.y, 0);
		} else {
			Vector3 newPosition = mousePosition - offset;
			armyT.position = new Vector3(Mathf.Clamp(newPosition.x, xMin, xMax), 0, Mathf.Clamp(newPosition.z, zMin, zMax));
		}
	}


	public override void StopMovement() {
	}


	public override float GetMovementLeft() {
		return 0;
	}


	public override void ResetMovement() { 
		movConstructed = true;
		if (armyT.GetChild (0).GetComponent<MovementController> ().movActive) {
			armyT.position = startPosition;
			armyT.rotation = Quaternion.LookRotation (startForward);
			armyT.rotation = Quaternion.Euler (0, armyT.rotation.eulerAngles.y, 0); 
		}
	}


	DeploymentArea FindDeplArea (ArmyController army) {
		GameObject[] areasGO = GameObject.FindGameObjectsWithTag ("Deployment Area");
		foreach (GameObject area in areasGO) {
			if (area.GetComponent<DeploymentArea> ().player.index == army.player.index) {
				return area.GetComponent<DeploymentArea> ();
			}
		}
		Debug.Log("Deployment area not found");
		return null;
	}


	void SetBoundaries(Transform deplArea) {
		xMin = deplArea.position.x + (-deplArea.localScale.x * 10 ) / 2.0f;
		xMax = deplArea.position.x + (deplArea.localScale.x * 10 ) / 2.0f;
		zMin = deplArea.position.z + (-deplArea.localScale.z * 10 ) / 2.0f;
		zMax = deplArea.position.z + (deplArea.localScale.z * 10 ) / 2.0f;
	}
}
	