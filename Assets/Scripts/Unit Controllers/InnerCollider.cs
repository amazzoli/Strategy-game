using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerCollider : MonoBehaviour {

    ArmyController myCtrl;
    WoodBonus woodBonus;
    TerrainImage terrainImage;
    
     
    private void Start()
    {
        myCtrl = transform.parent.GetComponent<ArmyController>();
        terrainImage = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Spawners").GetComponent<TerrainImage>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wood")
        {
            woodBonus = new WoodBonus();
            myCtrl.army.passiveSkills.Add(woodBonus);
            myCtrl.currentTerrain = Terrain.wood;
            myCtrl.statPanel.MakePanel(myCtrl);
            terrainImage.SetImage(myCtrl);
            other.transform.GetComponent<Wood>().AddUnitInWood();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wood")
        {
            myCtrl.army.passiveSkills.Remove(woodBonus);
            woodBonus = null;
            terrainImage.UnsetImage(null);
            myCtrl.currentTerrain = Terrain.none;
            myCtrl.statPanel.MakePanel(myCtrl);
            other.transform.GetComponent<Wood>().RemoveUnitFromWood();
        }
    }
}
