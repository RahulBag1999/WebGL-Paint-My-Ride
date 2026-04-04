using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiCell : MonoBehaviour
{
    private ColorData colorData;

    public TMP_Text cellIndex;
    public Image cellImage;

    private GridCellType cellType;
    private int row;
    private int column;
    private int index;

    private EssentialConfigData _essentialConfigData;
    private LevelConfig levelConfig;
    private GameSettings gameSettings;

    public void Init(EssentialConfigData essentialConfigData)
    {
        _essentialConfigData = essentialConfigData;

        colorData = _essentialConfigData.AccessConfig<ColorData>();
        gameSettings = _essentialConfigData.AccessConfig<GameSettings>();

        cellIndex.enabled = gameSettings.isDebug;
    }   

    public void SetData(LevelConfig config, int id)
    {
        levelConfig = config;
        index = id;

        row = levelConfig.fullGrid[index].row;
        column = levelConfig.fullGrid[index].col;  
        
        cellType = levelConfig.fullGrid[index].cellType;

        cellIndex.text = $"{row},{column}";

        cellImage.sprite = colorData.GetColorDatum(GetColorCode(cellType)).coloredTile;
    }

    private ColorCode GetColorCode(GridCellType cellType)
    {
        switch (cellType) 
        {
            case GridCellType.REDCELL: return ColorCode.RED;
            case GridCellType.GREENCELL: return ColorCode.GREEN;
            case GridCellType.BLUECELL: return ColorCode.BLUE;
            case GridCellType.YELLOWCELL: return ColorCode.YELLOW;
            case GridCellType.PURPLECELL: return ColorCode.PURPLE;
        }
        return ColorCode.NONE;
    }
}