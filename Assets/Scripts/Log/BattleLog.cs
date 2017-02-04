using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleLog {

    //public List<ArmyController> startArmies;
    //public List<ArmyController> endArmies;
    LogPanel panel;
    List<IActionLog> actionsLog = new List<IActionLog> ();


    public BattleLog()
    {
        panel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(3).GetComponent<LogPanel>();
    }

    public void Add(IActionLog action)
    {
        actionsLog.Add(action);
        panel.PrintAction(action);
    }
}
