using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ActionPanel : UnitPanel
{

    // Empty list of texts
    public float offsetBtwButtons, topSpace;
    public Button buttonGO;

    //DescriptionPanel descPanel;
    float buttonHeigth;
    List<Button> currentButtons = new List<Button>();


    protected override void Awake()
    {
        buttonHeigth = buttonGO.GetComponent<RectTransform>().sizeDelta.y;
        //descPanel = transform.parent.FindChild ("Description Panel").GetComponent<DescriptionPanel> ();
        base.Awake();
    }


    public override void MakePanel(ArmyController armyCtrl)
    {
        if (armyCtrl == GameController.currentArmy && GameController.currentFase == GameFase.battle)
        {
            base.MakePanel(armyCtrl);
            SetButtons(armyCtrl, !armyCtrl.inCombat);
        }
    }


    void SetButtons(ArmyController armyCtrl, bool outOfCombat)
    {
        DeleteCurrentButtons();
        int buttonCount = 0;

        if (outOfCombat)
            buttonCount += SetStandardOutOfCombatButtons(armyCtrl);
        else
            buttonCount += SetStandardInCombatButtons(armyCtrl);

        foreach (ActiveSkill skill in armyCtrl.army.skills)
        {
            if ((skill.atDistance && outOfCombat) || (skill.inCombat && !outOfCombat))
            {
                Button newButton = InstantiateButton(buttonCount, skill.name);
                newButton.onClick.AddListener(() => skill.InitSkill(armyCtrl));
                newButton.GetComponent<UIInfo>().title = skill.name;
                newButton.GetComponent<UIInfo>().infoText = skill.description;
                currentButtons.Add(newButton);
                buttonCount++;
            }
        }

        float y = buttonCount * (buttonHeigth + offsetBtwButtons) + topSpace;
        myTrans.sizeDelta = new Vector2(myTrans.sizeDelta.x, y);
    }


    Button InstantiateButton(int count, string text)
    {
        Button newButton = (Button)Instantiate(buttonGO, Vector3.zero, Quaternion.identity);
        newButton.transform.SetParent(transform);
        float y = -topSpace - count * (offsetBtwButtons + buttonHeigth);
        newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, y);
        newButton.transform.GetChild(0).GetComponent<Text>().text = text;
        return newButton;
    }


    void DeleteCurrentButtons()
    {
        foreach (Button butt in currentButtons)
        {
            butt.onClick.RemoveAllListeners();
            Destroy(butt.gameObject);
        }
        currentButtons.Clear();
    }


    int SetStandardOutOfCombatButtons(ArmyController armyCtrl)
    {

        // Movement
        MovementController movCtrl = armyCtrl.transform.GetChild(0).GetComponent<MovementController>();
        Button newButton = InstantiateButton(0, "Move");
        newButton.onClick.AddListener(() => movCtrl.StartInGameMovement(false));
        newButton.GetComponent<UIInfo>().title = "Forward Movement";
        string infoText = "Move the army in a new position. The army can cover a distance whose maximum value is given by the <i><b>";
        infoText += "movement</b></i> stat.\n<b>" + movCtrl.movementLeft.ToString("0.#") + "</b> movement left in the current turn.";
        newButton.GetComponent<UIInfo>().infoText = infoText;
        currentButtons.Add(newButton);

        // Backward movement
        newButton = InstantiateButton(1, "Move Back");
        newButton.onClick.AddListener(() => movCtrl.StartInGameMovement(true));
        newButton.GetComponent<UIInfo>().title = "Backward Movement";
        newButton.GetComponent<UIInfo>().infoText = "Move the army going backward. The movement is consumed two time faster.\n";
        newButton.GetComponent<UIInfo>().infoText += "<b>" + movCtrl.movementLeft.ToString("0.#") + "</b> movement left.";
        currentButtons.Add(newButton);

        // Charge / action
        if (movCtrl.canCharge)
        {
            newButton = InstantiateButton(2, "Charge");
            newButton.GetComponent<UIInfo>().title = "Charge";
            infoText = "Charge an enemy in sight. The charge action can be performed only if the army has not already moved ";
            infoText += "in the current turn. The <b><i>charge</i></b> causes a strength <b>" + BattleF.moraleDmgCharge.ToString("0.#");
            infoText += "</b> <i><b><color=blue>morale damage</color></b></i> to the charged enemy, and increases the attacker ";
            infoText += "<i><b>physical attack</b></i> by <b>1</b> for one round. The charge causes further <i><b><color=blue>";
            infoText += "morale damage</color></b></i> for a <b><i>lateral attack</i></b> (strength <b>" + BattleF.moraleDmgLateralAttack.ToString("0.#");
            infoText += "</b> damage) or a <b><i>back attack</i></b> (strength <b>" + BattleF.moraleDmgBackAttack.ToString("0.#") + "</b>)";
            newButton.GetComponent<UIInfo>().infoText = infoText;
        }
        else
        {
            newButton = InstantiateButton(2, "Attack");
            newButton.GetComponent<UIInfo>().title = "Attack";
            infoText = "Attack an enemy in sigth without charging. The attack may cause a strength <b>" + BattleF.moraleDmgLateralAttack.ToString("0.#");
            infoText += "</b> <i><b><color=blue>morale damage</color></b></i> for a <b><i>lateral attack</i></b> or a strength <b>";
            infoText += BattleF.moraleDmgBackAttack.ToString("0.#") + "</b> <i><b><color=blue>morale damage</color></b></i> for a <b><i>back attack</i></b>.";
            newButton.GetComponent<UIInfo>().infoText = infoText;
        }
        newButton.onClick.AddListener(() => armyCtrl.GetComponent<AttackController>().StartAttack(movCtrl.canCharge));
        currentButtons.Add(newButton);

        return 3;
    }

    int SetStandardInCombatButtons(ArmyController armyCtrl)
    {
        // Escape
        MovementController movCtrl = armyCtrl.transform.GetChild(0).GetComponent<MovementController>();
        Button newButton = InstantiateButton(0, "Escape");
        newButton.onClick.AddListener(() => movCtrl.ChooseEscapeDirection());
        newButton.GetComponent<UIInfo>().title = "Escape";
        string infoText = "Escape from the melee combat. The unit can move away from the enemy with a double consuption its ";
        infoText += "<i><b>movement</b></i>.\n<b>" + movCtrl.movementLeft.ToString("0.#") + "</b> movement left in the current turn.";
        newButton.GetComponent<UIInfo>().infoText = infoText;
        currentButtons.Add(newButton);

        return 1;
    }
}