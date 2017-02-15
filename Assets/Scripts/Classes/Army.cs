using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Army : CombatUnit
{
    // ARMY VARS AND PROPERTIES

    /// <summary> Number of soldiers in the army </summary>
    public int nSoldiers { get { return soldierList.Count; } }

    /// <summary> Number of wounds per soldier </summary>
    public List<int> soldierList = new List<int>();

    /// <summary> Army class</summary>
    public Classes armyClass;


    // ARMY METHODS

    /// <summary> Initialize the list of the wounds per soldier </summary>
    /// <param name="nSoldiers">Initial number of soldiers</param>
    public void InitalizeSoldierList(int nSoldiers)
    {
        for (int i = 0; i < nSoldiers; i++) { soldierList.Add(woundsNumber); }
    }


    /// <summary>
    /// A certain number of wounds are inflicted to the army.
    /// The wounds are distributited at random among the soldiers.
    /// </summary>
    /// <param name="nWounds">Inflicted wounds</param>
    public void UpdateSoldierList(int nWounds)
    {
        for (int i = 0; i < nWounds; i++)
        {
            int r = Random.Range(0, nSoldiers);
            soldierList[r]--;
            if (soldierList[r] == 0)
                soldierList.RemoveAt(r);
            if (nSoldiers == 0)
                break;
        }
    }
}