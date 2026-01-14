using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MovableCellData))]
public class MovableCellData : ScriptableObject
{
    public List<CellData> movableCellList = new List<CellData>();

    [Serializable]
    public class CellData
    {
        public MovableCell mcPrefab;
        public GridCellType cellType;
    }
}
