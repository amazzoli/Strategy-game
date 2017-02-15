using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEArea : MonoBehaviour {

    public AOESubArea subAreaPrefab;
    // Sub areas in decreasing order of radius!
    public List<AOESubArea> subAreas = new List<AOESubArea>();
    GameObject cylinder;
    public FieldOfView field;
    public delegate bool MoveCondition(AOEArea area);
    ErrorPanel errorPanel;


    public void MakeArea(int nSubAreas, float maxRadius)
    {
        errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();
        for (int i=0; i<nSubAreas; i++)
        {
            cylinder = transform.GetChild(0).gameObject;
            AOESubArea newArea = Instantiate(subAreaPrefab, Vector3.zero, Quaternion.identity);
            newArea.transform.SetParent(this.transform);
            newArea.transform.localPosition = Vector3.zero;
            newArea.radius = maxRadius * (1 - i / (float)nSubAreas);
            newArea.GetComponent<MeshRenderer>().material.renderQueue = 3105 + i;
            subAreas.Add(newArea);
        }
    }


    public void StartMyMovement(ArmyControllersDlg methodOverArmies, MoveCondition condition, string errorMessage)
    {
        StartCoroutine(MyMovement(methodOverArmies, condition, errorMessage));
    }


    public void StopMyMovement()
    {
        StopAllCoroutines();
    }


    public List<ArmyController> GetOverlappedArmies(int subAreaIndex)
    {
        return subAreas[subAreaIndex].GetComponent<AOESubArea>().overlappedArmies;
    }


    IEnumerator MyMovement(ArmyControllersDlg methodOverArmies, MoveCondition condition, string errorMessage)
    {
        yield return new WaitForSeconds(2 * Time.deltaTime);
        while (true)
        {
            Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 mousePosition = GeomF.ToYPlane(mouseScreenToWorld, Camera.main.transform.position, 0.05f);
            transform.position = mousePosition;
            if (Input.GetButtonDown("Fire1"))
            {
                if (condition(this))
                    methodOverArmies(GetOverlappedArmies(0));
                else
                    errorPanel.LaunchErrorText(errorMessage);
            }
            cylinder.SetActive(condition(this));
            yield return null;
        }
    }
}
