using UnityEngine;
using System.Collections;


/// <summary>
/// Properties of an army in sigth on an active army.
/// Determines which side is seen.
/// </summary>
public class InSightArmy {

	/// <summary> Controller of the seen army </summary>
	public ArmyController armyCtrl;
	/// <summary> Side seen </summary>
	public Directions armySide;
	/// <summary> Active army controller </summary>
	public bool isAllied;
	ArmyController myCtrl;


	public InSightArmy(ArmyController otherC, ArmyController myC){
		armyCtrl = otherC;
		myCtrl = myC;
		armySide = SetArmySide();
		if (otherC.player.index == myC.player.index)
			isAllied = true;
		else
			isAllied = false;
	}


	Directions SetArmySide(){
		float angleBtwArmies = GeomF.YAngleWithSign (myCtrl.transform.position - armyCtrl.transform.position, armyCtrl.transform.forward);
		float distance = Vector3.Distance (myCtrl.transform.position, armyCtrl.transform.position);
		float a = (armyCtrl.body.width - armyCtrl.body.length) / 2.0f;
		if (a < 0)
			Debug.Log ("Army with length > width..");
		float refAngle = 45f + Mathf.Asin (a * Mathf.Sqrt (2) / 2.0f / distance) * Mathf.Rad2Deg;
		if (angleBtwArmies <= refAngle)
			return Directions.north;
		else if (refAngle < angleBtwArmies && angleBtwArmies <= (180f - refAngle))
			return Directions.west;
		else if ((180f - refAngle) < angleBtwArmies && angleBtwArmies <= (180f + refAngle))
			return Directions.south;
		else if ((180f + refAngle) < angleBtwArmies && angleBtwArmies <= (360f - refAngle))
			return Directions.east;
		else
			return Directions.north;
	}
}

