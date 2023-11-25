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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeMap(1);
        }
    }

    public void ChangeMap(int mapID)
    {
        ToggleNavigationUI();
        if (!isMapChanging)
        {
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
        
    }

    void ToggleNavigationUI()
    {
        NavigationUI.SetActive(!NavigationUI.activeInHierarchy);
    }

    void ToggleNavigationUI(bool toggle)
    {
        NavigationUI.SetActive(toggle);
    }

    void ChangeMapSprite()
    {
        BG.sprite = maps[1].maps[nextMap];

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
