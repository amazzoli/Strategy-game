using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Base class of MovementArea, FieldOfView.
/// Instatiate a radial planar mesh object parallel to the y plane
/// </summary>
public abstract class PlanarMeshArea
{
    /// <summary>Center of the area</summary>
    public Vector3 center { get; protected set; }

    /// <summary>Length of the perimenter mesh segment</summary>
    protected float areaResolution;

    /// <summary>Spanned angle of the perimenter</summary>
    protected float spannedAngle;

    /// <summary>Points which defines the perimeter. The angles between radii from the center to this points are equal</summary>
    protected List<Vector3> areaPoints = new List<Vector3>();

    /// <summary>Raius length from the center to each permiter point</summary>
    //protected List<float> areaRadius = new List<float>();
    
    /// <summary>GO at which the mesh area will be added</summary>
    protected GameObject areaGO;

    Mesh movAreaMesh;


    public PlanarMeshArea(Vector3 center, GameObject movArea, float spannedAngle, float areaResolution)
    {
        this.center = center;
        this.areaGO = movArea;
        this.areaResolution = areaResolution;
        if (spannedAngle >= 360)
            spannedAngle = 360;
        this.spannedAngle = spannedAngle;
    }


    public void DrawArea()
    {
        ComputeAreaPoints();
        InstantiateArea();
    }


    protected abstract void ComputeAreaPoints();


    protected void InstantiateArea()
    {
        Vector3[] vertices = new Vector3[areaPoints.Count + 1];
        int[] triangles = new int[(vertices.Length - 1) * 3];
        vertices[0] = center;
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            vertices[i + 1] = areaPoints[i];
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            if (i == vertices.Length - 2)
            {
                if (spannedAngle == 360) triangles[i * 3 + 2] = 1;
                else triangles[i * 3 + 2] = i + 1;
            }
            else triangles[i * 3 + 2] = i + 2;
        }

        movAreaMesh = new Mesh();
        movAreaMesh.name = "Movement Area Mesh";
        movAreaMesh.Clear();
        movAreaMesh.vertices = vertices;
        movAreaMesh.triangles = triangles;
        movAreaMesh.RecalculateNormals();

        areaGO.GetComponent<MeshFilter>().mesh = movAreaMesh;
        GameObject go = GameObject.Instantiate(areaGO, new Vector3(0, 0.04f, 0), Quaternion.identity) as GameObject;
        go.GetComponent<MeshRenderer>().material.renderQueue = 3100; // This solve the "disappearing problem" of fade materials
    }


    public void DestroyArea()
    {
        if (areaGO.tag == null) Debug.Log("No tag assigned to the area");
        GameObject area = GameObject.FindGameObjectWithTag(areaGO.tag);
        GameObject.Destroy(area);
    }
}
