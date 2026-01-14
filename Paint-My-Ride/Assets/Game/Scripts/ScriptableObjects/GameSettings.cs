using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [Header("Game")]
    public string gameName;

    [Header("Debug")]
    public bool isDebug = false;

    [Header("Movable cell")]
    public float cellMoveSpeed = 5f;

    [Header("Game end")]
    public float gameWinDelay = 1f;
    public float gameLoseDelay = 1f;

    [Header("World Grid")]
    public NonMovableCell nmCellPrefab;
    public Vector3 mcSize = Vector3.one;
    public Vector3 nmcSize = Vector3.one;
    public Vector2 worldSpacing = Vector2.zero;

    [Header("UI Grid")]
    public UiCell uiCellPrefab;
    public float uiCellSize = 100f;
    public Vector2 uiSpacing = new Vector2(10f, 10f);
}
