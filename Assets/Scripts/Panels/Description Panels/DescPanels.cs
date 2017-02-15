using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescPanels : MonoBehaviour {

    public MovPanel movPanel;
    public ChoicePanel choicePanel;
    public DamagePanel damagePanel;

    GameObject currentActivePanel;


    private void Start()
    {
        HidePanel(null);
        SelectionController.ArmyDeselection += HidePanel;
    }


    public void SetActive(string panel)
    {
        if (currentActivePanel != null)
            currentActivePanel.SetActive(false);

        if (panel == "mov")
        {
            movPanel.gameObject.SetActive(true);
            currentActivePanel = movPanel.gameObject;
        }
            
        if (panel == "choice")
        {
            choicePanel.gameObject.SetActive(true);
            currentActivePanel = choicePanel.gameObject;
        }
            
        if (panel == "damage")
        {
            damagePanel.gameObject.SetActive(true);
            currentActivePanel = damagePanel.gameObject;
        } 
    }


    public void HidePanel(ArmyController asd)
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
        }
    }
}
