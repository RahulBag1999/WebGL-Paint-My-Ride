using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup uiGridLayout;
    [SerializeField] private RectTransform uiBgRect;
    [SerializeField] private SpriteRenderer gridBG;
    [SerializeField] private Vector2 uiBgPadding = new Vector2(20f, 20f);
    [SerializeField] private RectTransform targetTextRect;
    [SerializeField] private float targetOffsetY = 30f; 

    private GameSettings _gameSettings;
    private GameThemeData _gameThemeData;
    private LevelConfig _currentLevelConfig;

    private GameplayHelper _gameplayHelper;
    private EssentialConfigData _essentialConfigData;

    public void Init(GameplayHelper gameplayHelper, EssentialConfigData essentialConfigData)
    {
        _gameplayHelper = gameplayHelper;
        _essentialConfigData = essentialConfigData;

        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
        gridBG.enabled = false;
    }  

    public void GenerateAllGrid(LevelConfig levelConfig)
    {
        _currentLevelConfig = levelConfig;

        if (_gameSettings == null)
        {
            Debug.LogWarning("Game Settings not found in Resources");
            return;
        }

        if (_currentLevelConfig != null)
        {
            GenerateWorldGrid(_currentLevelConfig.Rows, _currentLevelConfig.Columns);
            GenerateUIGrid();
            gridBG.enabled = true;
            Debug.Log("Grid generated");
        }
    }

    /// <summary>
    /// World grid generation (UPDATED FOR 2D)
    /// </summary>
    private void GenerateWorldGrid(int rows, int columns)
    {
        if (_gameSettings.nmCellPrefab == null)
            return;

        ClearWorldGrid();

        //Grid size calculation
        float stepX = _gameSettings.mcSize.x + _gameSettings.worldSpacing.x;
        float stepY = _gameSettings.mcSize.y + _gameSettings.worldSpacing.y;

        float gridWidth = (columns - 1) * stepX;
        float gridHeight = (rows - 1) * stepY;

        //Positionable grid
        Vector3 topLeftOrigin = new Vector3(
            _gameSettings.gridPosition.x - gridWidth / 2f,
            _gameSettings.gridPosition.y + gridHeight / 2f,
            0f
        );

        //Grid Bg size calculation
        Vector3 centerPos = new Vector3(_gameSettings.gridPosition.x, _gameSettings.gridPosition.y, 0f);

        //Place behind grid (important)
        gridBG.transform.position = centerPos + new Vector3(0, 0, 1f);

        //Calculate final size with padding
        float finalWidth = gridWidth + _gameSettings.bgPadding.x * 2f;
        float finalHeight = gridHeight + _gameSettings.bgPadding.y * 2f;

        //Get sprite size
        Vector2 spriteSize = gridBG.sprite.bounds.size;

        //Scale properly
        gridBG.transform.localScale = new Vector3(
            finalWidth / spriteSize.x,
            finalHeight / spriteSize.y,
            1f
        );

        int index = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 worldPos = GetCellWorldPos(topLeftOrigin, r, c, stepX, stepY);

                // Non-movable cells
                if (_currentLevelConfig.fullGrid[index].cellType != GridCellType.EMPTY &&
                    !IsMovableCell(index).Item1)
                {
                    NonMovableCell cell = Instantiate(
                        _gameSettings.nmCellPrefab,
                        worldPos,
                        Quaternion.identity,
                        transform);

                    cell.transform.localScale = _gameSettings.nmcSize;
                    cell.Init(_gameplayHelper, _essentialConfigData);
                    cell.SetData(_currentLevelConfig, index);

                    _gameplayHelper.AddNmCellsToList(cell);
                }

                // Movable cells
                else if (IsMovableCell(index).Item1)
                {
                    MovableCell cell = Instantiate(
                        _gameSettings.mCellPrefab,
                        worldPos + new Vector3(0f, 0.2f, 0f),
                        Quaternion.identity,
                        transform);

                    cell.transform.localScale = _gameSettings.mcSize;

                    //Movable cell tile bg
                    GenerateNonMovableCellTile(index, worldPos, _currentLevelConfig);

                    int destRow;
                    int destCol;

                    (int, int) GetDestinationCell()
                    {
                        if (r == _currentLevelConfig.Rows - 1) destRow = 0;
                        else if (r == 0) destRow = _currentLevelConfig.Rows - 1;
                        else destRow = r;

                        if (c == _currentLevelConfig.Columns - 1) destCol = 0;
                        else if (c == 0) destCol = _currentLevelConfig.Columns - 1;
                        else destCol = c;

                        return (destRow, destCol);
                    }

                    var dest = GetDestinationCell();

                    Vector3 destPos = GetCellWorldPos(
                        topLeftOrigin,
                        dest.Item1,
                        dest.Item2,
                        stepX,
                        stepY);

                    cell.Init(_gameplayHelper, _essentialConfigData);
                    cell.SetData(_currentLevelConfig, index, destPos);

                    _gameplayHelper.AddMCellsToList(cell);
                }

                else if (!IsMovableCell(index).Item1 && IsMovableCell(index).Item2 == GridCellType.EMPTY)
                {
                    GenerateNonMovableCellTile(index, worldPos, _currentLevelConfig);
                }

                index++;
            }
        }
    }

    private void GenerateNonMovableCellTile(int id, Vector3 pos, LevelConfig config)
    {
        NonMovableCellTile tile = Instantiate(_gameSettings.tileBg, pos, Quaternion.identity, transform);
        tile.SetData(config, id);
        tile.transform.localScale = _gameSettings.nmcSize;
    }

    /// <summary>
    /// UI grid generation (UNCHANGED)
    /// </summary>
    private void GenerateUIGrid()
    {
        if (uiGridLayout == null || _gameSettings.uiCellPrefab == null)
            return;

        ClearUIGrid();

        int rows = _currentLevelConfig.Rows;
        int cols = _currentLevelConfig.Columns;

        int index = 0;

        uiGridLayout.constraintCount = cols - 2;

        // Get cell size + spacing
        Vector2 cellSize = uiGridLayout.cellSize;
        Vector2 spacing = uiGridLayout.spacing;

        int activeCols = cols - 2;
        int activeRows = rows - 2;

        // Calculate grid size
        float gridWidth = (activeCols * cellSize.x) + ((activeCols - 1) * spacing.x);
        float gridHeight = (activeRows * cellSize.y) + ((activeRows - 1) * spacing.y);

        // Resize background
        if (uiBgRect != null)
        {
            uiBgRect.sizeDelta = new Vector2(
                gridWidth + uiBgPadding.x * 2f,
                gridHeight + uiBgPadding.y * 2f
            );

            if (targetTextRect != null)
            {
                float bgHeight = uiBgRect.sizeDelta.y;

                targetTextRect.anchoredPosition = new Vector2(
                    targetTextRect.anchoredPosition.x,
                    (bgHeight / 2f) + targetOffsetY
                );

                targetTextRect.anchoredPosition = new Vector2(0f, (bgHeight / 2f) + targetOffsetY);
            }
        }

        // Generate cells
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (_currentLevelConfig.fullGrid[index].cellType != GridCellType.EMPTY &&
                    !IsMovableCell(index).Item1)
                {
                    UiCell cellGO = Instantiate(
                        _gameSettings.uiCellPrefab,
                        uiGridLayout.transform);

                    if (cellGO != null)
                    {
                        cellGO.Init(_essentialConfigData);
                        cellGO.SetData(_currentLevelConfig, index);
                    }
                }

                index++;
            }
        }

        // Force layout rebuild (IMPORTANT)
        LayoutRebuilder.ForceRebuildLayoutImmediate(uiGridLayout.GetComponent<RectTransform>());
    }

    /// <summary>
    /// UPDATED FOR 2D (XY PLANE)
    /// </summary>
    private Vector3 GetCellWorldPos(Vector3 origin, int r, int c, float stepX, float stepY)
    {
        return origin + new Vector3(
            c * stepX,
            -r * stepY,   //  rows go downward
            0f            //  no Z movement
        );
    }

    private (bool, GridCellType) IsMovableCell(int id)
    {
        switch (_currentLevelConfig.fullGrid[id].cellType)
        {
            case GridCellType.REDCAT: return (true, GridCellType.REDCAT);
            case GridCellType.GREENCAT: return (true, GridCellType.GREENCAT);
            case GridCellType.BLUECAT: return (true, GridCellType.BLUECAT);
            case GridCellType.YELLOWCAT: return (true, GridCellType.YELLOWCAT);
            case GridCellType.PURPLECAT: return (true, GridCellType.PURPLECAT);
            case GridCellType.EMPTY: return (false, GridCellType.EMPTY);
        }
        return (false, GridCellType.EMPTY);
    }    

    public void SetGridBg(int id)
    {
        gridBG.sprite = _gameThemeData.GetGameTheme(id).gridBg;
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
        for (int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void Cleanup()
    {
        gridBG.enabled = false;
        ClearUIGrid();
        ClearWorldGrid();
    }
}