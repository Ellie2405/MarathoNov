using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class Navigator : MonoBehaviour//, IPointerClickHandler
{
    static public event Action<int> OnNavigatorClicked;

    [SerializeField] int DestinationMapID;
    [SerializeField] Image img;
    [SerializeField] float fadeSpeed;
    [SerializeField] bool rotate;
    [SerializeField] float rotateSpeed;
    Tween tween;
    Color defaultColor;

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    OnNavigatorClicked.Invoke(DestinationMapID);
    //}

    public void ChangeScene()
    {
        OnNavigatorClicked?.Invoke(DestinationMapID);
    }

    private void Awake()
    {
        defaultColor = img.color;
    }

    private void OnEnable()
    {
        img.color = defaultColor;
        tween = img.DOFade(0.2f, fadeSpeed).SetLoops(-1, LoopType.Yoyo);
    }

    private void FixedUpdate()
    {
        if (rotate)
            transform.Rotate(0, 0, rotateSpeed);
    }
    private void OnDisable()
    {
        tween.Pause();
    }
}
