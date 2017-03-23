using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Manages the army size and collider.
/// Must be attached to the army body gameobject.
/// </summary>
public class Body : MonoBehaviour
{

    /// <summary> Gets the army length. </summary>
    public float length { get { return transform.localScale.y; } }
    /// <summary> Gets the army width. </summary>
    public float width { get { return transform.localScale.x; } }

    public Material lineMaterial;

    bool markedAsAttackable;
    float lineLength;
    bool drawLines;
    SelectionController sc;
    ArmyController armyCtrl;
    ArmyControllerDlg methodBySelection;
    List<LineRenderer> linesDuringAttack = new List<LineRenderer>();


    void Start()
    {
        markedAsAttackable = false;
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SelectionController>();
    }


    void OnMouseDown()
    {
        if (markedAsAttackable)
        {
            methodBySelection(armyCtrl);
            if (drawLines)
                CancelLines();
        }
            
        else
            Select();
    }


    void OnMouseEnter()
    {
        if (markedAsAttackable)
        {
            GetComponent<Renderer>().material.color = Color.red;
            if (drawLines)
                DrawLinesDuringAttack();
        }         
    }


    void OnMouseExit()
    {
        if (markedAsAttackable)
        {
            GetComponent<Renderer>().material.color = armyCtrl.player.color;
            if (drawLines)
                CancelLines();
        }
    }


    public void Select()
    {
        sc.SetSelection(transform.parent.GetComponent<ArmyController>());
    }


    /// <summary> Resize the army according to the current number of units. </summary>
    public void Resize()
    {
        if (armyCtrl == null)
            armyCtrl = transform.parent.GetComponent<ArmyController>();
        Transform flag = armyCtrl.flag.transform;
        Vector2 armySize = BattleF.GetArmySize(armyCtrl.army.nSoldiers, armyCtrl.army.areaPerSoldier, armyCtrl.army.heightWidthRatio);
        armyCtrl.battleSymbol.transform.localPosition = new Vector3(0, 0.91f, armySize[1] * 0.6f);
        flag.localPosition = new Vector3(0, flag.localPosition.y, armySize[1] * 3.0f / 8.0f);
        transform.localScale = new Vector3(armySize[0], armySize[1], transform.localScale.z);
    }


    /// <summary> Effective army length considering the enemies engaged  </summary>
    public float GetEffectiveLength()
    {
        float _length = length;
        if (armyCtrl.enemiesEngaged.GetEnemy(Directions.east) != null)
        {
            if (armyCtrl.enemiesEngaged.GetEnemy(Directions.east).body.width > length)
                _length = armyCtrl.enemiesEngaged.GetEnemy(Directions.east).body.width;
        }
        if (armyCtrl.enemiesEngaged.GetEnemy(Directions.west) != null)
        {
            if (armyCtrl.enemiesEngaged.GetEnemy(Directions.west).body.width > _length)
                _length = armyCtrl.enemiesEngaged.GetEnemy(Directions.west).body.width;
        }
        return _length;
    }


    /// <summary> Effective army width considering the enemies engaged  </summary>
    public float GetEffectiveWidth()
    {
        float _width = width;
        if (armyCtrl.enemiesEngaged.GetEnemy(Directions.north) != null)
        {
            if (armyCtrl == armyCtrl.enemiesEngaged.GetEnemy(Directions.north).enemiesEngaged.GetEnemy(Directions.north) ||
                armyCtrl == armyCtrl.enemiesEngaged.GetEnemy(Directions.north).enemiesEngaged.GetEnemy(Directions.south))
            {
                if (armyCtrl.enemiesEngaged.GetEnemy(Directions.north).body.width > width)
                    _width = armyCtrl.enemiesEngaged.GetEnemy(Directions.north).body.width;
            }
            else if (armyCtrl.enemiesEngaged.GetEnemy(Directions.north).body.length > width)
                _width = armyCtrl.enemiesEngaged.GetEnemy(Directions.north).body.length;
        }
        if (armyCtrl.enemiesEngaged.GetEnemy(Directions.south) != null)
        {
            if (armyCtrl.enemiesEngaged.GetEnemy(Directions.south).body.width > _width)
                _width = armyCtrl.enemiesEngaged.GetEnemy(Directions.south).body.width;
        }
        return _width;
    }


    public void MarkAsOverlapped() { GetComponent<Renderer>().material.color = Color.red; }


    public void UnmarkAsOverlapped() { GetComponent<Renderer>().material.color = armyCtrl.player.color; }


    public void MarkAsAttackable(ArmyController enemy, ArmyControllerDlg attackAction, bool drawLines)
    {
        methodBySelection = attackAction;
        this.drawLines = drawLines;
        lineLength = Vector3.Distance(enemy.transform.position, armyCtrl.transform.position);
        markedAsAttackable = true;
    }


    public void UnmarkAsAttackable()
    {
        GetComponent<Renderer>().material.color = armyCtrl.player.color;
        markedAsAttackable = false;
        CancelLines();
    }


    void DrawLinesDuringAttack()
    {
        Vector3 corner1 = armyCtrl.transform.position + (length * armyCtrl.transform.forward + width * armyCtrl.transform.right) / 2.0f;
        Vector3 corner2 = armyCtrl.transform.position + (-length * armyCtrl.transform.forward + width * armyCtrl.transform.right) / 2.0f;
        Vector3 corner3 = armyCtrl.transform.position + (length * armyCtrl.transform.forward - width * armyCtrl.transform.right) / 2.0f;
        Vector3 corner4 = armyCtrl.transform.position + (-length * armyCtrl.transform.forward - width * armyCtrl.transform.right) / 2.0f;

        Vector3 endPoint1 = corner1 + lineLength * (armyCtrl.transform.forward + armyCtrl.transform.right).normalized;
        Vector3 endPoint2 = corner2 + lineLength * (-armyCtrl.transform.forward + armyCtrl.transform.right).normalized;
        Vector3 endPoint3 = corner3 + lineLength * (armyCtrl.transform.forward - armyCtrl.transform.right).normalized;
        Vector3 endPoint4 = corner4 + lineLength * (-armyCtrl.transform.forward - armyCtrl.transform.right).normalized;

        linesDuringAttack.Add(OtherF.DrawLine(corner1, endPoint1, lineMaterial, true));
        linesDuringAttack.Add(OtherF.DrawLine(corner2, endPoint2, lineMaterial, true));
        linesDuringAttack.Add(OtherF.DrawLine(corner3, endPoint3, lineMaterial, true));
        linesDuringAttack.Add(OtherF.DrawLine(corner4, endPoint4, lineMaterial, true));
    }


    void CancelLines()
    {
        foreach (LineRenderer line in linesDuringAttack)
            Destroy(line.gameObject);
        linesDuringAttack.Clear();
    }
}
