using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NonMovableCell : MonoBehaviour
{
    public TMP_Text cellIndex;
    public SpriteRenderer spriteRenderer;

    private ColorCode colorCode;
    private ColorCode appliedColorCode = ColorCode.NONE;

    public ColorCode AppliedColorCode => appliedColorCode;

    private EssentialConfigData _essentialConfigData;
    private LevelConfig _levelConfig;
    private GameSettings _gameSettings;
    private ColorData _colorData;
    private GameplayHelper _gameplayHelper;

    private int row;
    private int column;
    private int index;

    private bool isColoured = false;  

    public void Init(GameplayHelper gameplayHelper, EssentialConfigData essentialConfigData)
    {
        _essentialConfigData = essentialConfigData;
        _gameplayHelper = gameplayHelper;

        _colorData = _essentialConfigData.AccessConfig<ColorData>();
        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();

        cellIndex.enabled = _gameSettings.isDebug;
    }

    public void SetData(LevelConfig config, int id)
    {
        _levelConfig = config;
        index = id;

        row = _levelConfig.fullGrid[index].row;
        column = _levelConfig.fullGrid[index].col;

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

    public void ResetColor(ColorCode prevColor)
    {
        appliedColorCode = prevColor;
        spriteRenderer.sprite = _colorData.GetColorDatum(prevColor).coloredTile;
        isColoured = prevColor == ColorCode.NONE ? false : true;        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.TryGetComponent(out MovableCell mCell))
            {
                if (appliedColorCode != mCell.ColorCode)
                {
                    _gameplayHelper.RecordCellState(this);

                    appliedColorCode = mCell.ColorCode;
                    spriteRenderer.sprite = _colorData.GetColorDatum(appliedColorCode).coloredTile;

                    if (!isColoured) isColoured = true;
                }
            }
        }
    }
}