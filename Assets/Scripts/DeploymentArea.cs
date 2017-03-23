using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeploymentArea : MonoBehaviour
{

    public Player player;
    public Material lineMaterial;
    List<LineRenderer> myLines = new List<LineRenderer>();


    void Awake()
    {     
        transform.name = "Deployment Area Player " + player.index;
        GameController.StartGame += SelfDestruction;
    }


    public void SetVisible(int playerIndex)
    {
        if (playerIndex == player.index)
        {
            //GetComponent<MeshRenderer>().material.renderQueue = 3090;
            DrawLines();
        }
        else
            CancelLines();
    }


    void DrawLines()
    {
        Vector3 myPos = transform.position;
        Vector3 northPoint = transform.forward * transform.localScale.z * 5f;
        Vector3 westPoint = - transform.right * transform.localScale.x * 5f;
        Vector3 southPoint = - transform.forward * transform.localScale.z * 5f;
        Vector3 eastPoint = transform.right * transform.localScale.x * 5f;
        myLines.Add(OtherF.DrawLine(myPos + northPoint + westPoint, myPos + northPoint + eastPoint, lineMaterial, false));
        myLines.Add(OtherF.DrawLine(myPos + northPoint + eastPoint, myPos + southPoint + eastPoint, lineMaterial, false));
        myLines.Add(OtherF.DrawLine(myPos + southPoint + eastPoint, myPos + southPoint + westPoint, lineMaterial, false));
        myLines.Add(OtherF.DrawLine(myPos + southPoint + westPoint, myPos + northPoint + westPoint, lineMaterial, false));
    }


    void CancelLines()
    {
        foreach (LineRenderer line in myLines)
            Destroy(line.gameObject);
        myLines.Clear();
    }


    void SelfDestruction()
    {
        CancelLines();
        Destroy(gameObject);
    }
}
