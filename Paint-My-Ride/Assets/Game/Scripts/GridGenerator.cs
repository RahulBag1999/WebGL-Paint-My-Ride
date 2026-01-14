using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    public GridLayoutGroup uiGridLayout;

    private GameSettings gameSettings;
    private MovableCellData movableCellData;
    private LevelConfig currentLevelConfig;

    private void Awake()
    {
        gameSettings = Resources.Load<GameSettings>(nameof(GameSettings));
        movableCellData = Resources.Load<MovableCellData>(nameof(MovableCellData));
    }

    public void GenerateAllGrid(LevelConfig levelConfig)
    {
        currentLevelConfig = levelConfig;

        if (gameSettings == null)
        {
            Debug.LogWarning("Game Settings not found in Resources");
            return;
        }            

        if (currentLevelConfig != null ) 
        {
            GenerateWorldGrid(currentLevelConfig.Rows, currentLevelConfig.Columns);
            GenerateUIGrid();
        }
    }

    /// <summary>
    /// World grid generation
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    private void GenerateWorldGrid(int rows, int columns)
    {
        if (gameSettings.nmCellPrefab == null)
            return;

        ClearWorldGrid();

        float stepX = gameSettings.mcSize.x + gameSettings.worldSpacing.x;
        float stepZ = gameSettings.mcSize.z + gameSettings.worldSpacing.y;

        float gridWidth = (columns - 1) * stepX;
        float gridDepth = (rows - 1) * stepZ;

        Vector3 topLeftOrigin = new Vector3(
            -gridWidth / 2f,
            0f,
             gridDepth / 2f
        );

        int index = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 worldPos = GetCellWorldPos(topLeftOrigin, r, c, stepX, stepZ);

                //For empty cells to color
                if (currentLevelConfig.fullGrid[index].cellType != GridCellType.EMPTY && !IsMovableCell(index).Item1)
                {
                    NonMovableCell cell = Instantiate(gameSettings.nmCellPrefab, worldPos, Quaternion.identity, transform);              
                    cell.transform.localScale = gameSettings.mcSize;
                    cell.SetData(currentLevelConfig, index);

                    GameManager.Instance.AddNmCellsToList(cell);
                }

                //For color cars
                else if (IsMovableCell(index).Item1) 
                {
                    MovableCell mc = GetMovableCell(IsMovableCell(index).Item2);
                    if (mc != null) 
                    {
                        MovableCell cell = Instantiate(mc, worldPos, Quaternion.identity, transform);
                        cell.transform.localScale = gameSettings.nmcSize;

                        int destRow;
                        int destCol;

                        (int, int) GetDestinationCell()
                        {
                            if (r == currentLevelConfig.Rows - 1) destRow = 0;
                            else if (r == 0) destRow = currentLevelConfig.Rows - 1;
                            else destRow = r;
                            if (c == currentLevelConfig.Columns - 1) destCol = 0;
                            else if (c == 0) destCol = currentLevelConfig.Columns - 1;
                            else destCol = c;                                                   

                            return (destRow, destCol);
                        }

                        Vector3 destPos = GetCellWorldPos(topLeftOrigin, GetDestinationCell().Item1, GetDestinationCell().Item2, stepX, stepZ);

                        cell.SetData(currentLevelConfig, index, destPos);

                        GameManager.Instance.AddMCellsToList(cell);
                    }                    
                }  
                else if(currentLevelConfig.fullGrid[index].cellType == GridCellType.EMPTY)
                {
                    //NonMovableCell cell = Instantiate(nmCellPrefab, worldPos, Quaternion.identity, transform);
                    //cell.transform.localScale = Vector3.one;
                    //cell.SetData(currentLevelConfig, index);
                }

                index++;
            }
        }
    }    

    /// <summary>
    /// Ui grid generation
    /// </summary>
    private void GenerateUIGrid()
    {
        if (uiGridLayout == null || gameSettings.uiCellPrefab == null)
            return;

        ClearUIGrid();
        int index = 0;

        uiGridLayout.constraintCount = currentLevelConfig.Columns - 2;

        for (int row = 0; row < currentLevelConfig.Rows; row++)
        {
            for (int col = 0; col < currentLevelConfig.Columns; col++)
            {
                if (currentLevelConfig.fullGrid[index].cellType != GridCellType.EMPTY && !IsMovableCell(index).Item1)
                {
                    UiCell cellGO = Instantiate(gameSettings.uiCellPrefab, uiGridLayout.transform);
                    if (cellGO != null)
                    {
                        cellGO.SetData(currentLevelConfig, index);
                    }
                }                

                index++;
            }
        }
    }

    private Vector3 GetCellWorldPos(Vector3 origin, int r, int c, float stepX, float stepZ)
    {
        return origin + new Vector3(
                    c * stepX,
                    0f,
                    -r * stepZ);
    }

    private (bool, GridCellType) IsMovableCell(int id)
    {
        switch (currentLevelConfig.fullGrid[id].cellType)
        {
            case GridCellType.REDCAR:
                return (true, GridCellType.REDCAR);

            case GridCellType.GREENCAR:
                return (true, GridCellType.GREENCAR);

            case GridCellType.BLUECAR:
                return (true, GridCellType.BLUECAR);

            case GridCellType.YELLOWCAR:
                return (true, GridCellType.YELLOWCAR);

            case GridCellType.PURPLECAR:
                return (true, GridCellType.PURPLECAR);
        }
        return (false, GridCellType.NONE);        
    }

    private MovableCell GetMovableCell(GridCellType gridCellType)
    {
        return movableCellData.movableCellList.Find(x => x.cellType == gridCellType).mcPrefab;
    }    

    private void ClearUIGrid()
    {
        for (int i = uiGridLayout.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(uiGridLayout.transform.GetChild(i).gameObject);
        }
    }

    private void ClearWorldGrid()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
