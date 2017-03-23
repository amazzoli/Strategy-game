using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TerrainImage : MonoBehaviour {

    public Image myImage;
    public Vector3 offset;
    public Resources resources;
    Coroutine followArmy;
    bool activeCoroutine;
    Sprite woodSymbol;

    void Awake()
    {
        SelectionController.ArmySelection += SetImage;
        SelectionController.ArmyDeselection += UnsetImage;
        woodSymbol = resources.modifiersSprites.woodBonus;
        myImage.enabled = false;
    }


    public void SetImage(ArmyController armyCtrl)
    {
        if (armyCtrl.currentTerrain == Terrain.wood)
        {
            myImage.enabled = true;
            myImage.sprite = woodSymbol;
            activeCoroutine = true;
            StopAllCoroutines();
            followArmy = StartCoroutine(FollowArmy(armyCtrl.transform));
        }
    }


    public void UnsetImage(ArmyController armyCtrl)
    {
        if (activeCoroutine)
        {
            activeCoroutine = false;
            StopCoroutine(followArmy);
            myImage.enabled = false;
        }
    }


    IEnumerator FollowArmy(Transform army)
    {
        while (true)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(army.position);
            if (GameController.currentFase == GameFase.battle)
                myImage.transform.GetComponent<RectTransform>().anchoredPosition = position + offset;
            else
                myImage.transform.GetComponent<RectTransform>().anchoredPosition = position;
            yield return null;
        }
    }
}
