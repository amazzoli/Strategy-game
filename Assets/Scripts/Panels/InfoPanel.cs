using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public float xOffset, textYSpace, width;
	public Text title, infoText;
	//Transform canvas;


	void Start () {
        Activate(false);
        //gameObject.SetActive (false);
        //canvas = GameObject.FindGameObjectWithTag ("Canvas").transform;
    }


    /// <summary>
    /// Makes the information panel, which shows information of a UI rectTransform
    /// </summary>
    /// <param name="describedPanel">Described RectTransform.</param>
    /// <param name="title">Title.</param>
    /// <param name="infoText">Info text.</param>
    /// <param name="onTheRight">If set to <c>true</c> the panel is shown on the right of the element.</param>
    /// <param name="topDown">If set to <c>true</c> the panel is shown from the top of the element towards the screen bottom.
    /// otherwise bottom-up</param>
    public void MakePanel(RectTransform describedPanel, string title, string infoText, bool onTheRight, bool topDown)
    {
        Activate(true);

        float x, y;
        if (onTheRight)
        {
            x = describedPanel.position.x + (1 - describedPanel.pivot.x) * describedPanel.rect.width + xOffset;
            GetComponent<RectTransform>().pivot = new Vector2(0, GetComponent<RectTransform>().pivot.y);
        }
        else
        {
            x = describedPanel.position.x - describedPanel.pivot.x * describedPanel.rect.width - xOffset;
            GetComponent<RectTransform>().pivot = new Vector2(1, GetComponent<RectTransform>().pivot.y);
        }
        if (topDown)
        {
            y = describedPanel.position.y + (1 - describedPanel.pivot.y) * describedPanel.rect.height;
            GetComponent<RectTransform>().pivot = new Vector2(GetComponent<RectTransform>().pivot.x, 1);
        }
        else
        {
            y = describedPanel.position.y - describedPanel.pivot.y * describedPanel.rect.height;
            GetComponent<RectTransform>().pivot = new Vector2(GetComponent<RectTransform>().pivot.x, 0);
        }
        transform.position = new Vector3(x, y, 0);
        this.title.text = title;
        this.infoText.text = infoText;
    }


    public void HidePanel () {
        Activate(false);
	}


    void Activate(bool activate)
    {
        this.title.enabled = activate;
        this.infoText.enabled = activate;
        GetComponent<Image>().enabled = activate;
    }
	
}
