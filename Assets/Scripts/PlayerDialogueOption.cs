using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class PlayerDialogueOption : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    static public event Action<int, int> OnOptionClick;

    [SerializeField] TextMeshProUGUI textElement;
    [SerializeField] GameObject graphic;
    [SerializeField] Transform arrow;
    [SerializeField] GameObject hover;
    [SerializeField] int ID;
    [SerializeField] int optionPointer;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sounds.OptionPick);
        GameplayManager.Instance.LogPlayer(textElement.text);
        OnOptionClick.Invoke(ID, optionPointer);
    }

    public void SetOption(StringInt reply)
    {
        if (!graphic.activeInHierarchy) graphic.SetActive(true);
        textElement.text = reply.str;
        optionPointer = reply.i;
    }

    public void Hide()
    {
        graphic.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sounds.OptionHover);
        hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover.SetActive(false);
    }
}
