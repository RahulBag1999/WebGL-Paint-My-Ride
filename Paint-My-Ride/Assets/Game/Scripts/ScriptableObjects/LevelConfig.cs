using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int levelId;

    [Space]

    [Min(2)]
    public int rows;
    [Min(2)]
    public int columns;

    private int gridCount;

    public int Rows => rows;
    public int Columns => columns;

    public GridData[] fullGrid;

    public int GetCellIndex( int r, int c)
    {
        return Array.FindIndex(fullGrid, g => g.row == r && g.col == c);
    }

    [Serializable]
    public class GridData
    {
        public int row;
        public int col;
        public GridCellType cellType;
    }

    // Ensures gridCount is always valid
    private void OnValidate()
    {
        if (gridCount < 1)
            gridCount = 1;
    }

    public void Cleanup()
    {
        fullGrid = null;
        rows = 0; columns = 0;
    }

    private void CalculateGridSize()
    {
        rows += 2;
        columns += 2;

        gridCount = (rows + 2) * (columns + 2);
    }

    [ContextMenu("Initiate Grid")]
    public void InitializeGrid()
    {
        CalculateGridSize();

        fullGrid = new GridData[Rows * Columns];

        int index = 0;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                fullGrid[index] = new GridData
                {
                    row = r,
                    col = c,
                    cellType = GridCellType.EMPTY
                };
                index++;
            }
        }

        Debug.Log($"Grid initialized: {Rows} x {Columns} ({fullGrid.Length} cells)");
    }
}
