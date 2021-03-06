﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BattleData : MonoBehaviour
{

    public Player[] players;
    public DeploymentArea[] deploymentAreas;
    public GameObject armyPrefab;
    public ArmyTextures textures;
    public List<DeploymentStats> armyList = new List<DeploymentStats>(); // -- List of the army stat to instantiate



    void Start()
    {
        //DontDestroyOnLoad (	);
    }
}


[System.Serializable]
public class ArmyTextures
{
    public Texture lancerTexture, shellfighterTexture, spongezookerTexture, macerTexture;
    public Sprite lancerSprite, shellfighterSprite, spongezookerSprite, macerSprite;


    public Texture GetTexture(Class aClass)
    {
        if (aClass == Class.Lancers)
            return lancerTexture;
        if (aClass == Class.Shellfighters)
            return shellfighterTexture;
        if (aClass == Class.Spongezookers)
            return spongezookerTexture;
        if (aClass == Class.Macers)
            return macerTexture;
        return null;
    }

    public Sprite GetSprite(Class aClass)
    {
        if (aClass == Class.Lancers)
            return lancerSprite;
        if (aClass == Class.Macers)
            return macerSprite;
        if (aClass == Class.Shellfighters)
            return shellfighterSprite;
        if (aClass == Class.Spongezookers)
            return spongezookerSprite;
        return null;
    }
}

