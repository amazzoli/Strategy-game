﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class FieldOfView : CircularMeshArea
{

    float radius;
    bool movConstrained;
    public Transform army { get; private set; }
    public List<InSightArmy> armiesInSight = new List<InSightArmy>();


    public FieldOfView(Transform army, GameObject movArea, int areaResolution, float radius, bool movConstrained) :
    base(army.position, movArea, 90, areaResolution)
    {
        this.radius = radius;
        this.army = army;
        completeCircle = false;
        this.movConstrained = movConstrained;
    }


    public override void DrawArea()
    {
        DestroyArea();
        if (radius <= 0) return;
        areaGO.transform.GetChild(0).gameObject.SetActive(true);
        areaGO.transform.GetChild(0).transform.position = army.transform.position + new Vector3(0, 1.5f, 0);
        ComputeAreaPoints();
        base.DrawArea();
    }


    public bool IsInFieldOfView(Vector3 point)
    {
        float distance = Vector3.Distance(point, army.position);
        float angle = GeomF.YAngleWithSign(army.forward, point - army.position);
        if (distance > GetMovAreaRadius(angle))
            return false;
        else
            return true;
    }


    void ComputeAreaPoints()
    {
        float stepAngleSize = spannedAngle / (float)areaResolution;
        areaPoints = new List<Vector3>();
        armiesInSight = new List<InSightArmy>();
        Transform armyBody = army.GetChild(0).transform;
        float diagonal = Mathf.Sqrt(Mathf.Pow(armyBody.localScale.x, 2) + Mathf.Pow(armyBody.localScale.y, 2));
        float angle = -spannedAngle / 2.0f;
        float forwardAngle = GeomF.YAngleWithSign(Vector3.forward, army.forward);

        for (int i = 0; i < areaResolution; i++)
        {
            RaycastHit hit;

            float distance;
            if (movConstrained)
                distance = radius - BattleF.RotationMovement(angle, diagonal);
            else
                distance = radius;

            Vector3 direction = GeomF.DirFromAngleY(angle + forwardAngle);
            bool rayCollision = Physics.Raycast(center, direction, out hit, distance);
            if (rayCollision)
            {
                areaPoints.Add(center + direction * hit.distance);
                areaRadius.Add(hit.distance);
                if (hit.transform.tag == "Army")
                {
                    bool alreadySeen = false;
                    foreach (InSightArmy seenArmy in armiesInSight)
                    {
                        if (hit.transform.parent.gameObject == seenArmy.armyCtrl.gameObject)
                        {
                            alreadySeen = true;
                            break;
                        }
                    }
                    if (!alreadySeen)
                    {
                        ArmyController hitArmy = hit.transform.parent.transform.GetComponent<ArmyController>();
                        InSightArmy seenArmy = new InSightArmy(hitArmy, army.GetComponent<ArmyController>());
                        armiesInSight.Add(seenArmy);
                    }
                }
            }
            else
            {
                areaPoints.Add(center + direction * distance);
                areaRadius.Add(distance);
            }
            angle += stepAngleSize;
        }
    }


    public float GetMovAreaRadius(float angle)
    {
        if (angle > spannedAngle / 2.0f && angle < (360 - spannedAngle / 2.0f))
            return 0;
        else
            return areaRadius[IndexFromAngle(angle)];
    }


    int IndexFromAngle(float angle)
    {
        if (angle > spannedAngle / 2.0f && angle < (360 - spannedAngle / 2.0f))
            return 0;
        if (angle < 180)
            return (int)((areaResolution - 1) * (angle / (spannedAngle / 2.0f) + 1) / 2.0f);
        else
            return (int)((areaResolution - 1) * (angle - (360 - spannedAngle / 2.0f)) / (spannedAngle / 2.0f) / 2.0f);
    }
}