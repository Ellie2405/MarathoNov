using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [SerializeField] DialogManager dm;
    [SerializeField] TravelManager tm;
    [SerializeField] Character InteractedChar;
    [SerializeField] Player player;
    int currentMapID;

    [SerializeField] GameObject startDialogueButtons;
    [SerializeField] GameObject endDialogueButton;
    [SerializeField] GameObject characterPortraitHolder;
    [SerializeField] GameObject[] characterPortraits;
    [SerializeField] GameObject[] grippedPortraits;
    [SerializeField] GameObject[] worldButtonLayouts;
    [SerializeField] SpriteRenderer Darkening;
    [SerializeField] FadingUI[] UIElements;
    Color DarkColor = new(0, 0, 0, .8f);
    Color NotDarkColor = new(0, 0, 0, 0);
    [SerializeField] Sprite test;
    bool isControlEnabled = false;
    bool IsCharacterIn = false;
    bool dialogueStarted = false;
    public bool IsWaitingForPlayer = false;

    [SerializeField] Button gripButton;
    [SerializeField] Character[] characters;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        CharacterImage.OnImageClicked += InteractWithCharacter;
    }

    private void Update()
    {
        if (isControlEnabled)
        {
            //if (Input.GetMouseButtonDown(1))
            //{
            //    if (!IsCharacterIn)
            //    {
            //        FadeCharacterIn();
            //    }
            //    else FadeCharacterOut();
            //    IsCharacterIn = !IsCharacterIn;
            //}

            if (Input.GetMouseButtonDown(0))
            {
                if (IsCharacterIn)
                {
                    dm.StepForward();
                }
            }
        }
    }

    //on interact with character, hide navigation ui? and display dialogue UI, ready systems and visuals
    public void InteractWithCharacter(int charID)
    {
        SelectPortrait(charID);
        dm.ResetSettings();
        dm.SetInteractedChar(characters[charID]);
        FadeCharacterIn();
        ToggleDialogueVisuals(true);
        ToggleStartDialogue(true);
    }

    void SelectPortrait(int ID)
    {
        foreach (var item in characterPortraits)
        {
            item.SetActive(false);
        }
        foreach (var item in grippedPortraits)
        {
            item.SetActive(false);
        }
        characterPortraits[ID].SetActive(true);
    }

    void SelectGripPortrait(int ID)
    {
        foreach (var item in characterPortraits)
        {
            item.SetActive(false);
        }
        foreach (var item in grippedPortraits)
        {
            item.SetActive(false);
        }
        grippedPortraits[ID].SetActive(true);
    }

    public void EndInteraction()
    {
        tm.ToggleNavigationUI(true);
        FadeCharacterOut();
    }

    void ToggleDialogueVisuals(bool state)
    {
        foreach (var item in UIElements)
        {
            item.gameObject.SetActive(state);
        }
        ToggleNavigationUI(!state);
    }

    public void ToggleNavigationUI(bool state)
    {
        worldButtonLayouts[tm.GetCurrentMapID()].SetActive(state);
    }

    public void ToggleStartDialogue(bool state)
    {
        startDialogueButtons.SetActive(state);
        //if (state == false)
        //{
        //    IsCharacterIn = false;
        //    tm.ToggleNavigationUI(true);
        //    ToggleDialogueVisuals(false);
        //    FadeCharacterOut();
        //}
    }

    public void StartDialogue() //talk button pressed
    {
        dialogueStarted = true;
        dm.StepForward();
        IsCharacterIn = true;
        startDialogueButtons.SetActive(false);
    }

    public void ToggleEndDialogue(bool state)
    {
        endDialogueButton.SetActive(state);
    }

    public void EndDialogue() //leave button pressed
    {
        dialogueStarted = false;
        ToggleStartDialogue(false);
        ToggleEndDialogue(false);
        IsCharacterIn = false;
        tm.ToggleNavigationUI(true);
        ToggleDialogueVisuals(false);
        FadeCharacterOut();
    }

    public void SelectWorldButtonLayout(int index)
    {
        foreach (var item in worldButtonLayouts)
        {
            item.SetActive(false);
        }
        ToggleNavigationUI(true);
    }

    public int GetCurrentMapID()
    {
        return tm.GetCurrentMapID();
    }


    public void EnableGrip()
    {
        gripButton.gameObject.SetActive(true);
    }

    public void Grip()
    {
        if (player.isGripping || dm.GetInteractedChar().wasGripped) return;
        isControlEnabled = false;
        player.StartGrip();
        FadeCharacterOut();
        dm.GetInteractedChar().ApplyGrip();
        dm.ResetSettings();
        Invoke(nameof(GripGraphic), 2);
        //Invoke(nameof(dm.StepForward), 4);
    }

    public void UnGrip()
    {
        isControlEnabled = false;
        player.EndGrip();
        FadeCharacterOut();
        Invoke(nameof(UngripGraphic), 2);
    }

    public void FadeCharacterIn()
    {
        characterPortraitHolder.transform.DOLocalMoveX(700, 2).OnComplete(() => { isControlEnabled = true; if (dialogueStarted) dm.StepForward(); });
    }

    public void FadeCharacterOut()
    {
        characterPortraitHolder.transform.DOLocalMoveX(1300, 2).SetEase(Ease.InQuad);
    }


    //dirty
    //public void GripGraphic1()
    //{
    //    characterPortraitHolder.transform.DOMoveX(15, 2).SetEase(Ease.InQuad);
    //}

    public void GripGraphic()
    {
        SelectGripPortrait(dm.GetInteractedChar().id);
        FadeCharacterIn();
    }

    public void UngripGraphic()
    {
        SelectPortrait(dm.GetInteractedChar().id);
        FadeCharacterIn();
    }

    void Darken(bool state)
    {
        if (state)
        {
            Darkening.DOColor(DarkColor, 2);
        }
        else
            Darkening.DOColor(NotDarkColor, 2);
    }
}
