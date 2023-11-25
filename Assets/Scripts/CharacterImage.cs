using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class CharacterImage : MonoBehaviour, IPointerClickHandler
{
    public static event Action<int> OnImageClicked;

    [SerializeField] int CharacterID;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnImageClicked?.Invoke(CharacterID);
    }
}
