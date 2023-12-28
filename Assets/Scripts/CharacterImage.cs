using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterImage : MonoBehaviour//, IPointerClickHandler
{
    public static event Action<int> OnImageClicked;

    [SerializeField] int CharacterID;
    [SerializeField] Button button;

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    OnImageClicked?.Invoke(CharacterID);
    //}

    private void OnEnable()
    {
        if (true)
        {
            button.interactable = true;
        }
    }

    public void BeginInteraction()
    {
        OnImageClicked?.Invoke(CharacterID);

    }
}
