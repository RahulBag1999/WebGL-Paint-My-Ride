using UnityEngine;

public class NonMovableCellTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int row;
    private int column;
    private int index;

    public void SetData(LevelConfig config, int id)
    {
        index = id;

        row = config.fullGrid[index].row;
        column = config.fullGrid[index].col;

        gameObject.name = $"NMC_Tile {row},{column}";
    }
}