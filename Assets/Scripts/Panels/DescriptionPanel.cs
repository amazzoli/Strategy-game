using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class DescriptionPanel : MonoBehaviour
{

    public delegate void DirectionDlg(Directions dir);

    public Button armyButton, directionButton;
    public List<GameObject> descriptionGO;
    public List<Text> myTexts = new List<Text>();

    List<Button> myButtons = new List<Button>(), instantButton = new List<Button>();
    //	bool allowed;


    void Start()
    {
        SelectionController.ArmyDeselection += HidePanel;
        gameObject.SetActive(false);
    }


    //	public override void MakePanel (ArmyController armyCtrl) {
    //		if (armyCtrl == GameController.currentArmy && GameController.currentFase == GameFase.battle) {
    //			allowed = true;
    //		} else
    //			allowed = false;
    //	}


    public void SetMovementPanel(float movement)
    {
        InitPanel(0);
        myTexts[0].text = "Straight movement done = 0.0";
        myTexts[1].text = "Rotation movement done = 0.0";
        myTexts[2].text = "Total movement done = 0.0";
        myTexts[3].text = "Movement left = " + movement.ToString("0.0");
    }


    public void SetArmyChioce(List<ArmyController> armies, ArmyControllerDlg calledMethod)
    {
        InitPanel(1);
        float buttonHeight = armyButton.GetComponent<RectTransform>().sizeDelta.y;
        float titleHeight = 20;

        for (int i = 0; i < armies.Count; i++)
        {
            Button newButton = AddArmyButton(armies[i], 0, -titleHeight - 10 - i * (buttonHeight + 5));
            int tempIndex = i;
            newButton.onClick.AddListener(() => calledMethod(armies[tempIndex]));
            instantButton.Add(newButton);
        }
        if (armies.Count == 0)
            myTexts[0].text = "No enemies in sight";
        else
            myTexts[0].text = "Choose the target: ";
        float textWidth = myTexts[0].GetComponent<RectTransform>().sizeDelta.x;
        myTexts[0].GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, titleHeight);
        RectTransform panel = descriptionGO[1].GetComponent<RectTransform>();
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, titleHeight + 10 + armies.Count * (buttonHeight + 5));
    }


    public void SetDirectionChioce(List<Directions> allowedDirections, DirectionDlg calledMethod, string title, int titleNLines)
    {
        InitPanel(1);
        float buttonHeight = directionButton.GetComponent<RectTransform>().sizeDelta.y;
        float titleHeight = titleNLines * 15 + 5;

        for (int i = 0; i < allowedDirections.Count; i++)
        {
            Button newButton = AddButton(directionButton, 0, -titleHeight - 10 - i * (buttonHeight + 5), BattleF.dirToString[allowedDirections[i]]);
            int tempIndex = i;
            newButton.onClick.AddListener(() => calledMethod(allowedDirections[tempIndex]));
            instantButton.Add(newButton);
        }
        myTexts[0].text = title;
        float textWidth = myTexts[0].GetComponent<RectTransform>().sizeDelta.x;
        myTexts[0].GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, titleHeight);
        RectTransform panel = descriptionGO[1].GetComponent<RectTransform>();
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, titleHeight + 10 + allowedDirections.Count * (buttonHeight + 5));
    }


    public void SetDamagePanel(OneTargetDamageSkill skill, bool hitsGenerated, bool damageAlreadyGenerated, string enemyName)
    {
        InitPanel(2);
        myTexts[0].text = skill.name + " on " + enemyName;
        myTexts[1].text = "N hits: " + skill.nHits;
        myTexts[2].text = "Precision: " + skill.precision.ToString("0.00");
        myTexts[3].text = "N hits on mark: " + skill.nHitsOnMarks;
        myTexts[4].text = "Attack strength: " + skill.attack;
        myTexts[5].text = "Physical defense: " + skill.defense;
        myTexts[6].text = "Damage probability: " + skill.damageProb.ToString("0.00");
        myTexts[7].text = "N wounds: " + skill.nWound;
        myTexts[7].color = Color.red;
        if (!hitsGenerated)
        {
            myTexts[3].gameObject.SetActive(false);
            myTexts[4].gameObject.SetActive(false);
            myTexts[5].gameObject.SetActive(false);
            myTexts[6].gameObject.SetActive(false);
            myTexts[7].gameObject.SetActive(false);
            myButtons[0].gameObject.SetActive(true);
            myButtons[1].gameObject.SetActive(false);
            myButtons[0].onClick.AddListener(() => skill.GenerateHits());
            myButtons[0].onClick.AddListener(() => SwitchButtonWithText(myButtons[0], myTexts[3], "N hits on mark: " + skill.nHitsOnMarks));
        }
        else if (!damageAlreadyGenerated)
        {
            myTexts[3].gameObject.SetActive(true);
            myTexts[4].gameObject.SetActive(true);
            myTexts[5].gameObject.SetActive(true);
            myTexts[6].gameObject.SetActive(true);
            myTexts[7].gameObject.SetActive(false);
            myButtons[0].gameObject.SetActive(false);
            myButtons[1].gameObject.SetActive(true);
            myButtons[1].onClick.AddListener(() => skill.GenerateWounds());
            myButtons[1].onClick.AddListener(() => SwitchButtonWithText(myButtons[1], myTexts[7], "N wounds: " + skill.nWound));
        }
        else
        {
            myTexts[3].gameObject.SetActive(true);
            myTexts[7].gameObject.SetActive(true);
            myButtons[0].gameObject.SetActive(false);
            myButtons[1].gameObject.SetActive(false);
        }
    }


    public void HidePanel(ArmyController armyCtrl)
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }


    void InitPanel(int i)
    {
        ClearPanel();
        foreach (Transform child in descriptionGO[i].transform)
            if (child.tag == "Button")
                myButtons.Add(child.GetComponent<Button>());
            else
                myTexts.Add(child.GetComponent<Text>());

        //base.MakePanel (GameController.currentArmy);
        descriptionGO[i].gameObject.SetActive(true);
        gameObject.SetActive(true);
    }


    Button AddArmyButton(ArmyController armyCtrl, float x, float y)
    {
        Button newButton = AddButton(armyButton, x, y, armyCtrl.army.unitName);
        SetButtonColors(newButton, armyCtrl);
        newButton.GetComponentInChildren<Text>().text = armyCtrl.army.unitName;
        newButton.transform.GetChild(1).GetComponent<Image>().overrideSprite = armyCtrl.spriteSymbol;
        return newButton;
    }


    Button AddButton(Button button, float x, float y, string text)
    {
        Button newButton = (Button)Instantiate(button, Vector3.zero, Quaternion.identity);
        RectTransform rTrans = newButton.GetComponent<RectTransform>();
        newButton.transform.SetParent(transform);
        rTrans.anchoredPosition = new Vector2(x, y);
        newButton.GetComponentInChildren<Text>().text = text;
        return newButton;
    }


    void ClearPanel()
    {
        foreach (GameObject go in descriptionGO)
            go.SetActive(false);
        foreach (Button butt in myButtons)
        {
            butt.onClick.RemoveAllListeners();
            butt.gameObject.SetActive(false);
        }
        foreach (Button butt in instantButton)
        {
            butt.onClick.RemoveAllListeners();
            Destroy(butt.gameObject);
        }
        myButtons.Clear();
        instantButton.Clear();
        myTexts.Clear();
    }


    void SetButtonColors(Button newButton, ArmyController armyCtrl)
    {
        Color color = armyCtrl.player.color;
        ColorBlock cb = newButton.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.disabledColor = color;
        color.a = 0.7f;
        cb.pressedColor = color;
        newButton.colors = cb;
        Color textColor = Color.black;
        if (color.b + color.g + color.r < 1.5f) textColor = Color.white;
        newButton.transform.GetChild(0).GetComponent<Text>().color = textColor;
    }


    void SwitchButtonWithText(Button button, Text text, string newText)
    {
        button.onClick.RemoveAllListeners();
        button.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
        text.text = newText;
    }
}
