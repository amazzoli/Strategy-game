using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Set of enemies engaged in close combat
/// </summary>
public class EnemiesEngaged {

    List<ArmyController> enemiesEngaged;
    List<Directions> directionsEngaged;


    public EnemiesEngaged()
    {
        enemiesEngaged = new List<ArmyController>();
        directionsEngaged = new List<Directions>();
    }


    public List<ArmyController> list
    {
        get
        {
            return enemiesEngaged;
        }
    }


    public void Add(ArmyController enemy, Directions dir)
    {
        if (!directionsEngaged.Contains(dir))
        {
            enemiesEngaged.Add(enemy);
            directionsEngaged.Add(dir);
        }
        else
            Debug.Log("Side already engaged!");
    }


    public void Remove(ArmyController enemy)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (enemiesEngaged[i] == enemy)
            {
                enemiesEngaged.RemoveAt(i);
                directionsEngaged.RemoveAt(i);
            }
        }
    }


    public Directions GetDirection(ArmyController enemy)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (enemy == list[i])
                return directionsEngaged[i];
        }
        return Directions.none;
    }


    public ArmyController GetEnemy(Directions dir)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (dir == directionsEngaged[i])
                return list[i];
        }
        return null;
    }


    public List<Directions> freeSides
    {
        get
        {
            List<Directions> sides = new List<Directions>();
            if (!directionsEngaged.Contains(Directions.north))
                sides.Add(Directions.north);
            if (!directionsEngaged.Contains(Directions.east))
                sides.Add(Directions.east);
            if (!directionsEngaged.Contains(Directions.south))
                sides.Add(Directions.south);
            if (!directionsEngaged.Contains(Directions.west))
                sides.Add(Directions.west);
            return sides;
        }
    }


    public void DebugPrint()
    {
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i].army.unitName + " " + directionsEngaged[i]);
        }
    }

}
