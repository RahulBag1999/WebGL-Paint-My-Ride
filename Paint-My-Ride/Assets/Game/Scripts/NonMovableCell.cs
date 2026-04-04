using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NonMovableCell : MonoBehaviour
{
    public TMP_Text cellIndex;
    public SpriteRenderer spriteRenderer;

    private ColorCode colorCode;
    public ColorCode originalColorCode => colorCode;
    private ColorCode appliedColorCode = ColorCode.NONE;
    public ColorCode AppliedColorCode => appliedColorCode;

    private EssentialConfigData _essentialConfigData;
    private LevelConfig levelConfig;
    private GameSettings gameSettings;
    private ColorData colorData;

    private int row;
    private int column;
    private int index;

    private bool isColoured = false;  

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

        gameObject.name = $"NMC {row},{column}";

        cellIndex.text = $"{row},{column}";

        if (config != null) 
        {
            if (config.fullGrid[id].cellType != GridCellType.EMPTY && 
                (
                config.fullGrid[id].cellType != GridCellType.REDCAT ||
                config.fullGrid[id].cellType != GridCellType.GREENCAT ||
                config.fullGrid[id].cellType != GridCellType.YELLOWCAT ||
                config.fullGrid[id].cellType != GridCellType.BLUECAT
                ))
            {
                colorCode = Utility.GetColorCodeByGridCellType(config.fullGrid[id].cellType);
            }
        }
    }

    public bool HasColorMatched()
    {
        return colorCode == appliedColorCode;
    }

    public bool HasCellColored()
    {
        return isColoured;
    }   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.TryGetComponent(out MovableCell mCell))
            {
                if (appliedColorCode != mCell.ColorCode)
                {
                    appliedColorCode = mCell.ColorCode;
                    spriteRenderer.sprite = colorData.GetColorDatum(appliedColorCode).coloredTile;

                    if (!isColoured) isColoured = true;
                }
            }
        }
    }
}