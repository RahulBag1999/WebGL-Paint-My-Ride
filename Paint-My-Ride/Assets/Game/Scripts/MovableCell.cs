using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableCell : MonoBehaviour
{
    private LevelConfig levelConfig;
    private GameSettings gameSettings;

    private int index;
    private int row;
    private int col;

    private GridCellType cellType = GridCellType.NONE;

    private bool canMove = false;
    private bool hasMovedToDestCell = false;

    private Vector3 destinationCellPos;
    private ColorCode colorCode = ColorCode.NONE;
    public ColorCode ColorCode { get { return colorCode; } }

    private void Awake()
    {
        gameSettings = Resources.Load<GameSettings>(nameof(GameSettings));
    }

    public void SetData(LevelConfig lc, int id, Vector3 dPos)
    {
        levelConfig = lc;
        index = id;
        destinationCellPos = dPos;

        row = levelConfig.fullGrid[index].row;
        col = levelConfig.fullGrid[index].col;

        gameObject.name = $"MC {row},{col}";

        cellType = levelConfig.fullGrid[index].cellType;

        colorCode = Utility.GetColorCodeByGridCellType(cellType);

        SetOrientation();
    }

    private void SetOrientation()
    {
        //Left   
        if (col == levelConfig.Columns - 1)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        //Right
        if(col == 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }       
        //Top
        if(row == 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        //Bottom
        if(row == levelConfig.Rows - 1)
        {
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.IsGameOver)
            return;

        if (hasMovedToDestCell)
            return;

        if (GameManager.Instance.IsCellMoving)
            return;

        if (!canMove) canMove = true;
    }

    private void Update()
    {
        if (!canMove) return;

        GameManager.Instance.CellMoving(true);
        transform.position = Vector3.MoveTowards(transform.position, destinationCellPos, gameSettings.cellMoveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, destinationCellPos) < 0.01f)
        {
            canMove = false;
            hasMovedToDestCell = true;

            GameManager.Instance.CellMoving(false);
            GameManager.Instance.OnCellColorCompletion?.Invoke();
        }
    }

    public bool IsMovable()
    {
        if(hasMovedToDestCell) return false;
        else return true;
    }
}
