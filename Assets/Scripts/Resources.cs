using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour {

    public AOEArea area;
    public ModifiersSprites modifiersSprites;


    public Sprite GetModSprite(string name)
    {
        if (name == "wood")
            return modifiersSprites.woodBonus;
        else
            return null;
    }
}


[System.Serializable]
public struct ModifiersSprites
{
    public Sprite woodBonus;
}
