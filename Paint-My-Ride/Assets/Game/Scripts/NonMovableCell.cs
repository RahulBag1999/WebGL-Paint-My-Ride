using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NonMovableCell : MonoBehaviour
{
    private ColorData colorData;
    public TMP_Text cellIndex;
    public MeshRenderer cellRenderer;

    private ColorCode colorCode;
    public ColorCode originalColorCode => colorCode;
    private ColorCode appliedColorCode = ColorCode.NONE;
    public ColorCode AppliedColorCode => appliedColorCode;

    private LevelConfig levelConfig;
    private GameSettings gameSettings;
    private Material currentMat;  

    private int row;
    private int column;
    private int index;

    private bool isColoured = false;

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

        gameObject.name = $"NMC {row},{column}";

        cellIndex.text = $"{row},{column}";

        if (config != null) 
        {
            if (config.fullGrid[id].cellType != GridCellType.EMPTY && 
                (
                config.fullGrid[id].cellType != GridCellType.REDCAR ||
                config.fullGrid[id].cellType != GridCellType.GREENCAR ||
                config.fullGrid[id].cellType != GridCellType.YELLOWCAR ||
                config.fullGrid[id].cellType != GridCellType.BLUECAR
                ))
            {
                colorCode = Utility.GetColorCodeByGridCellType(config.fullGrid[id].cellType);
                SetColorCode(colorCode);
            }
        }
    }

    private void SetColorCode(ColorCode code)
    {
        colorCode = code;
    }

    public bool HasColorMatched()
    {
        return colorCode == appliedColorCode;
    }

    public bool HasCellColored()
    {
        return isColoured;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.TryGetComponent(out MovableCell carCell))
            {
                if (appliedColorCode != carCell.ColorCode)
                {
                    currentMat = null;
                    appliedColorCode = carCell.ColorCode;
                    currentMat = colorData.GetColorDatum(appliedColorCode).material;

                    if (!isColoured) isColoured = true;
                    cellRenderer.material = currentMat;
                }
            }
        }
    }
}
