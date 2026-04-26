using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PreResultData
{
    public float remainingTime = 0f;
}

public class GameplayHelper : MonoBehaviour
{
    public class UndoData
    {
        public MovableCell movableCell;
        public Vector3 startPos;

        public List<NonMovableCell> affectedCells = new List<NonMovableCell>();
        public List<ColorCode> previousColors = new List<ColorCode>();
    }

    private PopupHandler _popupHandler;
    private EssentialConfigData _essentialConfigData;

    private List<NonMovableCell> nmCellList = new List<NonMovableCell>();
    private List<MovableCell> mCellList = new List<MovableCell>();    

    private bool _isCellMoving = false;
    private bool _isGameOver = false;
    private bool _isGamePause = false;
    private bool _isGameRestart = false;
    private bool _isGameContinue = false;
    private bool _isTutorialLevel = false;
    public bool IsCellMoving => _isCellMoving;
    public bool IsGameOver => _isGameOver;
    public bool IsGamePause => _isGamePause;

    private GameSettings _gameSettings;
    private LevelTimingData _levelTimingData;
    private LevelContainer _levelContainer;
    private LevelConfig _levelConfig;
    private UndoData _lastUndoData;
    private HandPointer _handPointer;

    private int _currentLevel = -1;    
    private int _currentThemeId = -1;    
    private float _levelTimeRemaining = 0f;
    private int _tutorialStep = 0;

    private GameLoseType _gameLoseType = GameLoseType.NONE;
    private GameEndType _gameEndType = GameEndType.NONE;

    public bool IsGameContinue => _isGameContinue;

    public  void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;

        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
        _levelTimingData = _essentialConfigData.AccessConfig<LevelTimingData>();
        _levelContainer = _essentialConfigData.AccessConfig<LevelContainer>();
    }

    private void Update()
    {
        if (!_isGameContinue)
            return;

        UpdateGameplay();
    }

    public void InitiateGameplay(int level, int themeId)
    {
        _isGameOver = false;

        _currentLevel = level;
        _currentThemeId = themeId;
        _isTutorialLevel = (_currentLevel == 0);
        _levelTimeRemaining = _levelTimingData.GetTimeForLevel(_currentLevel);
        _isGameContinue = true;

        _levelConfig = FetchLastLevelConfig(_currentLevel);
        GameConstants.CurrentLevelConfig = _levelConfig;
        GameConstants.LevelLoseLifeCount = 1;

        if (_isTutorialLevel) 
        {
            _handPointer = Instantiate(_gameSettings.handPointerPrefab);
            _handPointer.SetVisibility(true);
            StartCoroutine(TutorialCoroutine());
        } 

        GameHelper.Instance.StartListening(GameConstants.GameplayPause, HandleGameplayPauseStatus);
        GameHelper.Instance.StartListening(GameConstants.GameplayRestart, HandleGameplayRestart);
        GameHelper.Instance.StartListening(GameConstants.CellColorCompletion, CellColorCompletion);
        GameHelper.Instance.StartListening(GameConstants.UndoMovableCell, UndoLastMove);        
    }

    public void InitiateGameplay(PreResultData data)
    {
        _isGameOver = false;
        _isGameContinue = true;
        _gameEndType = GameEndType.NONE;
        _levelTimeRemaining = data.remainingTime;        
    }

    private LevelConfig FetchLastLevelConfig(int index)
    {
        return _levelContainer.GetLevelConfig(index);
    }

    private void UpdateGameplay()
    {
        if (_isGameRestart)
            return;

        if (_isGamePause)
            return;

        if (_gameEndType == GameEndType.WIN || _gameEndType == GameEndType.LOSE)
            return;

        if (_isTutorialLevel)
        {
            GameHelper.Instance.InvokeAction(GameConstants.OnTimerUpdate, 0);
            return;
        }

        if (_levelTimeRemaining > 0f)
        {
            _levelTimeRemaining -= Time.deltaTime;
            GameHelper.Instance.InvokeAction(GameConstants.OnTimerUpdate, (int)_levelTimeRemaining);
        }
        else if (_gameEndType != GameEndType.LOSE)
        {
            HandleTimeUp();
        }
    }

    private void HandleTimeUp()
    {
        _isGameContinue = false;
        _isGameOver = true;
        _gameEndType = GameEndType.LOSE;

        bool hasAllColored = nmCellList.All(x => x.HasCellColored());
        bool hasAllMatched = nmCellList.All(x => x.HasColorMatched());

        if (hasAllColored && !hasAllMatched)
        {
            _gameLoseType = GameLoseType.WRONGPLAY;
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.RESULT, new object[] { _gameEndType, _gameLoseType } });
        }
        else
        {
            _gameLoseType = GameLoseType.TIMEUP;

            if (HasEnoughCoinForLevelReplay()) 
            {
                InitiatePreGameEnd();
            }
            else
            {
                InitiateGameEnd();
            }
        } 
    }

    private void InitiateGameEnd()
    {
        _isGameContinue = false;
        _isGameOver = true;

        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.RESULT, new object[] 
        {
            _gameEndType,
            _gameLoseType
        } });
    }

    private void InitiatePreGameEnd()
    {
        if(GameConstants.LevelLoseLifeCount == 0)
        {
            InitiateGameEnd();
        }
        else
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.PRESULT, new object[] 
            {
                new PreResultData
                {
                    remainingTime = _gameSettings.levelFailContinueTime
                }
            } });
            GameConstants.LevelLoseLifeCount = 0;
        }            
    }

    public void CellMovingStatus(bool isMove)
    {
        _isCellMoving = isMove;

        if(!_isTutorialLevel) GameHelper.Instance.InvokeAction(GameConstants.UndoAvailabilityChanged, CanUndo());

        if (_isTutorialLevel)
        {
            if(_handPointer != null)
            {
                if (isMove)
                {
                    _handPointer.SetVisibility(false);
                }
                else
                {
                    _handPointer.SetVisibility(true);
                }
            }
        }
    }

    public void AddNmCellsToList(NonMovableCell nmCell)
    {
        nmCellList.Add(nmCell);        
    }

    public void AddMCellsToList(MovableCell mCell)
    {
        mCellList.Add(mCell);
    }

    private IEnumerator TutorialCoroutine()
    {
        while (_isTutorialLevel)
        {
            yield return null;

            _tutorialStep = 1;

            yield return new WaitUntil(() => _tutorialStep == 1);
            MovableCell c1 = null;
            foreach (var c in mCellList)
            {
                if (c.GetIndex() == _gameSettings.tutorialDataList[0].cellIndex)
                {
                    c1 = c;
                    c1.SetInteractibility(true);
                }
                else
                {
                    c.SetInteractibility(false);
                }
            }
            HandleHandPointer(_tutorialStep);
            yield return new WaitUntil(() => !c1.IsMovable());
            _tutorialStep = 2;

            yield return new WaitUntil(() => _tutorialStep == 2);
            MovableCell c2 = null;
            foreach (var c in mCellList)
            {
                if (c.GetIndex() == _gameSettings.tutorialDataList[1].cellIndex)
                {
                    c2 = c;
                    c2.SetInteractibility(true);
                }
                else
                {
                    c.SetInteractibility(false);
                }
            }
            HandleHandPointer(_tutorialStep);
            yield return new WaitUntil(() => !c2.IsMovable());
            yield return null;
            Debug.Log("Tutorial completed!");
            yield break;
        }
    }

    private void HandleHandPointer(int tutStep)
    {
        _handPointer.SetPosition(_gameSettings.tutorialDataList[tutStep - 1].handPos);
    }

    private void CellColorCompletion(object obj)
    {
        bool hasAllCellColored = nmCellList.All(x => x.HasCellColored());
        bool hasAllCellColorMatched = nmCellList.All(x => x.HasColorMatched());
        bool hasRemainingMovableCell = mCellList.Any(x => x.IsMovable());

        if (hasAllCellColored)
        {
            if (hasAllCellColorMatched)
            {
                //win
                _gameEndType = GameEndType.WIN;
                _gameLoseType = GameLoseType.NONE;

                InitiateGameEnd();
            }
            else
            {
                if (!hasRemainingMovableCell)
                {
                    _gameEndType = GameEndType.LOSE;
                    _gameLoseType = GameLoseType.WRONGPLAY;

                    InitiateGameEnd();
                }
                else
                {
                    //game not end yet
                }
            }
        }
        else
        {
            //level not completed yet. Still cells left to color
            return;
        }
    }

    private bool HasEnoughCoinForLevelReplay()
    {
        return PlayerDataHandler.Player.GameCurrency.Coins >= _gameSettings.levelFailContinueCoin;
    }

    public void DetermineGameEnd(GameEndType gameEndType, GameLoseType gameLoseType)
    {
        if(gameEndType == GameEndType.WIN)
        {
            StartCoroutine(DelayGameEnd(true));
        }
        else if(gameEndType == GameEndType.LOSE)
        {
            StartCoroutine(DelayGameEnd(false));
        }
    }

    public void StartUndoRecording(MovableCell cell)
    {
        _lastUndoData = new UndoData();
        _lastUndoData.movableCell = cell;
        _lastUndoData.startPos = cell.transform.position;

        if (!_isTutorialLevel) GameHelper.Instance.InvokeAction(GameConstants.UndoAvailabilityChanged, true);
    }

    public void RecordCellState(NonMovableCell cell)
    {
        if (_lastUndoData == null) return;

        // Avoid duplicate entries
        if (_lastUndoData.affectedCells.Contains(cell))
            return;

        _lastUndoData.affectedCells.Add(cell);
        _lastUndoData.previousColors.Add(cell.AppliedColorCode);
    }

    private void UndoLastMove(object obj)
    {
        if (_lastUndoData == null) return;

        //Move cat back
        MovableCell cell = _lastUndoData.movableCell;
        cell.transform.position = _lastUndoData.startPos;

        // Reset movement state
        cell.ResetMovement();

        //Restore tiles
        for (int i = 0; i < _lastUndoData.affectedCells.Count; i++)
        {
            NonMovableCell nmCell = _lastUndoData.affectedCells[i];
            ColorCode prevColor = _lastUndoData.previousColors[i];

            nmCell.ResetColor(prevColor);
        }

        _lastUndoData = null;

        if (!_isTutorialLevel) GameHelper.Instance.InvokeAction(GameConstants.UndoAvailabilityChanged, false);        
    }

    public bool CanUndo()
    {
        return _lastUndoData != null && !_isCellMoving;
    }

    private void HandleGameplayRestart(object obj)
    {
        _isGameRestart = (bool)obj;
    }

    private void HandleGameplayPauseStatus(object obj)
    {
        bool isPause = (bool)obj;
        Action pauseCallback = () => _isGamePause = isPause;
        if (isPause)
        {
            pauseCallback();
        }
        else
        {
            StartCoroutine(DelayToResume(pauseCallback));
        }
    }

    private IEnumerator DelayToResume(Action pauseStateCallback)
    {
        yield return null;
        pauseStateCallback();
    }

    private IEnumerator DelayGameEnd(bool hasWon)
    {
        _isGameOver = true;

        yield return new WaitForSeconds(hasWon ? _gameSettings.gameWinDelay : _gameSettings.gameLoseDelay);
        if (hasWon)
        {
            _popupHandler.ShowPopup<WinPopup>(true, null, new object[] { _currentLevel, _currentThemeId, _levelTimeRemaining });
        }
        else
        {
            _popupHandler.ShowPopup<LosePopup>(true, null, new object[] { _currentLevel, _currentThemeId});
        }
    }    

    public void Cleanup()
    {
        if (nmCellList.Count > 0) nmCellList.Clear();
        if (mCellList.Count > 0) mCellList.Clear();       

        _isGameContinue = false;
        _isCellMoving = false;
        _isGameOver = false;
        _isGamePause = false;
        _isGameRestart = false;

        _gameEndType = GameEndType.NONE;
        _gameLoseType = GameLoseType.NONE;        

        if (_handPointer != null && _isTutorialLevel)
            Destroy(_handPointer.gameObject); _handPointer = null; _isTutorialLevel = false; _tutorialStep = 0;

        GameHelper.Instance.StopListening(GameConstants.GameplayPause, HandleGameplayPauseStatus);
        GameHelper.Instance.StopListening(GameConstants.GameplayRestart, HandleGameplayRestart);
        GameHelper.Instance.StopListening(GameConstants.CellColorCompletion, CellColorCompletion);
        GameHelper.Instance.StopListening(GameConstants.UndoMovableCell, UndoLastMove);
    }
}
