using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovableCellData", menuName = "Game/Movable Cell Data")]
public class MovableCellData : EssentialConfigScriptableObject
{
    public List<CellData> movableCellList = new List<CellData>();

    private Dictionary<GridCellType, CellData> cellMap;

    public override void Init()
    {
        if (cellMap != null) return;

        cellMap = new Dictionary<GridCellType, CellData>();

        foreach (var cell in movableCellList)
        {
            if (cell == null) continue;

            if (!cellMap.ContainsKey(cell.cellType))
                cellMap.Add(cell.cellType, cell);
            else
                Debug.LogWarning($"Duplicate CellData for {cell.cellType}");
        }
    }

    public CellData GetCellData(GridCellType type)
    {
        //Init();

        if (cellMap.TryGetValue(type, out var cell))
            return cell;

        Debug.LogError($"No CellData found for {type}");
        return null;
    }    

    // ----------------------------

    [Serializable]
    public class CellData
    {
        public GridCellType cellType;

        public List<AnimationData> animations = new List<AnimationData>();

        private Dictionary<(AnimationState, ViewDirection), AnimationData> animMap;

        public void Init()
        {
            if (animMap != null) return;

            animMap = new Dictionary<(AnimationState, ViewDirection), AnimationData>();

            foreach (var anim in animations)
            {
                var key = (anim.state, anim.view);

                if (!animMap.ContainsKey(key))
                    animMap.Add(key, anim);
                else
                    Debug.LogWarning($"Duplicate animation: {key}");
            }
        }

        public AnimationData GetAnimation(AnimationState state, ViewDirection view)
        {
            Init();

            if (animMap.TryGetValue((state, view), out var anim))
                return anim;

            return null;
        }
    }

    // ----------------------------

    [Serializable]
    public class AnimationData
    {
        public AnimationState state;
        public ViewDirection view;

        public List<Sprite> sprites = new List<Sprite>();

        public float frameRate = 6f;
    }
}