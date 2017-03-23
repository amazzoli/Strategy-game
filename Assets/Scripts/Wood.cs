using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour {

    public List<GameObject> meshGroups;
    public Material woodPalette;
    public Material ghostMaterial;
    int unitsInWoods = 0;


    public void AddUnitInWood()
    {
        unitsInWoods++;
        if (unitsInWoods == 1)
            SetGhostMode();
    }


    public void RemoveUnitFromWood()
    {
        unitsInWoods--;
        if (unitsInWoods == 0)
            SetPaletteMode();
    }


    void SetGhostMode()
    {
        foreach (GameObject group in meshGroups)
            foreach (Transform element in group.transform)
            {
                element.GetComponent<MeshRenderer>().material = ghostMaterial;
                foreach (Transform otherChild in element)
                    otherChild.GetComponent<MeshRenderer>().material = ghostMaterial;
            } 
    }


    void SetPaletteMode()
    {
        foreach (GameObject group in meshGroups)
            foreach (Transform element in group.transform)
            {
                element.GetComponent<MeshRenderer>().material = woodPalette;
                foreach (Transform otherChild in element)
                    otherChild.GetComponent<MeshRenderer>().material = woodPalette;
            }
    }
}
