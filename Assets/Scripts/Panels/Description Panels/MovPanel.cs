using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovPanel : MonoBehaviour {

    public Text movDone, rotationMov, straightMov, movInWood;
    DescPanels descPanels;
    float heightInWood = 95, normalHeight = 80;
    float currentHeight;


    public void MakePanel(float movement)
    {
        if (descPanels == null)
            descPanels = transform.parent.GetComponent<DescPanels>();
        descPanels.SetActive("mov");

        SetTexts(movement, 0, 0, 0, 1);
        SetHeight(normalHeight);
    }


    public void SetTexts(float movLeft, float rot, float straight, float wood, float friction)
    {
        string frict = "";
        if (friction != 1)
            frict = "*" + friction;

        float done = friction * (rot + straight + wood);
        if (done > movLeft)
            done = movLeft;

        movDone.text = "Movement done = " + done.ToString("0.0") + " / " + movLeft.ToString("0.#");
        rotationMov.text = "Rotation = " + rot.ToString("0.0") + frict;
        straightMov.text = "Straight movement = " + (straight - wood).ToString("0.0") + frict;
        if (wood == 0)
        {
            SetHeight(normalHeight);
            if (movInWood.gameObject.activeSelf)
                movInWood.gameObject.SetActive(false);
        }
        else
        {
            SetHeight(heightInWood);
            if (!movInWood.gameObject.activeSelf)
                movInWood.gameObject.SetActive(true);
            movInWood.text = "Straight movement in wood = " + wood.ToString("0.0") + "*2" + frict;
        }
    }


    void SetHeight(float height)
    {
        if (height != currentHeight)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, height);
            currentHeight = height;
        }
    }
            


    public void HidePanel() { transform.parent.GetComponent<DescPanels>().HidePanel(null); }
}
