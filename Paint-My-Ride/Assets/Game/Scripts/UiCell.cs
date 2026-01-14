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

    private LevelConfig levelConfig;
    private GameSettings gameSettings;

    private void Awake()
    {
        colorData = Resources.Load<ColorData>(nameof(ColorData));
        gameSettings = Resources.Load<GameSettings>(nameof(GameSettings));

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

        cellImage.color = colorData.GetColorDatum(GetColorCode(cellType)).color;
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