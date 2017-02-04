using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AuxCollider : MonoBehaviour {

	BoxCollider myCollider;
	Body body;
	public List<Collider> collidersInsideMe = new List<Collider> (); 


	void Awake () {
		myCollider = GetComponent<BoxCollider> ();
		body = transform.parent.GetChild (0).transform.GetComponent<Body> ();
	}


	void Start () {
		//ResetPosition ();
	}


	void OnTriggerEnter(Collider other) {
		if (!collidersInsideMe.Contains (other) && !other.gameObject.CompareTag("Aux Collider"))
			collidersInsideMe.Add (other);
	}
		

	public void ResetPosition () {
		myCollider.size = new Vector3(body.width, 0.5f, body.width);
		myCollider.center = Vector3.zero;
		//transform.position = armyBody.position;
		transform.rotation = transform.parent.rotation;
		collidersInsideMe = new List<Collider> (); 
	}


	// Dilatate the collider toward the direction by length meters
	public void Elongate (float length, Vector3 dir) {
		transform.rotation = Quaternion.LookRotation (dir);
		myCollider.size = new Vector3 (body.width, 0.5f, length);
		myCollider.center = new Vector3 (0, 0, (length + body.length) / 2.0f);
	}


	// Cast the collider 
	public void PutOnArmySide (float x, float y, float distance, Directions side) {

		if (side == Directions.north) {
			myCollider.size = new Vector3 (x, 0.5f, y);
			myCollider.center = new Vector3 (0, 0, distance + (body.length + y) / 2.0f);
		}
		if (side == Directions.west) {
			myCollider.size = new Vector3 (y, 0.5f, x);
			myCollider.center = new Vector3 (-  distance - (body.width + y) / 2.0f, 0, 0);
		}
		if (side == Directions.south) {
			myCollider.size = new Vector3 (x, 0.5f, y);
			myCollider.center = new Vector3 (0, 0, - distance - (body.length + y) / 2.0f);
		}
		if (side == Directions.east) {
			myCollider.size = new Vector3 (y, 0.5f, x);
			myCollider.center = new Vector3 (distance + (body.width + y) / 2.0f, 0, 0);
		}
	}
}
