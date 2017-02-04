using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LogPanel : MonoBehaviour
{

    public Text showButtonText;
    public Text actionText;
    public RectTransform content;
    public Scrollbar bar;
    public float heightOffset;

    float height;
    float lastCrtStart = 0;
    List<Coroutine> activeCoroutines = new List<Coroutine>();

    void Start()
    {
        height = 0;
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(content.sizeDelta.x, 0);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
        showButtonText.text = "Show log";
    }


    public void PrintAction(IActionLog action)
    {
        
        Coroutine crt = StartCoroutine(PrintCoroutine(action));
        activeCoroutines.Add(crt);
    }


    public void ShowHidePanel()
    {
        if (GetComponent<RectTransform>().anchoredPosition.y == 0)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
            showButtonText.text = "Show log";
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            showButtonText.text = "Hide log";
        }
    }


    IEnumerator PrintCoroutine(IActionLog action)
    {
        while ((Time.time - lastCrtStart) < 2 * Time.deltaTime)
            yield return null;

        lastCrtStart = Time.time;

        RectTransform newText = Instantiate(actionText.gameObject, Vector3.zero, Quaternion.identity).GetComponent<RectTransform>();
        newText.transform.SetParent(content.transform);
        newText.anchoredPosition = new Vector2(8, height + 8);
        newText.sizeDelta = new Vector2(-16, 0);
        newText.GetComponent<Text>().text = action.actionText;
        //if (action.actionName != "None")
        //{
        //    newText.gameObject.AddComponent<UIInfo>();
        //    newText.GetComponent<UIInfo>().title = action.actionName;
        //    newText.GetComponent<UIInfo>().infoText = action.actionDescription;
        //}


        yield return new WaitForSeconds(Time.deltaTime);
        float myHeight = newText.sizeDelta.y;
        height += myHeight + heightOffset;
        content.sizeDelta = new Vector2(content.sizeDelta.x, height + 16 - heightOffset);
        StopPrinting();
    }


    void StopPrinting()
    {
        StopCoroutine(activeCoroutines[0]);
        activeCoroutines.RemoveAt(0);
    }
}
