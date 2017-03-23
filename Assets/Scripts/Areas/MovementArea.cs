using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementArea : ArmyArea
{

    public MovementArea(Transform army, GameObject movArea, float movement, float angleOfView, float armyAreaAngle) :
    base(army, movArea, 0.1f, movement, angleOfView, armyAreaAngle, true, true) { }


    public Vector3 GetMovAreaBoundary(float angle)
    {
        if (angle > limitAngleRight && angle < (360 - limitAngleRight))
            return center;
        else
            return areaPoints[IndexFromAngle(angle)];
    }


    public float MovementDoneInWood(float angle, float straightMov)
    {
        if (angle > limitAngleRight && angle < (360 - limitAngleRight))
            return 0;
        else
            return radii[IndexFromAngle(angle)].GetMovInWood(straightMov);
    }
}
