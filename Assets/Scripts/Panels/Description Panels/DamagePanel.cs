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
    public Button generateHits, generateWounds;

    // Colors of the hits and wound texts
    public Color woundColor, hitsColor;

    // Lists of the actual instantiated texts
    private List<Text> armyTexts = new List<Text>(), hitsTexts = new List<Text>(), woundTexts = new List<Text>();

    // Lists of the actual instantiated buttons
    private List<Button> hitsButtons = new List<Button>(), woundButtons = new List<Button>();

    

    // Parent panel
    DescPanels descPanels;

    // Space between the rows
    float heightOffset = 5;

    // Stored positions
    float xArmyName, xHits, xWounds, y0, rowHeight;
    

    /// <summary>
    /// Meke the damage panel associated to a damageSkill
    /// </summary>
    public void MakePanel(DamageSkill skill)
    {
        Init(skill.hitsGenerated);
        skillName.text = skill.caster.army.unitName + "'s " + skill.name + " against: ";

        float y = y0;
        for (int i = 0; i < skill.targets.Count; i++)
        {
            InstantiateTexts(skill, y, i);
            InstantiateWoundButt(skill, y, i);
            if (skill.hitsGenerated)
            {
                hitsTexts[i].gameObject.SetActive(true);
                UpdateTextsAfterHits(skill, i);
            }
            else
            {
                InstantiateHitsButt(skill, y, i);
                hitsTexts[i].gameObject.SetActive(false);
                woundButtons[i].GetComponent<UIInfo>().infoText = skill.WoundsInfo(i);
            }
            y += rowHeight + heightOffset;
        }
        SetListeners(skill);
        ResizeComponents(y);
    }


    public void UpdateTextsAfterHits(DamageSkill skill, int i)
    {
        descPanels.damagePanel.hitsTexts[i].text = "Hits on mark: " + skill.nHitsOnMarks[i].ToString();
        descPanels.damagePanel.hitsTexts[i].GetComponent<UIInfo>().infoText = skill.HitsInfo(i);
        descPanels.damagePanel.woundButtons[i].GetComponent<UIInfo>().infoText = skill.WoundsInfo(i);
    }


    public void UpdateTextsAfterWounds(DamageSkill skill, int i)
    {
        descPanels.damagePanel.woundTexts[i].GetComponent<UIInfo>().infoText = skill.WoundsInfo(i);
        descPanels.damagePanel.woundTexts[i].text = "Wounds: " + skill.nWound[i].ToString();
    }


    void Init(bool hitsGenerated)
    {
        if (descPanels == null)
            descPanels = transform.parent.GetComponent<DescPanels>();
        descPanels.SetActive("damage");

        if (xArmyName == 0)
        {
            xArmyName = armyName.GetComponent<RectTransform>().anchoredPosition.x;
            xHits = hitsOnMark.GetComponent<RectTransform>().anchoredPosition.x;
            xWounds = wounds.GetComponent<RectTransform>().anchoredPosition.x;
            y0 = skillName.GetComponent<RectTransform>().rect.height + 2 * heightOffset;
            rowHeight = armyName.GetComponent<RectTransform>().rect.height;
            generateHits.GetComponent<UIInfo>().title = "Generate hits on mark";
            generateWounds.GetComponent<UIInfo>().title = "Generate wounds";
        }

        generateHits.onClick.RemoveAllListeners();
        generateWounds.onClick.RemoveAllListeners();
        for (int i = 0; i < hitsTexts.Count; i++)
        {
            Destroy(hitsTexts[i].gameObject);
            Destroy(woundTexts[i].gameObject);
            Destroy(armyTexts[i].gameObject);
            if (hitsButtons.Count > 0)
                Destroy(hitsButtons[i].gameObject);
            Destroy(woundButtons[i].gameObject);
        }
        hitsTexts.Clear();
        woundTexts.Clear();
        armyTexts.Clear();
        hitsButtons.Clear();
        woundButtons.Clear();

        wounds.gameObject.SetActive(false);
        generateWounds.gameObject.SetActive(true);
        if (hitsGenerated)
            generateHits.gameObject.SetActive(false);
        else
            generateHits.gameObject.SetActive(true);
    }


    void InstantiateTexts(DamageSkill skill, float y, int rowIndex)
    {
        Text newArmyText = Instantiate(armyName, Vector3.zero, Quaternion.identity);
        newArmyText.text = skill.targets[rowIndex].army.unitName;
        newArmyText.transform.SetParent(this.transform);
        newArmyText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xArmyName, -y);
        armyTexts.Add(newArmyText);

        Text newHitsText = Instantiate(hitsOnMark, Vector3.zero, Quaternion.identity);
        newHitsText.text = "Hits on mark: " + skill.hitsGenerated;
        newHitsText.GetComponent<UIInfo>().title = "Hits on " + skill.targets[rowIndex].army.unitName;
        newHitsText.transform.SetParent(this.transform);
        newHitsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xHits, -y);
        newHitsText.color = hitsColor;
        hitsTexts.Add(newHitsText);

        Text newWoundText = Instantiate(wounds, Vector3.zero, Quaternion.identity);
        newWoundText.text = "Wounds: " + skill.nWound;
        newWoundText.GetComponent<UIInfo>().title = "Wounds on " + skill.targets[rowIndex].army.unitName;
        newWoundText.transform.SetParent(this.transform);
        newWoundText.GetComponent<RectTransform>().anchoredPosition = new Vector2(xWounds, -y);
        newWoundText.color = woundColor;
        woundTexts.Add(newWoundText);
        woundTexts[rowIndex].gameObject.SetActive(false);
    }


    void InstantiateWoundButt(DamageSkill skill, float y, int rowIndex)
    {
        Button newWoundButt = Instantiate(generateWounds, Vector3.zero, Quaternion.identity);
        newWoundButt.transform.SetParent(this.transform);
        newWoundButt.GetComponent<RectTransform>().anchoredPosition = new Vector2(xWounds, -y);
        woundButtons.Add(newWoundButt);
    }


    void InstantiateHitsButt(DamageSkill skill, float y, int rowIndex)
    {
        Button newHitsButt = Instantiate(generateHits, Vector3.zero, Quaternion.identity);
        newHitsButt.transform.SetParent(this.transform);
        newHitsButt.GetComponent<RectTransform>().anchoredPosition = new Vector2(xHits, -y);
        hitsButtons.Add(newHitsButt);
        hitsButtons[rowIndex].GetComponent<UIInfo>().infoText = skill.HitsInfo(rowIndex);
    }


    public void SetListeners(DamageSkill skill)
    {
        for (int k = 0; k < hitsButtons.Count; k++)
        {
            hitsButtons[k].onClick.AddListener(() => skill.GenerateHits());
            for (int i = 0; i < hitsButtons.Count; i++)
            {
                int tempI = i;
                hitsButtons[k].onClick.AddListener(() => SwitchButtonWithText(hitsButtons[tempI], hitsTexts[tempI], skill, true));
            } 
        }
        for (int k = 0; k < woundButtons.Count; k++)
        {
            for (int i = 0; i < woundButtons.Count; i++)
            {
                int tempI = i;
                woundButtons[k].onClick.AddListener(() => SwitchButtonWithText(woundButtons[tempI], woundTexts[tempI], skill, false));
            }
            woundButtons[k].onClick.AddListener(() => skill.GenerateWounds());
        }
    }


    void ResizeComponents(float panelHeight)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, panelHeight);
        //float buttonsHeight = panelHeight - y0 - heightOffset;
        //RectTransform hitsButt = generateHits.GetComponent<RectTransform>(), woundsButt = generateWounds.GetComponent<RectTransform>();
        //hitsButt.sizeDelta = new Vector2(hitsButt.sizeDelta.x, buttonsHeight);
        //woundsButt.sizeDelta = new Vector2(woundsButt.sizeDelta.x, buttonsHeight);
    }


    void SwitchButtonWithText(Button button, Text text, DamageSkill skill, bool areHits)
    {
        if (!areHits && !skill.hitsGenerated)
            return;
        button.onClick.RemoveAllListeners();
        button.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
    }
}
