using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : EssentialConfigScriptableObject
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
    public MovableCell mCellPrefab;
    public NonMovableCell nmCellPrefab;
    public Vector3 mcSize = Vector3.one;
    public Vector3 nmcSize = Vector3.one;
    public Vector2 worldSpacing = Vector2.zero;

    [Header("Grid Position (World XY)")]
    public Vector2 gridPosition;

    [Header("UI Grid")]
    public UiCell uiCellPrefab;
    public float uiCellSize = 100f;
    public Vector2 uiSpacing = new Vector2(10f, 10f);

    [Header("Movable cell back tile")]
    public NonMovableCellTile tileBg;

    [Header("Grid Background")]
    public Vector2 bgPadding = new Vector2(0.5f, 0.5f);

    [Header("Loading Screen Settings")]
    public float uvScrollSpeed = 1f;
    public float loadingTime = 3f;
    public float dotDelay = 0.4f;
    public int maxDots = 3;
    public float x, y;
    public float parallaxSpeed = 1f;

    [Header("Gameplay settings")]
    public float levelFailContinueTime = 30f;
    public byte levelFailContinueCoin = 150;

    [Header("Win popup")]
    public float delayBetweenStars = 0.2f;
    public Sprite filledStarMid;
    public Sprite filledStar;

    public override void Init()
    {
        
    }
}
