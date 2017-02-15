using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovPanel : MonoBehaviour {

    public Text straightMov, rotationMov, movDone, movLeft;
    DescPanels descPanels;


    public void MakePanel(float movement)
    {
        if (descPanels == null)
            descPanels = transform.parent.GetComponent<DescPanels>();
        descPanels.SetActive("mov");

        straightMov.text = "Straight movement done = 0.0";
        rotationMov.text = "Rotation movement done = 0.0";
        movDone.text = "Total movement done = 0.0";
        movLeft.text = "Movement left = " + movement.ToString("0.0");
    }


    public void HidePanel() { transform.parent.GetComponent<DescPanels>().HidePanel(null); }
}
