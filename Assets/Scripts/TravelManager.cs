using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TravelManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer BG;
    [SerializeField] GameObject NavigationUI;
    int currentMapID;
    int nextMap;
    public bool isMapChanging = false;
    [SerializeField] MapCollection[] maps;

    private void Awake()
    {
        Navigator.OnNavigatorClicked += ChangeMap;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        ChangeMap(1);
    //    }
    //}

    public void ChangeMap(int mapID)
    {
        //remove the blow line and instead activate a transition animation, will be highest layer and block raycast
        GameplayManager.Instance.ToggleNavigationUI(false);
        if (!isMapChanging)
        {
            SoundManager.Instance.PlayTransitionSound();
            BG.DOColor(Color.black, 1.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() => ChangeMapComplete());
            nextMap = mapID;
            isMapChanging = true;
            Invoke(nameof(ChangeMapSprite), 1.2f);
        }
    }

    void ChangeMapComplete()
    {
        isMapChanging = false;
        ToggleNavigationUI(true);
        GameplayManager.Instance.SelectWorldButtonLayout(currentMapID);
    }

    void ToggleNavigationUI()
    {
        NavigationUI.SetActive(!NavigationUI.activeInHierarchy);
    }

    public void ToggleNavigationUI(bool toggle)
    {
        //NavigationUI.SetActive(toggle);
    }

    void ChangeMapSprite()
    {
        BG.sprite = maps[0].maps[nextMap];
        currentMapID = nextMap;
    }

    public int GetCurrentMapID()
    {
        return currentMapID;
    }
}

[System.Serializable]
class MapCollection
{
    public Sprite[] maps;
}
