using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int CurrentDialogTree;
    [SerializeField] DialogueSO[] DialogTrees;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] string characterName;
    [SerializeField] Sprite[] graphic = new Sprite[2];
    bool wasGripped = false;
    bool isGripped = false;
    bool doneTalking = false;
    public bool hasSomethingToSay = true;
    int completeness;
    [SerializeField] int closeness = 0;
    public List<Vector2> usedDialogOptions = new List<Vector2>();


    int currentDialogue = 0;

    //add trigger system
    //some chat options will trigger an event - closeness, information, narrator, etc

    private void Start()
    {
        spriteRenderer.sprite = graphic[0];
    }

    public void SwitchGraphic(int index)
    {
        spriteRenderer.sprite = graphic[index];
    }

    public string GetDialogue()
    {
        hasSomethingToSay = false;
        return DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].dialogue;
    }

    public bool CheckGripCondition()
    {
        return DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].requiresGrip;
    }

    public StringInt[] GetAllOptions()
    {
        hasSomethingToSay = true;
        int optionAmount = DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].options.Length;
        if (optionAmount == 0) //if there are no options, the character can return an empty array to continue talking
        {
            AddUsedDialog();
            currentDialogue++;
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
        return new StringInt(DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].options[index], DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].nextDialogueIndex[index]);
    }

    public void GiveAnswer(int answerNum)
    {
        AddUsedDialog();
        //currentDialogue = DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].nextDialogueIndex[answerNum];
        currentDialogue = answerNum;
    }

    public ConditionClass[] GetAllConditions()
    {
        return DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].conditions;
    }

    //return true if the option has more conditions than index
    public bool CheckOptionConditions(int index)
    {
        return DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].conditions.Length > index;
    }

    //return true if the option has at least 1 condition
    public bool CheckOptionConditions()
    {
        return DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].conditions.Length > 0;
    }

    public ConditionClass GetCondition(int index)
    {
        return DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].conditions[index];
    }

    void AddUsedDialog()
    {
        usedDialogOptions.Add(new Vector2(CurrentDialogTree, currentDialogue));
    }

    //return true if dialog was used
    public bool CheckUsedDialog(int num)
    {
        foreach (var item in usedDialogOptions)
        {
            if (item.x == CurrentDialogTree && item.y == DialogTrees[CurrentDialogTree].playerOptions[currentDialogue].nextDialogueIndex[num])
            {
                return true;
            }
        }
        return false;
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
