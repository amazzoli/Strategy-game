using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Create a plane shape gameObject given a center a list of points which define the perimenter
// Base class of MovementArea, FieldOfView
public class CircularMeshArea {

	protected int areaResolution; // --------------------------- Number of areaPoints
	protected float spannedAngle;
	protected List<Vector3> areaPoints = new List<Vector3>(); // Boundary of the area
	protected List<float> areaRadius = new List<float>(); // --- Length of the rays from the center
	public Vector3 center { get; protected set; }
	protected GameObject areaGO; // ---------------------------- GO to instantiate. The material must be attached 
	protected bool completeCircle; // -------------------------- If the shape spans a 360 angle
	Mesh movAreaMesh;


	public CircularMeshArea (Vector3 center, GameObject movArea, float spannedAngle, int areaResolution) {
		this.center = center;
		this.areaGO = movArea;
		this.areaResolution = areaResolution;
		completeCircle = false;
		if (spannedAngle >= 360) {
			spannedAngle = 360;
			completeCircle = true;
		}
		this.spannedAngle = spannedAngle;
	}


	public virtual void DrawArea() {
		InstantiateArea ();
	} 


	protected void InstantiateArea() {

		Vector3[] vertices = new Vector3[areaPoints.Count + 1];
		int[] triangles = new int[(vertices.Length - 1)*3];
		vertices [0] = center;
		for (int i = 0; i < vertices.Length - 1; i++) {
			vertices [i + 1] = areaPoints [i];
			triangles [i * 3] = 0;
			triangles [i * 3 + 1] = i + 1;
			if (i == vertices.Length - 2) {
				if (completeCircle) triangles [i * 3 + 2] = 1;
				else triangles [i * 3 + 2] = i + 1;
			}
			else triangles [i * 3 + 2] = i + 2;
		}

		movAreaMesh = new Mesh ();
		movAreaMesh.name = "Movement Area Mesh";
		movAreaMesh.Clear ();
		movAreaMesh.vertices = vertices;
		movAreaMesh.triangles = triangles;
		movAreaMesh.RecalculateNormals();	

		areaGO.GetComponent<MeshFilter>().mesh = movAreaMesh;
		GameObject go = GameObject.Instantiate (areaGO, new Vector3(0,0.04f,0), Quaternion.identity) as GameObject;
		go.GetComponent<MeshRenderer> ().material.renderQueue = 3100; // This solve the "disappearing problem" of fade materials
	}


	public void DestroyArea() {
		if (areaGO.tag == null) Debug.Log ("No tag assigned to the area");
		GameObject area = GameObject.FindGameObjectWithTag (areaGO.tag);
		GameObject.Destroy (area);
	}
}
