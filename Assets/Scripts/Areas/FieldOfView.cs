using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class FieldOfView : ArmyArea
{
    public List<InSightArmy> armiesInSight = new List<InSightArmy>();


    public FieldOfView(Transform army, GameObject movArea, float radius, bool movConstrained) :
    base(army, movArea, 0.2f, radius, 90, 0, true, true)
    { }


    public bool IsInFieldOfView(Vector3 point)
    {
        float distance = Vector3.Distance(point, army.position);
        float angle = GeomF.YAngleWithSign(army.forward, point - army.position);
        if (distance > GetMovAreaRadius(angle))
            return false;
        else
            return true;
    }


    protected override void ActionOnSeenArmy(ArmyController hitArmy)
    {
        bool alreadySeen = false;
        foreach (InSightArmy seenArmy in armiesInSight)
        {
            if (hitArmy == seenArmy.armyCtrl)
            {
                alreadySeen = true;
                break;
            }
        }
        if (!alreadySeen)
        {
            InSightArmy seenArmy = new InSightArmy(hitArmy, army.GetComponent<ArmyController>());
            armiesInSight.Add(seenArmy);
        }
    }
}