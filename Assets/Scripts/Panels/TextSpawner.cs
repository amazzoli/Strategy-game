using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class TextSpawner : MonoBehaviour
{

    public Text damageText;
    public float totHeight, totTime, spawnDelay;

    float acc, startVelocity;
    float lastSpawnStart;
    //List<Text> activeTexts = new List<Button> ();
    List<Coroutine> activeCoroutines = new List<Coroutine>();


    void Start()
    {
        acc = -2 * totHeight / (totTime * totTime);
        startVelocity = 2 * totHeight / totTime;
    }


    public void SpawnMoraleDamage(float damage, ArmyController army)
    {
        string text = "- " + (damage * 100).ToString("0.#") + "% morale";
        Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, Color.blue, text));
        activeCoroutines.Add(textAnimation);
    }


    public void SpawnWoundDamage(int damage, ArmyController army)
    {
        string text = "- " + damage + " wounds";
        Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, Color.magenta, text));
        activeCoroutines.Add(textAnimation);
    }


    public void SpawnEnergyConsuption(int value, ArmyController army)
    {
        string text = "- " + value + " energy";
        Color orange = new Color(1, 69f / 255f, 0);
        Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, orange, text));
        activeCoroutines.Add(textAnimation);
    }


    public void SpawnMovementReduction(float value, ArmyController army)
    {
        string text = "- " + value.ToString("0.#") + " <i>movement</i>";
        Color orange = new Color(84f / 255f, 40f / 255f, 28f / 255f);
        Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, orange, text));
        activeCoroutines.Add(textAnimation);
    }


    public void SpawnNewStatModifier(StatModifier mod, ArmyController army)
    {
        foreach (StatModText modifier in mod.mods)
        {
            string text = modifier.text;
            Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, Color.black, text));
            activeCoroutines.Add(textAnimation);
        }
    }


    public void SpawnEndStatModifier(StatModifier mod, ArmyController army)
    {
        foreach (StatModText modifier in mod.mods)
        {
            string text = modifier.text;
            if (text[0] == '+')
                text = text.Replace("+", "-");
            else if (text[0] == '-')
                text = text.Replace("-", "+");
            Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, Color.black, text));
            activeCoroutines.Add(textAnimation);
        }
    }


    public void SpawnSoldiersLost(int nSoldiers, ArmyController army)
    {
        string text = "- " + nSoldiers.ToString("0") + " soldiers";
        Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, Color.red, text));
        activeCoroutines.Add(textAnimation);
    }


    public void SpawnArmyText(string text, ArmyController army, Color color)
    {
        Coroutine textAnimation = StartCoroutine(TextAnimation(army.transform.position, color, text));
        activeCoroutines.Add(textAnimation);
    }


    IEnumerator TextAnimation(Vector3 position, Color color, string textStr)
    {
        while ((Time.time - lastSpawnStart) < spawnDelay)
        {
            yield return null;
        }

        Text text = InstantiateText(position, color, textStr);
        Vector2 startPos = new Vector2(text.rectTransform.anchoredPosition.x, text.rectTransform.anchoredPosition.y);
        float t0 = Time.time;
        lastSpawnStart = t0;

        while (true)
        {
            if ((Time.time - t0) < totTime)
                text.rectTransform.anchoredPosition = new Vector2(startPos.x, startPos.y + MotionEquation(Time.time - t0));
            else
                DestroyText(text);
            yield return null;
        }
    }


    Text InstantiateText(Vector3 position, Color color, string textStr)
    {
        Text newText = Instantiate(damageText, Vector3.zero, Quaternion.identity) as Text;
        newText.transform.SetParent(this.transform);
        newText.rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(position);
        newText.text = textStr;
        newText.color = color;
        return newText;
    }


    void DestroyText(Text text)
    {
        StopCoroutine(activeCoroutines[0]);
        activeCoroutines.RemoveAt(0);
        //text.gameObject.SetActive(false);
        Destroy(text.gameObject);
    }


    float MotionEquation(float time)
    {
        return startVelocity * time + acc * time * time / 2f;
    }
}
