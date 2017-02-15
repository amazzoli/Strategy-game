using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public static class PanelF
{
    public static void SetButtonColors(Button newButton, Color color)
    {
        ColorBlock cb = newButton.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.disabledColor = color;
        color.a = 0.7f;
        cb.pressedColor = color;
        newButton.colors = cb;
        Color textColor = Color.black;
        if (color.b + color.g + color.r < 1.5f) textColor = Color.white;
        newButton.transform.GetChild(0).GetComponent<Text>().color = textColor;
    }


    public static void SetButtonColors(Button newButton, ArmyController armyCtrl)
    {
        SetButtonColors(newButton, armyCtrl.player.color);
    }
}