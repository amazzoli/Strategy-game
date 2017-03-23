using UnityEngine;
using System.Collections.Generic;


public static class OtherF
{

    private static string GetHex(int decimalInt)
    {
        string alpha = "0123456789ABCDEF";
        string hex = "" + alpha[decimalInt];
        return hex;
    }


    public static string RGBToHex(Color color)
    {
        float red = color.r * 255;
        float green = color.g * 255;
        float blue = color.b * 255;

        string a = GetHex((int)Mathf.Floor(red / 16));
        string b = GetHex((int)Mathf.Round(red % 16));
        string c = GetHex((int)Mathf.Floor(green / 16));
        string d = GetHex((int)Mathf.Round(green % 16));
        string e = GetHex((int)Mathf.Floor(blue / 16));
        string f = GetHex((int)Mathf.Round(blue % 16));

        string z = a + b + c + d + e + f;
        return z;
    }


    public static LineRenderer DrawLine(Vector3 start, Vector3 end, Material lineMaterial, bool areaLayer)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = lineMaterial;
        //lr.startColor = color;
        //lr.endColor = color;
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        if (areaLayer)
            myLine.layer = 9;
        return lr;
    }
}

