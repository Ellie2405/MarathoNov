using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Log : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] Color playerColor;
    [SerializeField] Color eventColor;
    [SerializeField] Color narratorColor;
    [SerializeField] string test;
    string endcolor = "</Color>";
    List<LogMessage> textlog = new List<LogMessage>();

    [ContextMenu("boop")]
    public void TestMet()
    {
        AddPlayerMessage(test);
    }

    public void AddMessage(string text)
    {
        textlog.Add(new LogMessage(text, 0));
        UpdateLog();
    }

    public void AddPlayerMessage(string text)
    {
        string t = $"<color=#{playerColor.ToHexString()}>{text}</color>";
        AddMessage(t);
    }

    public void AddEvent(string text)
    {
        string t = $"<color=#{eventColor.ToHexString()}>{text}</color>";
        AddMessage(t);
    }

    public void AddNarrator(string text)
    {
        string t = $"<color=#{narratorColor.ToHexString()}>{text}</color>";
        AddMessage(t);
    }

    void UpdateLog()
    {
        //switch (textlog[textlog.Count - 1].type)
        //{
        //    case 0:
        //        //use strings to change color
        //        break;
        //    case 1:
        //        break;
        //}
        textBox.text += $"\n{textlog[textlog.Count - 1].text}";
    }
}

public struct LogMessage
{
    public readonly string text;
    public readonly byte type;

    public LogMessage(string input, byte messageType)
    {
        text = input;
        type = messageType;
    }
}
