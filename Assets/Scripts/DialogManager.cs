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
    [SerializeField] TypeWritterVFX typeWriter;
    string earlyNarratorText;

    bool IsWaitingForPlayer = false;
    bool isNarratorQueued = false;
    bool isNarratorSpeaking = false;
    bool isEarlyNarratorQueued = false;
    bool isEarlyNarratorSpeaking = false;
    bool isEventQueued = false;
    bool isShowingEventText = false;
    bool areEventsChecked = false;
    bool isAtEnding = false;
    bool isTypeWriterReady = true;
    bool isGripQueued = false;

    private void Awake()
    {
        PlayerDialogueOption.OnOptionClick += PickReply;
        TypeWritterVFX.CompleteTextRevealed += TextRevealed;
    }

    private void Start()
    {
        characterDialogueBox.text = string.Empty;
        EventText.text = string.Empty;
        NarratorText.text = string.Empty;
    }

    public void ResetSettings()
    {
        isAtEnding = false;
        IsWaitingForPlayer = false;
        HideOptions();
    }

    public void StepForward()
    {
        if (!isTypeWriterReady) { print("typewriter not ready"); return; }
        if (InteractedChar.CheckEnding()) { print("ending reached"); return; }
        if (isAtEnding) { print("ending point"); return; }
        if (IsWaitingForPlayer) { print("not player turn"); return; }
        if (!areEventsChecked && CheckPrecedingEvents()) //if there is anything that should come before the next chat
        {
            //narrator
            areEventsChecked = true;
            print("Found Preceding Events");
        }

        CheckEventText();
        if (CheckEarlyNarrator()) return;
        if (InteractedChar.hasSomethingToSay)
            PrintCharacterDialogue();
        else
        {
            if (CheckNarrator()) return;
            GetDialogueOptions();
        }

    }

    private void TextRevealed()
    {
        if (InteractedChar.PeekOptions() == 0 || isNarratorQueued || isEarlyNarratorSpeaking) //if this dialogue step has a narrator or no player options
        {
            if (InteractedChar.PeekEnding()) //unless it is an ending point
            {
                isAtEnding = true;
                isTypeWriterReady = true;
                GameplayManager.Instance.ToggleEndDialogue(true);
            }
            else //delay autostep
            {
                StartCoroutine(nameof(GetNextStep));
            }
        }
        else
        {
            isTypeWriterReady = true;
            StepForward();
        }
        if (isGripQueued) EnableGrip();
    }

    private IEnumerator GetNextStep()
    {
        yield return new WaitForSeconds(2);
        isTypeWriterReady = true;
        StepForward();
    }

    public void SetInteractedChar(Character character)
    {
        InteractedChar = character;
        InteractedChar.hasSomethingToSay = true;
        //InteractedChar.SetDialogueTree(GameplayManager.Instance.GetCurrentMapID());
        //InteractedChar.SetDialogueTree(0);
    }

    public Character GetInteractedChar()
    {
        return InteractedChar;
    }

    //print what the character has to say
    public void PrintCharacterDialogue()
    {
        SetCharDialogue(InteractedChar.GetDialogue());
    }

    void SetCharDialogue(string input)
    {
        characterDialogueBox.text = input;
        typeWriter.Hi();
        isTypeWriterReady = false;
    }

    //print what the player can reply
    public void GetDialogueOptions()
    {
        ParseOptions(InteractedChar.GetAllOptions());
    }

    //read through the reply options and put them into buttons
    void ParseOptions(StringInt[] playerOptions)
    {
        areEventsChecked = false;
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
                if (item.condi == Condition.EarlyNarrator)
                {
                    PrintEarlyNarrator(item.text);
                    return true;
                }
                if (item.condi == Condition.EnableGrip)
                {
                    isGripQueued = true;
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

    private int[] GetOptionsWithMissingInfo() //run through the conditions to filter out options that require having or not having information
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
                if (condition.condi == Condition.InformationMiss)
                {
                    if (player.CheckIfHasInformation(condition.num))
                        fails.Add(condition.optionNum);
                }
            }
            if (fails.Count > 0)
                return fails.ToArray();
        }
        return default;
    }

    public void PickReply(int index, int pointer)
    {
        IsWaitingForPlayer = false;
        if (CheckFollowingCondition(index))
        {
            InteractedChar.GiveAnswer(pointer);
            StepForward();
        }
        HideOptions();
        //areEventsChecked = false;

    }

    private bool CheckFollowingCondition(int index) //returns false if grip exits
    {
        if (InteractedChar.CheckOptionConditions())
        {
            foreach (var item in InteractedChar.GetAllConditions())
            {
                if (item.optionNum == index)
                {
                    if (item.condi == Condition.InformationGained)
                    {
                        if (!player.CheckIfHasInformation(item.num))
                        {
                            player.GainInformation(item.num);
                            PrintEventText(item.text);
                        }
                    }

                    if (item.condi == Condition.PlayerState)
                    {
                        player.ChangeCompleteness(item.num > 0);
                    }

                    if (item.condi == Condition.ExitGrip)
                    {
                        print("Exiting grip");
                        GameplayManager.Instance.UnGrip();
                        InteractedChar.SetDialogueTree(2, item.num);
                        return false;
                    }
                }
            }
        }
        return true;
    }

    void EnableGrip()
    {
        isGripQueued = false;
        GameplayManager.Instance.EnableGrip();
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
        print("Narrator Queued");
        isNarratorQueued = true;
        NarratorText.text = "Narrator: " + text;
    }

    public void PrintEarlyNarrator(string text)
    {
        print("Early Narrator Queued");
        isEarlyNarratorQueued = true;
        earlyNarratorText = "Narrator: " + text;
    }

    private bool CheckNarrator()
    {
        if (isNarratorQueued) //activate narrator if he is queued
        {
            //NarratorText.gameObject.SetActive(true);
            SetCharDialogue(NarratorText.text);
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

    bool CheckEarlyNarrator()
    {
        if (isEarlyNarratorQueued) //activate narrator if he is queued
        {
            SetCharDialogue(earlyNarratorText);
            isEarlyNarratorSpeaking = true;
            isEarlyNarratorQueued = false;
            return true;
        }
        if (isEarlyNarratorSpeaking)
        {
            isEarlyNarratorSpeaking = false;
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
