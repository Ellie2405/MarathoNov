using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action<int> OnCompletenessChange;
    int completeness; //starts low, wins when fullm loses when empty
    [SerializeField] PieceSO[] pieces;
    bool[] informationCollection = new bool[4];
    public bool isGripping;


    public void ChangeCompleteness(bool state)
    {
        if (state) completeness++;
        else completeness--;
        OnCompletenessChange.Invoke(completeness);
    }

    public void GainInformation(int infoID)
    {
        informationCollection[infoID] = true;
        if (infoID == 1)
        {
            //GameplayManager.Instance.ImproveBG();
        }
    }

    public bool CheckIfHasInformation(int infoID)
    {
        return informationCollection[infoID];
    }

    public void StartGrip()
    {
        isGripping = true;
    }

    public void EndGrip()
    {
        isGripping = false;
    }
}
