using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int CurrentDialogTree = 0;
    [SerializeField] DialogueSO[] DialogTrees;
    [SerializeField] public int id;
    [SerializeField] string characterName;
    [SerializeField] GameObject[] graphic = new GameObject[2];
    int currentGraphic;
    public bool wasGripped = false;
    bool isGripped = false;
    bool doneTalking = false;
    public bool hasSomethingToSay = true;
    int completeness;
    [SerializeField] int closeness = 0;
    public List<Vector2> usedDialogOptions = new List<Vector2>();


    [SerializeField] int currentDialogue = 0;

    //add trigger system
    //some chat options will trigger an event - closeness, information, narrator, etc

    public void SetDialogueTree(int treeIndex)
    {
        CurrentDialogTree = treeIndex;
        hasSomethingToSay = true;
        currentDialogue = 0;
    }

    public void SetDialogueTree(int treeIndex, int chatIndex)
    {
        CurrentDialogTree = treeIndex;
        currentDialogue = chatIndex;

    }

    public bool CheckEnding() 
    {
        return currentDialogue >= DialogTrees[CurrentDialogTree].DialogueSteps.Count;
    }

    public bool PeekEnding() //return true if at an ending point
    {
        foreach (var item in DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].conditions)
        {
            if (item.condi == Condition.EndingPoint)
            {
                return true;
            }
        }
        return false;
    }

    public string GetDialogue()
    {
        hasSomethingToSay = false;
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].dialogue;
    }

    public bool CheckGripCondition()
    {
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].requiresGrip;
    }

    public int PeekOptions()
    {
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].playerOptions.Length;
    }

    public StringInt[] GetAllOptions()
    {
        hasSomethingToSay = true;
        int optionAmount = DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].playerOptions.Length;
        if (optionAmount == 0) //if there are no options, the character can return an empty array to continue talking
        {
            AddUsedDialog();
            currentDialogue++;
            //if (currentDialogue >= DialogTrees[CurrentDialogTree].playerOptions.Count)
            return default;
        }

        StringInt[] options = new StringInt[optionAmount];
        for (int i = 0; i < optionAmount; i++)
        {
            options[i] = GetOption(i);
        }
        return options;
    }

    StringInt GetOption(int index)
    {
        return new StringInt(DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].playerOptions[index], DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].nextDialogueIndex[index]);
    }

    public void GiveAnswer(int answerNum)
    {
        AddUsedDialog();
        //currentDialogue = DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].nextDialogueIndex[answerNum];
        currentDialogue = answerNum;
    }

    public ConditionClass[] GetAllConditions()
    {
        print(currentDialogue);
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].conditions;
    }

    //return true if the option has more conditions than index
    public bool CheckOptionConditions(int index)
    {
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].conditions.Length > index;
    }

    //return true if the option has at least 1 condition
    public bool CheckOptionConditions()
    {
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].conditions.Length > 0;
    }

    public ConditionClass GetCondition(int index)
    {
        return DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].conditions[index];
    }

    void AddUsedDialog()
    {
        if (PeekEnding()) return;
        usedDialogOptions.Add(new Vector2(CurrentDialogTree, currentDialogue));
    }

    //return true if dialog was used
    public bool CheckUsedDialog(int num)
    {
        foreach (var item in usedDialogOptions)
        {
            if (item.x == CurrentDialogTree && item.y == DialogTrees[CurrentDialogTree].DialogueSteps[currentDialogue].nextDialogueIndex[num])
            {
                return true;
            }
        }
        return false;
    }

    public void ApplyGrip()
    {
        wasGripped = true;
        SetDialogueTree(1);
    }
}

public struct StringInt
{
    public string str;
    public int i;

    public StringInt(string x, int y)
    {
        str = x;
        i = y;
    }

}
