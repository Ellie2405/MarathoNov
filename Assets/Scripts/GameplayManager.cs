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
    [SerializeField] Log log;
    [SerializeField] GameObject logWindow;
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
    bool isLogOpen = false;
    bool isGripEnabled = false;
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
            if (state)
                item.gameObject.SetActive(true);
            else
                item.Hide();
        }
        if (state)
            ToggleNavigationUI(!state);
        else
            StartCoroutine(nameof(ToggleNavUI));
    }

    public void ToggleNavigationUI(bool state)
    {
        worldButtonLayouts[tm.GetCurrentMapID()].SetActive(state);
    }

    IEnumerator ToggleNavUI()
    {
        yield return new WaitForSeconds(1);
        ToggleNavigationUI(true);
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
        LogEvent($"Started conversation with {dm.GetInteractedChar().Name()}");
        if (isGripEnabled) gripButton.gameObject.SetActive(true);
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
        gripButton.gameObject.SetActive(false);
        IsCharacterIn = false;
        tm.ToggleNavigationUI(true);
        ToggleDialogueVisuals(false);
        FadeCharacterOut();
        LogEvent($"Ended conversation with {dm.GetInteractedChar().Name()}");
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
        isGripEnabled = true;
    }

    public void Grip() //grip button pressed
    {
        if (player.isGripping || dm.GetInteractedChar().wasGripped) return;
        isControlEnabled = false;
        ToggleStartDialogue(false);
        ToggleEndDialogue(false);
        player.StartGrip();
        FadeCharacterOut();
        dm.GetInteractedChar().ApplyGrip();
        dm.ResetSettings();
        Invoke(nameof(GripGraphic), 2);
        LogEvent($"You gripped {dm.GetInteractedChar().Name()}");
        //Invoke(nameof(dm.StepForward), 4);
    }

    public void UnGrip()
    {
        isControlEnabled = false;
        player.EndGrip();
        FadeCharacterOut();
        Invoke(nameof(UngripGraphic), 2);
        LogEvent($"You stopped gripping {dm.GetInteractedChar().Name()}");
    }

    public void FadeCharacterIn()
    {
        characterPortraitHolder.transform.DOLocalMoveX(700, 2).OnComplete(() => { isControlEnabled = true; if (dialogueStarted) dm.StepForward(); });
    }

    public void FadeCharacterOut()
    {
        characterPortraitHolder.transform.DOLocalMoveX(1500, 2).SetEase(Ease.InQuad);
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

    #region Log

    public void LogMessage(string str)
    {
        log.AddMessage(str);
    }
    public void LogPlayer(string str)
    {
        log.AddPlayerMessage(str);
    }
    public void LogNarrator(string str)
    {
        log.AddNarrator(str);
    }

    public void LogEvent(string str)
    {
        log.AddEvent(str);
    }

    public void ToggleLogWindow()
    {
        logWindow.SetActive(!isLogOpen);
        //UIElements[0].gameObject.SetActive(isLogOpen);
        //UIElements[1].gameObject.SetActive(isLogOpen);
        isLogOpen = !isLogOpen;
    }

    #endregion
}
