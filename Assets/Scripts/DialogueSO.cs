using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueDataSO", order = 1)]
public class DialogueSO : ScriptableObject
{
    public List<Array2D> DialogueSteps;
}

[System.Serializable]
public class Array2D
{
    public ConditionClass[] conditions;
    public int verull;
    public string dialogue;
    public string[] playerOptions;
    public int[] nextDialogueIndex;
}

//a unique option, remember character ID and option id to prevent repearrance of options

[System.Serializable]
public class ConditionClass
{
    public Condition condi;
    public string text;
    public int optionNum;
    public int num;
}

public enum Condition
{
    None,
    Narrator,
    InformationGained,
    InformationReq,
    EnableGrip,
    ExitGrip,
    EndingPoint,
    InformationMiss,
    PlayerState,
    EarlyNarrator,
}