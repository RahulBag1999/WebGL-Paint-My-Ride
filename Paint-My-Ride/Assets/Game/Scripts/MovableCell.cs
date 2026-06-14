using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class MovableCell : MonoBehaviour
{
    private LevelConfig levelConfig;
    private GameSettings gameSettings;
    private MovableCellData movableCellData;

    private SpriteRenderer sr;

    private int index;
    private int row;
    private int col;

    private GridCellType cellType = GridCellType.NONE;

    private bool canInteract = true;
    private bool canMove = false;
    private bool hasMovedToDestCell = false;

    private Vector3 destinationCellPos;
    private ColorCode colorCode = ColorCode.NONE;
    public ColorCode ColorCode { get { return colorCode; } }

    // -------------------- Animation --------------------
    private AnimationState currentState;
    private ViewDirection currentView = ViewDirection.Side;

    private Sprite[] currentSprites;
    private float frameRate = 6f;

    private int frame;
    private float timer;

    private MovableCellData.CellData cellData;
    // ---------------------------------------------------

    private GameplayHelper _gameplayHelper;
    private EssentialConfigData _essentialConfigData;

    public void Init(GameplayHelper gameplayHelper, EssentialConfigData essentialConfigData)
    {
        _gameplayHelper = gameplayHelper;
        _essentialConfigData = essentialConfigData;

        gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
        movableCellData = _essentialConfigData.AccessConfig<MovableCellData>();

        sr = GetComponent<SpriteRenderer>();
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

        cellData = movableCellData.GetCellData(cellType);

        if (cellData == null)
        {
            Debug.LogError($"No CellData found for {cellType}");
            return;
        }

        DelayToSetIdle().Forget();

        SetOrientation();
    }

    private async UniTaskVoid DelayToSetIdle()
    {
        await UniTask.Delay(
            TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, 0.3f)),
            DelayType.DeltaTime,
            cancellationToken: this.GetCancellationTokenOnDestroy());

        SetState(AnimationState.Idle);
    }

    private void SetOrientation()
    {
        // Left (face right → Side view)
        if (col == levelConfig.Columns - 1)
        {
            sr.flipX = false;
            SetView(ViewDirection.Side);
        }
        // Right (face left → Side view)
        else if (col == 0)
        {
            sr.flipX = true;
            SetView(ViewDirection.Side);
        }
        // Top (face down → TopDown view)
        else if (row == 0)
        {
            SetView(ViewDirection.TopDown);
        }
        // Bottom (face up → TopDown view)
        else if (row == levelConfig.Rows - 1)
        {
            SetView(ViewDirection.TopDown);
        }
    }

    private void OnMouseDown()
    {
        if (!canInteract)
            return;

        if (_gameplayHelper.IsGamePause)
            return;

        if (!_gameplayHelper.IsGameContinue)
            return;

        if (_gameplayHelper.IsGameOver)
            return;

        if (hasMovedToDestCell)
            return;

        if (_gameplayHelper.IsCellMoving)
            return;

        if (!canMove)
        {
            canMove = true;
            _gameplayHelper.StartUndoRecording(this);
            SetState(AnimationState.Walk);

            GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Meow1");
        }
    }

    private void Update()
    {
        UpdateAnimation();

        if (!canMove) return;

        _gameplayHelper.CellMovingStatus(true);

        transform.position = Vector3.MoveTowards(
            transform.position,
            destinationCellPos,
            gameSettings.cellMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destinationCellPos) < 0.01f)
        {
            canMove = false;
            hasMovedToDestCell = true;

            SetState(AnimationState.Idle);

            _gameplayHelper.CellMovingStatus(false);
            GameHelper.Instance.InvokeAction(GameConstants.CellColorCompletion, null);
        }
    }

    public bool IsMovable()
    {
        return !hasMovedToDestCell;
    }

    private void SetState(AnimationState newState)
    {
        //if (currentState == newState) return;
        //if (cellData == null) return;

        var anim = cellData.GetAnimation(newState, currentView);

        if (anim == null || anim.sprites == null || anim.sprites.Count == 0)
        {
            Debug.LogWarning($"No animation for {newState} - {currentView}");
            return;
        }

        currentState = newState;

        currentSprites = anim.sprites.ToArray();
        frameRate = anim.frameRate;

        frame = 0;
        timer = 0f;

        sr.sprite = currentSprites[0];
    }

    private void UpdateAnimation()
    {
        if (currentSprites == null || currentSprites.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            frame = (frame + 1) % currentSprites.Length;
            sr.sprite = currentSprites[frame];
        }
    }

    public void SetView(ViewDirection view)
    {
        if (currentView == view) return;

        currentView = view;

        SetState(currentState);
    }

    public void SetInteractibility(bool isInteractible)
    {
        canInteract = isInteractible;
    }

    public void ResetMovement()
    {
        hasMovedToDestCell = false;
        canMove = false;

        SetState(AnimationState.Idle);
    }

    public int GetIndex()
    {
        return index;
    }
}