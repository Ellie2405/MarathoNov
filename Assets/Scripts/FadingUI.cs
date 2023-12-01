using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadingUI : MonoBehaviour
{
    [SerializeField] int FadeSpeed;
    Image img;
    private void Awake()
    {
        img = GetComponent<Image>();
        img.color = new(255, 255, 255, 0);
    }

    private void OnEnable()
    {
        img.DOFade(1, FadeSpeed);
    }

    public void Hide()
    {
        img.DOFade(0, 1).OnComplete(() => gameObject.SetActive(false));
    }
}
