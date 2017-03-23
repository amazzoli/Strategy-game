using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyArea : PlanarMeshArea
{
    float maxRadius;
    float armyAreaAngle;
    protected float limitAngleRight;
    float stepAngle;
    int nRadii;
    bool movConstrained;
    bool stopAtObstacles;
    bool isInWood;
    protected Transform army;
    protected List<AreaRadius> radii = new List<AreaRadius>();


    public ArmyArea(Transform army, 
        GameObject movArea, 
        float areaResolution, 
        float maxRadius, 
        float angleOfView, 
        float armyAreaAngle, 
        bool movConstrained,
        bool stopAtObstacles) :
    base(army.position, movArea, angleOfView, areaResolution)
    {
        this.army = army;
        this.maxRadius = maxRadius;
        this.armyAreaAngle = armyAreaAngle;
        this.movConstrained = movConstrained;
        this.stopAtObstacles = stopAtObstacles;
        stepAngle = areaResolution / maxRadius * Mathf.Rad2Deg;
        limitAngleRight = angleOfView / 2.0f;
        nRadii = (int)(angleOfView / stepAngle);
        DrawArea();
    }


    protected override void ComputeAreaPoints()
    {
        // Initialization
        areaGO.transform.GetChild(0).gameObject.SetActive(true);
        areaGO.transform.GetChild(0).transform.position = center + new Vector3(0, 1.5f, 0);
        Transform armyBody = army.GetChild(0).transform;
        float diagonal = Mathf.Sqrt(Mathf.Pow(armyBody.localScale.x, 2) + Mathf.Pow(armyBody.localScale.y, 2));
        float forwardAngle = GeomF.YAngleWithSign(Vector3.forward, army.forward);
        isInWood = false;
        if (army.GetComponent<ArmyController>().currentTerrain == Terrain.wood)
            isInWood = true;

        float angle = (armyAreaAngle + forwardAngle - spannedAngle / 2.0f) % 360;
        for (int i = 0; i < nRadii; i++)
        {
            angle += stepAngle;
            float relativeAngle = angle - forwardAngle - armyAreaAngle;
            Vector3 direction = GeomF.DirFromAngleY(angle);

            float maxDistance;
            if (movConstrained)
                maxDistance = maxRadius - BattleF.RotationMovement(relativeAngle, diagonal);
            else
                maxDistance = maxRadius;

            if (relativeAngle > 0 && maxDistance <= 0 && limitAngleRight > relativeAngle)
                limitAngleRight = relativeAngle;

            radii.Add(ComputeAreaRadius(direction, maxDistance));
            areaPoints.Add(center + GeomF.DirFromAngleY(angle) * radii[i].length);
        }
    }


    AreaRadius ComputeAreaRadius(Vector3 dir, float distance)
    {
        if (distance > 0)
        { 
            if (!stopAtObstacles)
                return new AreaRadius(distance);

            RaycastHit hit;
            bool hitSomething = Physics.Raycast(center, dir, out hit, distance);
            if (movConstrained)
            {
                if (isInWood || (hitSomething && hit.collider.tag == "Wood"))
                    return ComputeWoodPath(dir, distance);
                else if (hitSomething)
                {
                    if (hit.collider.tag == "Army")
                        ActionOnSeenArmy(hit.transform.parent.GetComponent<ArmyController>());
                    return new AreaRadius(hit.distance);
                }
                else
                    return new AreaRadius(distance);
            }
            else
            {
                if (hitSomething && hit.collider.tag != "Wood")
                {
                    if (hit.collider.tag == "Army")
                        ActionOnSeenArmy(hit.transform.parent.GetComponent<ArmyController>());
                    return new AreaRadius(hit.distance);
                } 
                else
                    return new AreaRadius(distance);
            }

        }
        else
            return new AreaRadius(0);
    }


    AreaRadius ComputeWoodPath(Vector3 direction, float maxDistance)
    {
        List<RaycastHit> forwardHits = new List<RaycastHit>(Physics.RaycastAll(center, direction, maxDistance));
        List<RaycastHit> backHits = new List<RaycastHit>(Physics.RaycastAll(center + direction * maxDistance, -direction, maxDistance));
        //Creo la lista contenente le info su collinders incontrati (distanza in ingresso, in uscita e tag)
        List<ColliderInSight> rayInColliders = RayInColliders(forwardHits, backHits, maxDistance, isInWood);
        //foreach (ColliderInSight coll in rayInColliders)
            //coll.PrintRayInCollider();

        float maxMovement = maxDistance;
        List<ColliderInSight> newRayInColliders = new List<ColliderInSight>(); // nuova lista di colliders vincolata al nuovo movimento
        foreach (ColliderInSight rayInCollider in rayInColliders)
        {
            if (rayInCollider.inDistance < maxMovement)
            {
                newRayInColliders.Add(rayInCollider);
                if (rayInCollider.name != "Wood")
                {
                    if (rayInCollider.name == "Army")
                        ActionOnSeenArmy(rayInCollider.collider.transform.parent.GetComponent<ArmyController>());
                    maxMovement = rayInCollider.inDistance;
                }
                else
                {
                    float newMaxMovement = maxMovement - rayInCollider.outDistance + rayInCollider.inDistance;
                    if (rayInCollider.outDistance < newMaxMovement)
                        maxMovement = newMaxMovement;
                    else
                        maxMovement = (maxMovement - rayInCollider.inDistance) / 2.0f + rayInCollider.inDistance;
                }
            }
        }
        return new AreaRadius(maxMovement, newRayInColliders);
    }


    protected virtual void ActionOnSeenArmy(ArmyController armyCtrl) { }


    public float[] GetLimitAngles()
    {
        float[] angles = new float[2];
        angles[0] = limitAngleRight;
        angles[1] = 360 - limitAngleRight;
        return angles;
    }


    public float GetMovAreaRadius(float angle)
    {
        if (angle > limitAngleRight && angle < (360 - limitAngleRight))
            return 0;
        else
            return radii[IndexFromAngle(angle)].length;
    }


    protected int IndexFromAngle(float angle)
    {
        if (angle > limitAngleRight && angle < (360 - limitAngleRight))
            return 0;
        else if (angle < 180)
            return (int)((nRadii - 1) * (angle / (spannedAngle / 2.0f) + 1) / 2.0f);
        else
            return (int)(nRadii * (angle - (360 - spannedAngle / 2.0f)) / (spannedAngle / 2.0f) / 2.0f);
    }


    List<ColliderInSight> RayInColliders(List<RaycastHit> forwardHits, List<RaycastHit> backHits, float maxDistance, bool inWood)
    {
        int[] sortedIndexes = new int[forwardHits.Count]; //Contiene gli indici ordinati per distanza in ingresso
        for (int i = 0; i < forwardHits.Count; i++) //Ordino per distanza gli hits in ingresso
        {
            int count = 0;
            foreach (RaycastHit hit in forwardHits)
                if (forwardHits[i].distance > hit.distance)
                    count++;
            sortedIndexes[count] = i;
        }

        // Creo la lista dei colliders ordinati per distanza
        List<ColliderInSight> colliders = new List<ColliderInSight>();
        if (inWood) // Caso di armata in bosco
        {
            float exitDistance = maxDistance;
            foreach (RaycastHit bhit in backHits)
                if (bhit.collider.tag == "Wood")
                    if (maxDistance - bhit.distance < exitDistance)
                        exitDistance = maxDistance - bhit.distance;
            colliders.Add(new ColliderInSight(0, exitDistance, null, "Wood"));
        }
        foreach (int i in sortedIndexes)
        {
            float exitDistance = maxDistance;
            foreach (RaycastHit bhit in backHits)
                if (bhit.collider.tag == forwardHits[i].collider.tag)
                {
                    if (maxDistance - bhit.distance > forwardHits[i].distance && maxDistance - bhit.distance < exitDistance)
                        exitDistance = maxDistance - bhit.distance;
                }

            colliders.Add(new ColliderInSight(forwardHits[i].distance, exitDistance, forwardHits[i].collider, forwardHits[i].collider.tag));
        }
        return colliders;
    }
}


/// <summary>Contains all the information of an area radius: length, terrain info</summary>
public struct AreaRadius
{
    public float length;
    public List<ColliderInSight> colliders;

    public AreaRadius(float length)
    {
        this.length = length;
        colliders = new List<ColliderInSight>();
    }

    public AreaRadius(float length, List<ColliderInSight> seenColliders)
    {
        this.length = length;
        colliders = seenColliders;
    }

    public float GetMovInWood(float distanceFromCenter)
    {
        float mov = 0;
        foreach (ColliderInSight collider in colliders)
            if (distanceFromCenter > collider.inDistance)
                if (distanceFromCenter > collider.outDistance)
                    mov += collider.outDistance - collider.inDistance;
                else
                    mov += distanceFromCenter - collider.inDistance;
        return mov;
    }
}


public struct ColliderInSight
{
    public float inDistance; // distanza di ingresso
    public float outDistance; // distanza uscita
    public Collider collider; // tag collider
    public string name;

    public ColliderInSight(float inDistance, float outDistance, Collider collider, string name)
    {
        this.inDistance = inDistance;
        this.outDistance = outDistance;
        this.collider = collider;
        this.name = name;
    }

    public void PrintRayInCollider()
    {
        Debug.Log(name + " " + inDistance + " " + outDistance);
    }
}