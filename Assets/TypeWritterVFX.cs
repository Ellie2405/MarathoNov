using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TypeWritterVFX : MonoBehaviour
{
    private TMP_Text _textBox;

    // Basic Typewriter Functionality
    private int _currentVisibleCharacterIndex;
    private Coroutine _typewriterCoroutine;
    private bool _readyForNewText = true;

    private WaitForSeconds _simpleDelay;
    private WaitForSeconds _interpunctuationDelay;

    [Header("Typewriter Settings")]
    [SerializeField]
    private float charactersPerSecond = 20;

    [SerializeField] private float interpunctuationDelay = 0.5f;


    // Skipping Functionality
    public bool CurrentlySkipping { get; private set; }
    private WaitForSeconds _skipDelay;

    [Header("Skip options")]
    [SerializeField]
    private bool quickSkip;

    [SerializeField] [Min(1)] private int skipSpeedup = 5;


    // Event Functionality
    private WaitForSeconds _textboxFullEventDelay;

    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float sendDoneDelay = 0.25f; // In testing, I found 0.25 to be a good value

    public static event Action CompleteTextRevealed;
    public static event Action<char> CharacterRevealed;


    private void OnEnable()
    {

        _textBox = GetComponent<TMP_Text>();
        _textBox.maxVisibleCharacters = 0;

        _simpleDelay = new WaitForSeconds(1 / charactersPerSecond);
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);

        _skipDelay = new WaitForSeconds(1 / (charactersPerSecond * skipSpeedup));
        _textboxFullEventDelay = new WaitForSeconds(sendDoneDelay);

        //StartCoroutine(StartTyping());
    }

    [ContextMenu("hi")]
    public void Hi()
    {
        _textBox.maxVisibleCharacters = 0;
        StartCoroutine(StartTyping());
    }

    private IEnumerator StartTyping()
    {
        yield return new WaitForEndOfFrame();
        PrepareForNewText();
    }
    //private void OnEnable()
    //{
    //    TMPro_EventManager.TEXT_CHANGED_EVENT.Add(PrepareForNewText);
    //}

    //private void OnDisable()
    //{
    //    TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(PrepareForNewText);
    //}

    #region Skipfunctionality

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_textBox.maxVisibleCharacters != _textBox.textInfo.characterCount - 1)
                Skip();
        }
    }

    #endregion

    private void PrepareForNewText()
    {
        //_textBox.maxVisibleCharacters = 0;
        //if (obj != _textBox || !_readyForNewText || _textBox.maxVisibleCharacters >= _textBox.textInfo.characterCount)
        //    return;

        CurrentlySkipping = false;
        _readyForNewText = false;

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        _typewriterCoroutine = StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;

        while (_currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            var lastCharacterIndex = textInfo.characterCount - 1;

            if (_currentVisibleCharacterIndex >= lastCharacterIndex)
            {
                _textBox.maxVisibleCharacters++;
                yield return _textboxFullEventDelay;
                CompleteTextRevealed?.Invoke();
                _readyForNewText = true;
                yield break;
            }

            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;

            _textBox.maxVisibleCharacters++;

            if (!CurrentlySkipping &&
                (character == '?' || character == '.' || character == ',' || character == ':' ||
                 character == ';' || character == '!' || character == '-'))
            {
                yield return _interpunctuationDelay;
            }
            else
            {
                yield return CurrentlySkipping ? _skipDelay : _simpleDelay;
            }

            CharacterRevealed?.Invoke(character);
            _currentVisibleCharacterIndex++;
        }
    }

    private void Skip()
    {
        if (CurrentlySkipping)
            return;

        CurrentlySkipping = true;

        if (!quickSkip)
        {
            StartCoroutine(SkipSpeedupReset());
            return;
        }

        StopCoroutine(_typewriterCoroutine);
        _textBox.maxVisibleCharacters = _textBox.textInfo.characterCount;
        _readyForNewText = true;
        CompleteTextRevealed?.Invoke();
    }

    private IEnumerator SkipSpeedupReset()
    {
        yield return new WaitUntil(() => _textBox.maxVisibleCharacters == _textBox.textInfo.characterCount - 1);
        CurrentlySkipping = false;
    }
}



