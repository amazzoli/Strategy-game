using UnityEngine;
using System.Collections;


public class ForwardMovement : InGameMovement {

    public ForwardMovement(ArmyController armyCtrl, GameObject movAreaGO, float mov) :
    base(armyCtrl, movAreaGO, mov, 360, 0, 1) {}
}
