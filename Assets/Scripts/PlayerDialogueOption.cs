using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class PlayerDialogueOption : MonoBehaviour, IPointerClickHandler
{
    static public event Action<int> OnOptionClick;

    [SerializeField] TextMeshProUGUI textElement;
    [SerializeField] GameObject graphic;
    [SerializeField] int ID;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnOptionClick.Invoke(ID);
    }

    public void SetOption(StringInt reply)
    {
        if (!graphic.activeInHierarchy) graphic.SetActive(true);
        textElement.text = reply.str;
        ID = reply.i;
    }

    public void Hide()
    {
        graphic.SetActive(false);
    }
}
