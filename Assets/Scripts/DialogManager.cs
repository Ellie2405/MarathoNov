using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] Character InteractedChar;
    [SerializeField] Player player;
    [SerializeField] TextMeshProUGUI characterDialogueBox;
    [SerializeField] GameObject PlayerDialogueOptions;
    [SerializeField] PlayerDialogueOption[] options;
    [SerializeField] TextMeshProUGUI EventText;
    [SerializeField] TextMeshProUGUI NarratorText;

    bool IsWaitingForPlayer = false;
    bool isNarratorQueued = false;
    bool isNarratorSpeaking = false;
    bool isEventQueued = false;
    bool isShowingEventText = false;
    bool areEventsChecked = false;

    private void Awake()
    {
        PlayerDialogueOption.OnOptionClick += pickReply;
    }

    private void Start()
    {
        characterDialogueBox.text = string.Empty;
        EventText.text = string.Empty;
        NarratorText.text = string.Empty;
    }

    public void StepForward()
    {
        if (!areEventsChecked && CheckPrecedingEvents()) //if there is anything that should come before the next chat
        {
            //narrator
            areEventsChecked = true;
            print("yes");
        }

        CheckEventText();
        if (!IsWaitingForPlayer)
        {
            if (InteractedChar.hasSomethingToSay)
                PrintCharacterDialogue();
            else
            {
                if (CheckNarrator()) return;
                GetDialogueOptions();
            }
        }
    }

    //print what the character has to say
    public void PrintCharacterDialogue()
    {
        characterDialogueBox.text = InteractedChar.GetDialogue();
    }

    //print what the player can reply
    public void GetDialogueOptions()
    {
        ParseOptions(InteractedChar.GetAllOptions());
    }

    //read through the reply options and put them into buttons
    void ParseOptions(StringInt[] playerOptions)
    {
        if (playerOptions is null) //if received an empty array, show no options and continue to the next step
        {
            StepForward();
            return;
        }

        int printedOptions = 0;
        int[] fails = GetOptionsWithMissingInfo(); //get a list of options that should be hidden

        for (int i = 0; i < playerOptions.Length; i++) //print viable options by going through all options
        {
            bool isOptionHidden = false;
            if (fails is not null)
            {
                foreach (var item in fails) //go through the list and check if the current option is there
                {
                    if (i == item)
                    {
                        isOptionHidden = true; //skip this option
                        break;
                    }
                }
            }
            if (isOptionHidden) continue;

            if (CanOptionBeUsed(i))
            {
                options[printedOptions].SetOption(playerOptions[i]);
                printedOptions++;
            }
        }

        if (printedOptions == 0) //if nothing was printed due to failing conditions, special event
        {
            //no options to print - print special option?
            print("no options");
            return;
        }

        if (printedOptions < 3) //hide unused buttons
        {
            for (int i = printedOptions; i < 3; i++)
            {
                options[i].Hide();
            }
        }

        IsWaitingForPlayer = true;
    }

    //returns true if there is a preceding event
    bool CheckPrecedingEvents()
    {
        if (InteractedChar.CheckOptionConditions())
        {
            foreach (var item in InteractedChar.GetAllConditions())
            {
                if (item.condi == Condition.Narrator)
                {
                    PrintNarrator(item.text);
                    return true;
                }
            }
        }
        return false;
    }

    //returns true if dialog wasnt used
    private bool CanOptionBeUsed(int index)
    {
        if (InteractedChar.CheckUsedDialog(index))
        {
            print(index + " used");
            return false;
        }
        return true;
    }

    private int[] GetOptionsWithMissingInfo()
    {
        List<int> fails = new();
        if (InteractedChar.CheckOptionConditions())
        {
            foreach (var condition in InteractedChar.GetAllConditions())
            {
                if (condition.condi == Condition.InformationReq)
                {
                    if (!player.CheckIfHasInformation(condition.num))
                        fails.Add(condition.optionNum);
                }
            }
            if (fails.Count > 0)
                return fails.ToArray();
        }
        return default;
    }



    public void pickReply(int index)
    {
        CheckFollowingCondition(index);
        InteractedChar.GiveAnswer(index);
        IsWaitingForPlayer = false;
        areEventsChecked = false;

        HideOptions();
        StepForward();
    }

    private void CheckFollowingCondition(int index)
    {
        if (InteractedChar.CheckOptionConditions())
        {
            foreach (var item in InteractedChar.GetAllConditions())
            {
                if (item.condi == Condition.InformationGained)
                {
                    PrintEventText(item.text);
                    return;
                }
            }
        }
    }

    public void HideOptions()
    {
        for (int i = 0; i < 3; i++)
        {
            options[i].Hide();
        }
    }

    //handle narrator
    public void PrintNarrator(string text)
    {
        isNarratorQueued = true;
        NarratorText.text = "Narrator: " + text;
    }

    private bool CheckNarrator()
    {
        if (isNarratorQueued) //activate narrator if he is queued
        {
            NarratorText.gameObject.SetActive(true);
            isNarratorSpeaking = true;
            isNarratorQueued = false;
            return true;
        }
        if (isNarratorSpeaking) //deactivate the step after
        {
            NarratorText.gameObject.SetActive(false);
            isNarratorSpeaking = false;
        }
        return false;
    }

    //handle events
    public void PrintEventText(string text)
    {
        isEventQueued = true;
        EventText.text = text;
    }

    private void CheckEventText()
    {
        if (isEventQueued)
        {
            EventText.gameObject.SetActive(true);
            isShowingEventText = true;
            isEventQueued = false;
            return;
        }
        if (isShowingEventText)
        {
            EventText.gameObject.SetActive(false);
            isShowingEventText = false;
        }
    }

}
