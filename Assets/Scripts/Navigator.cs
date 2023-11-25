using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Navigator : MonoBehaviour, IPointerClickHandler
{
    static public event Action<int> OnNavigatorClicked;

    [SerializeField] int DestinationMapID;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnNavigatorClicked.Invoke(DestinationMapID);
    }
}
