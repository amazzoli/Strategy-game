using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOESubArea : MonoBehaviour {

    public List<ArmyController> overlappedArmies = new List<ArmyController>();


    public float radius
    {
        set { transform.localScale = new Vector3(value * 2, 0.03f, value * 2); }
        get { return transform.localScale.x / 2.0f; }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Army")
            overlappedArmies.Add(other.GetComponentInParent<ArmyController>());
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Army")
            overlappedArmies.Remove(other.GetComponentInParent<ArmyController>());
    }
}
