using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementArea: CircularMeshArea {

	float movement, armyAreaAngle;
	float limitAngleRight = 180f;
	Transform army;


	public MovementArea(Transform army, GameObject movArea, int areaResolution, float movement, float angleOfView, float armyAreaAngle): 
	base(army.position, movArea, angleOfView, areaResolution) {
		this.army = army;
		this.movement = movement;
		this.armyAreaAngle = armyAreaAngle;
		limitAngleRight = angleOfView / 2.0f;
	}


	public override void DrawArea() {
		DestroyArea ();
		if (movement <= 0)
			return;
		areaGO.transform.GetChild (0).gameObject.SetActive (true);
		areaGO.transform.GetChild (0).transform.position = center + new Vector3 (0, 1.5f, 0);
		ComputeAreaPoints ();
		base.DrawArea ();
	}


	public Vector3 GetMovAreaBoundary (float angle) {
		if (angle > limitAngleRight && angle < (360 - limitAngleRight))
			return center;
		else
			return areaPoints [IndexFromAngle(angle)];
	}


	public float GetMovAreaRadius (float angle){
		if (angle > limitAngleRight && angle < (360 - limitAngleRight))
			return 0;
		else
			return areaRadius [IndexFromAngle(angle)];
	}


	public float[] GetLimitAngles() {
		float[] angles = new float[2];
		angles [0] = limitAngleRight;
		angles [1] = 360 - limitAngleRight;
		return angles;
	}


	void ComputeAreaPoints() {
		Transform armyBody = army.GetChild (0).transform;
		float diagonal = Mathf.Sqrt (Mathf.Pow (armyBody.localScale.x, 2) + Mathf.Pow (armyBody.localScale.y, 2));
		float stepAngleSize = spannedAngle / (float)areaResolution;
		InitVariables ();

		float angle = -spannedAngle / 2.0f;
		for (int i = 0; i < areaResolution; i++) {
			angle += stepAngleSize; // angle form the transform.forward
			float movementAfterRotation = movement - BattleF.RotationMovement (angle, diagonal);
			if (angle > 0 && movementAfterRotation <= 0 && limitAngleRight > angle)
				limitAngleRight = angle;
	
			if (movementAfterRotation > 0)
				areaRadius [i] = movementAfterRotation;
			else
				areaRadius [i] = 0;
		}

		angle = -spannedAngle / 2.0f;
		for (int i = 0; i < areaResolution; i++) {
			angle += stepAngleSize;
			areaPoints.Add (center + GeomF.DirFromAngleY (angle + army.rotation.eulerAngles.y + armyAreaAngle) * areaRadius [i]);
		}
	}


	void InitVariables() {
		areaPoints = new List<Vector3> ();
		areaRadius = new List<float> ();
		for (int i = 0; i < areaResolution; i++)
			areaRadius.Add (movement);
	}


	int IndexFromAngle (float angle) {
		if (angle > limitAngleRight && angle < (360 - limitAngleRight)) {
			return 0;
		} else if (angle < 180) {
			return (int)((areaResolution - 1) * (angle / (spannedAngle / 2.0f) + 1) / 2.0f);
		} else {
			return (int)((areaResolution - 1) * (angle - (360 - spannedAngle / 2.0f)) / (spannedAngle / 2.0f) / 2.0f);
		}
	}
}
