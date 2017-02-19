using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel for the menagement of a DamageSkill (both single and multi targets)
/// </summary>
public class DamagePanel : MonoBehaviour {

    // Texts set in the inspector, they are template for instantiated texts
    public Text skillName, armyName, hitsOnMark, wounds;

    // Buttons set in the inspector for the generation of the hits and the wounds
    public Button generateHitsWounds;

    // Colors of the hits and wound texts
    public Color woundColor, hitsColor;

    // Lists of the actual instantiated texts
    List<Text> armyTexts = new List<Text>(), hitsTexts = new List<Text>(), woundTexts = new List<Text>();

    // Lists of the actual instantiated buttons
    List<Button> hitsWoundsButtons = new List<Button>();

    // Parent panel
    DescPanels descPanels;

    // Space between the rows
    float heightOffset = 2;

    // Stored positions
    float xArmyName, xHits, xWounds, y0, rowHeight, xButton;
    

    /// <summary>
    /// Meke the damage panel associated to a damageSkill
    /// </summary>
    public void MakePanel(DamageSkill skill)
    {
        Init();
        skillName.text = skill.caster.army.unitName + "'s " + skill.name + " against: ";

        float y = y0;
        for (int i = 0; i < skill.targets.Count; i++)
        {
            InstantiateTextsButtons(skill, y, i);  
            y += rowHeight + heightOffset;
        }
        SetListeners(skill);
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, y);
    }


    public void UpdateTextsAfterWounds(DamageSkill skill, int i)
    {
        hitsTexts[i].text = "Hits on mark: " + skill.nHitsOnMarks[i].ToString();
        woundTexts[i].text = "Wounds: " + skill.nWound[i].ToString();
        descPanels.damagePanel.hitsTexts[i].GetComponent<UIInfo>().infoText = skill.HitsInfo(i);
        descPanels.damagePanel.woundTexts[i].GetComponent<UIInfo>().infoText = skill.WoundsInfo(i);
    }


    void Init()
    {
        if (descPanels == null)
            descPanels = transform.parent.GetComponent<DescPanels>();
        descPanels.SetActive("damage");

        if (y0 == 0)
        {
            xArmyName = armyName.GetComponent<RectTransform>().anchoredPosition.x;
            xHits = hitsOnMark.GetComponent<RectTransform>().anchoredPosition.x;
            xWounds = wounds.GetComponent<RectTransform>().anchoredPosition.x;
            xButton = generateHitsWounds.GetComponent<RectTransform>().anchoredPosition.x;
            y0 = skillName.GetComponent<RectTransform>().rect.height + heightOffset - skillName.GetComponent<RectTransform>().anchoredPosition.y;
            rowHeight = armyName.GetComponent<RectTransform>().rect.height;
            generateHitsWounds.GetComponent<UIInfo>().title = "Generate hits and wounds";
        }

        generateHitsWounds.onClick.RemoveAllListeners();
        for (int i = 0; i < hitsTexts.Count; i++)
        {
            Destroy(hitsTexts[i].gameObject);
            Destroy(woundTexts[i].gameObject);
            Destroy(armyTexts[i].gameObject);
            Destroy(hitsWoundsButtons[i].gameObject);
        }
        hitsTexts.Clear();
        woundTexts.Clear();
        armyTexts.Clear();
        hitsWoundsButtons.Clear();
    }


    void InstantiateTextsButtons(DamageSkill skill, float y, int rowIndex)
    {
        Text newArmyText = Instantiate(armyName, Vector3.zero, Quaternion.identity);
        newArmyText.text = skill.targets[rowIndex].army.unitName;
        newArmyText.transform.SetParent(this.transform);
        newArmyText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xArmyName, -y);
        armyTexts.Add(newArmyText);

        Text newHitsText = Instantiate(hitsOnMark, Vector3.zero, Quaternion.identity);
        newHitsText.GetComponent<UIInfo>().title = "Hits on " + skill.targets[rowIndex].army.unitName;
        newHitsText.transform.SetParent(this.transform);
        newHitsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xHits, -y);
        newHitsText.color = hitsColor;
        hitsTexts.Add(newHitsText);
        hitsTexts[rowIndex].gameObject.SetActive(false);

        Text newWoundText = Instantiate(wounds, Vector3.zero, Quaternion.identity);
        newWoundText.GetComponent<UIInfo>().title = "Wounds on " + skill.targets[rowIndex].army.unitName;
        newWoundText.transform.SetParent(this.transform);
        newWoundText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xWounds, -y);
        newWoundText.color = woundColor;
        woundTexts.Add(newWoundText);
        woundTexts[rowIndex].gameObject.SetActive(false);

        Button newWoundButt = Instantiate(generateHitsWounds, Vector3.zero, Quaternion.identity);
        newWoundButt.transform.SetParent(this.transform);
        newWoundButt.GetComponent<RectTransform>().anchoredPosition = new Vector2(xButton, -y);
        newWoundButt.GetComponent<UIInfo>().title = "Generation of hits and wounds on " + skill.targets[rowIndex].army.unitName;
        newWoundButt.GetComponent<UIInfo>().infoText = skill.InfoBeforeGeneration(rowIndex);
        hitsWoundsButtons.Add(newWoundButt);
    }


    public void SetListeners(DamageSkill skill)
    {
        for (int k = 0; k < hitsWoundsButtons.Count; k++)
        {
            hitsWoundsButtons[k].onClick.AddListener(() => skill.GenerateHitsAndWounds());
            for (int i = 0; i < hitsWoundsButtons.Count; i++)
            {
                int tempI = i;
                hitsWoundsButtons[k].onClick.AddListener(() => SwitchButtonWithText(hitsWoundsButtons[tempI], hitsTexts[tempI], woundTexts[tempI], skill));
            } 
        }
    }


    void SwitchButtonWithText(Button button, Text hitsText, Text woundText, DamageSkill skill)
    {
        button.onClick.RemoveAllListeners();
        button.gameObject.SetActive(false);
        hitsText.gameObject.SetActive(true);
        woundText.gameObject.SetActive(true);
    }
}
