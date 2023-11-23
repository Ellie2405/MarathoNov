using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "ScriptableObjects/PieceDataSO", order = 2)]
public class PieceSO : ScriptableObject
{
    public int ID;
    public string Name;
    public string Description;
}
