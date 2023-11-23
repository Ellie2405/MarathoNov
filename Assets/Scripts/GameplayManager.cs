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
    [SerializeField] Character InteractedChar;
    [SerializeField] Player player;

    [SerializeField] GameObject characterPortrait;
    [SerializeField] SpriteRenderer Darkening;
    Color DarkColor = new(0, 0, 0, .8f);
    Color NotDarkColor = new(0, 0, 0, 0);
    [SerializeField] Sprite test;
    bool isControlEnabled = true;
    bool IsCharacterIn = false;
    public bool IsWaitingForPlayer = false;
    bool isNarratorQueued = false;
    bool isNarratorSpeaking = false;
    bool isEventQueued = false;
    bool isShowingEventText = false;

    [SerializeField] TextMeshProUGUI characterDialogue;
    [SerializeField] GameObject PlayerDialogueOptions;
    [SerializeField] PlayerDialogueOption[] options;
    [SerializeField] TextMeshProUGUI EventText;
    [SerializeField] TextMeshProUGUI NarratorText;
    [SerializeField] Button gripButton;
    [SerializeField] SpriteRenderer BG;
    [SerializeField] Sprite BGNew;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        characterDialogue.text = string.Empty;
        EventText.text = string.Empty;
        NarratorText.text = string.Empty;
    }

    private void Update()
    {
        if (isControlEnabled)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (!IsCharacterIn)
                {
                    FadeCharacterIn();
                }
                else FadeCharacterOut();
                IsCharacterIn = !IsCharacterIn;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if(IsCharacterIn)
                dm.StepForward();
            }
        }
    }

    public void ImproveBG()
    {
        BG.sprite = BGNew;
    }


    void EnableGrip()
    {
        gripButton.gameObject.SetActive(true);
    }

    public void Grip()
    {
        isControlEnabled = false;
        player.StartGrip();
        GripGraphic1();
        Invoke(nameof(GripGraphic2), 2);
        //Invoke(nameof(StepForward), 4);
        isControlEnabled = true;
    }

    public void FadeCharacterIn()
    {
        characterPortrait.transform.DOMoveX(6.5f, 2);
        //Darkening.DOColor(DarkColor, 2);
        Darken(true);
    }

    public void FadeCharacterOut()
    {
        characterPortrait.transform.DOMoveX(15, 2).SetEase(Ease.InQuad);
        //Darkening.DOColor(NotDarkColor, 2);
        Darken(false);
    }


    //dirty
    public void GripGraphic1()
    {
        characterPortrait.transform.DOMoveX(15, 2).SetEase(Ease.InQuad);
    }
    public void GripGraphic2()
    {
        InteractedChar.SwitchGraphic(1);
        characterPortrait.transform.DOMoveX(6.5f, 2);
    }

    public void GripGraphic3()
    {
        InteractedChar.SwitchGraphic(0);
        characterPortrait.transform.DOMoveX(6.5f, 2);
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
