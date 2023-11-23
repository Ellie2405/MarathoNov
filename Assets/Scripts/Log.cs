using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    List<string> textlog = new List<string>();

    public void AddToLog(string text)
    {
        textlog.Add(text);
    }

}
