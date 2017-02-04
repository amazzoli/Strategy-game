using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEArea : MonoBehaviour {

    GameObject internalArea;
    Coroutine myMovement;

    public float externalRadius
    {
        get { return transform.localScale.x / 2.0f; }
        set { transform.localScale = new Vector3(value * 2, 0.01f, value * 2); }
    }
    

    public float internalRadiusProportion
    {
        get
        {
            if (internalArea == null)
                internalArea = transform.GetChild(0).gameObject;
            return internalArea.transform.localScale.x;
        }
        set
        {
            if (internalArea == null)
                internalArea = transform.GetChild(0).gameObject;

            if (value < 0 )
                internalArea.transform.localScale = new Vector3(0, 0, 0);
            if (value > 1)
                internalArea.transform.localScale = new Vector3(1, 1, 1);
            else
                internalArea.transform.localScale = new Vector3(value, 1, value);
        }
    }


    private void Start()
    {
        if (internalArea == null)
            internalArea = transform.GetChild(0).gameObject;
        GetComponent<MeshRenderer>().material.renderQueue = 3101;
        internalArea.GetComponent<MeshRenderer>().material.renderQueue = 3102;
    }


    public void StartMyMovement()
    {
        myMovement = StartCoroutine(MyMovement());
    }


    public void StopMyMovement()
    {
        StopCoroutine(myMovement);
    }


    IEnumerator MyMovement()
    {
        while (true)
        {
            Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 mousePosition = GeomF.ToYPlane(mouseScreenToWorld, Camera.main.transform.position, 0.05f);
            transform.position = mousePosition;
            yield return null;
        }
    }
}
