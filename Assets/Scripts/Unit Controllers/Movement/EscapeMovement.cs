using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EscapeMovement : InGameMovement
{

    ArmyController myCtrl;
    List<GameObject> battleSymblesOnMe = new List<GameObject>();


    public EscapeMovement(ArmyController armyCtrl, GameObject movAreaGO, float mov, Directions dir) :
    base(armyCtrl, movAreaGO, mov, 90, BattleF.dirToAngle[dir], 2)
    {
        myCtrl = armyT.GetComponent<ArmyController>();
    }


    public override void InitMovement()
    {
        battleSymblesOnMe.Clear();
        if (myCtrl.battleSymbol.activeSelf)
        {
            myCtrl.battleSymbol.SetActive(false);
            battleSymblesOnMe.Add(myCtrl.battleSymbol);
        }
        foreach (ArmyController enemy in myCtrl.enemiesEngaged.list)
        {
            if (enemy.enemiesEngaged.GetDirection(myCtrl) == Directions.north && enemy.battleSymbol.activeSelf)
            {
                enemy.battleSymbol.SetActive(false);
                battleSymblesOnMe.Add(enemy.battleSymbol);
            }

        }

        base.InitMovement();
    }


    public override void StopMovement()
    {
        List<ArmyController> enemyList = new List<ArmyController> ();
        foreach (ArmyController enemy in myCtrl.enemiesEngaged.list)
            enemyList.Add(enemy);
        foreach (ArmyController enemy in enemyList)
        {
            enemy.Disengage(myCtrl);
            myCtrl.Disengage(enemy);            
        }
        base.StopMovement();
    }


    public override void ResetMovement()
    {
        foreach (GameObject symbol in battleSymblesOnMe)
            symbol.SetActive(true);
        base.ResetMovement();
    }
}
