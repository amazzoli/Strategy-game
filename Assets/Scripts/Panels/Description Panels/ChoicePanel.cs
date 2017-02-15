using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour {

    DescPanels descPanels;
    List<Button> myButtons = new List<Button>();

    public float heightOffset;
    public Text title;
    public Button choiceButton;
    public delegate void OneArgMethod<T>(T arg);
    public delegate string ArgToString<T>(T arg);


    void Awake()
    {
        descPanels = transform.parent.GetComponent<DescPanels>();
        //gameObject.SetActive(false);
    }


    public void MakePanel(List<Directions> possibleDirections, OneArgMethod<Directions> calledMethod, string title)
    {
        if (possibleDirections.Count == 0)
            title = "No directions available";
        MakePanel<Directions>(possibleDirections, calledMethod, dir => dir.ToString(), title);
    }


    public void MakePanel(List<ArmyController> possibleArmies, OneArgMethod<ArmyController> calledMethod)
    {
        string title = "Choose the target: ";
        if (possibleArmies.Count == 0)
            title = "No armies in sight";
        MakePanel<ArmyController>(possibleArmies, calledMethod, army => army.army.unitName, title);
        for (int i = 0; i < possibleArmies.Count; i++)
            PanelF.SetButtonColors(myButtons[i], possibleArmies[i]);
    }


    public void MakePanel<T>(List<T> possibleItems, OneArgMethod<T> calledMethod, ArgToString<T> argToString, string title)
    {
        if (descPanels == null)
            descPanels = transform.parent.GetComponent<DescPanels>();
        foreach (Button button in myButtons)
            Destroy(button.gameObject);
        myButtons.Clear();
        descPanels.SetActive("choice");

        this.title.text = title;
        for (int i=0; i<possibleItems.Count; i++)
        {
            Button newButton = AddButton(argToString(possibleItems[i]));
            int tempIndex = i;
            newButton.onClick.AddListener(() => calledMethod(possibleItems[tempIndex]));
        }
        Invoke("SetHeights", 2 * Time.deltaTime);
    }


    public void HidePanel() { transform.parent.GetComponent<DescPanels>().HidePanel(null); }


    Button AddButton(string text)
    {
        Button newButton = (Button)Instantiate(choiceButton, Vector3.zero, Quaternion.identity);
        newButton.transform.SetParent(transform);
        newButton.GetComponentInChildren<Text>().text = text;
        myButtons.Add(newButton);
        return newButton;
    }


    void SetHeights()
    {
        float y = title.GetComponent<RectTransform>().rect.height + 2 * heightOffset;
        foreach (Button button in myButtons)
        {
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -y);
            y += button.GetComponent<RectTransform>().rect.height + heightOffset;
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, y);
    }
}
