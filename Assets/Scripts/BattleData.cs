using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BattleData: MonoBehaviour {

	public Player[] players;
	public DeploymentArea[] deploymentAreas;
	public GameObject armyPrefab;
	public ArmyTextures textures;
	public List<DeploymentStats> armyList = new List<DeploymentStats> (); // -- List of the army stat to instantiate



	void Start() {
		//DontDestroyOnLoad (	);
	}
}


[System.Serializable]
public class ArmyTextures {
	public Texture lancerTexture, shellfighterTexture, spongezookerTexture, macerTexture;
	public Sprite lancerSprite, shellfighterSprite, spongezookerSprite, macerSprite;


	public Texture GetTexture (Classes aClass) {
		if (aClass == Classes.Lancers)
			return lancerTexture;
		if (aClass == Classes.Shellfighters)
			return shellfighterTexture;
		if (aClass == Classes.Spongezookers)
			return spongezookerTexture;
        if (aClass == Classes.Macers)
            return macerTexture;
        return null;
	}

	public Sprite GetSprite (Classes aClass) {
		if (aClass == Classes.Lancers)
			return lancerSprite;
        if (aClass == Classes.Macers)
            return macerSprite;
        if (aClass == Classes.Shellfighters)
			return shellfighterSprite;
		if (aClass == Classes.Spongezookers)
			return spongezookerSprite;
		return null;
	}
}

