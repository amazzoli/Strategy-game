using UnityEngine;
using System.Collections;

[System.Serializable]
public class DeploymentStats {

    public Player player;
	public string myName;
	public int nSoldiers;
	[HideInInspector] public bool isInstatiated = false;
	public Classes myClass ;

}
