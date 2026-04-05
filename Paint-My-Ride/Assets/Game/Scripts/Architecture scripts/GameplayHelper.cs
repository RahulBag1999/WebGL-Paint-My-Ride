using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayHelper : MonoBehaviour
{
    private GridGenerator _gridGenerator;
    private PopupHandler _popupHandler;
    private EssentialConfigData _essentialConfigData;

    private List<NonMovableCell> nmCellList = new List<NonMovableCell>();
    private List<MovableCell> mCellList = new List<MovableCell>();

    private bool _isCellMoving = false;
    private bool _isGameOver = false;
    private bool _isGamePause = false;
    public bool IsCellMoving => _isCellMoving;
    public bool IsGameOver => _isGameOver;

    private GameSettings _gameSettings;
    private LevelContainer _levelContainer;
    private LevelTimingData _levelTimingData;

    private int _currentLevel = 0;    
    private float _levelTimeRemaining;

    private bool _isGameContinue = false;
    private GameLoseType _gameLoseType = GameLoseType.NONE;
    private GameEndType _gameEndType = GameEndType.NONE;

    public bool IsGameContinue => _isGameContinue;

    public  void Init(PopupHandler popupHandler, GridGenerator gridGenerator, EssentialConfigData essentialConfigData)
    {
        _gridGenerator = gridGenerator;
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

    public void InitiateGameplay(int level)
    {
        _isGameOver = false;

        _currentLevel = level;
        _levelTimeRemaining = _levelTimingData.GetTimeForLevel(_currentLevel);
        _isGameContinue = true;

        GameHelper.Instance.StartListening(GameConstants.GameplayPause, HandleGameplayPauseStatus);
        GameHelper.Instance.StartListening(GameConstants.CellColorCompletion, CellColorCompletion);
    }

    private void UpdateGameplay()
    {
        if (_isGamePause)
            return;

        if (_gameEndType == GameEndType.WIN || _gameEndType == GameEndType.LOSE)
            return;

        if (_levelTimeRemaining > 0f)
        {
            _levelTimeRemaining -= Time.deltaTime;
            GameHelper.Instance.InvokeAction(GameConstants.OnTimerUpdate, (int)_levelTimeRemaining);
        }
        else
        {
            HandleTimeUp();
        }
    }

    private void HandleTimeUp()
    {
        if (_isCellMoving)
        {
            if (_gameEndType == GameEndType.WIN)
            {
                return;
            }
            else if (_gameEndType == GameEndType.LOSE)
            {
                _gameLoseType = GameLoseType.WRONGPLAY;
            }
        }
        else
        {
            _gameLoseType = GameLoseType.TIMEUP;
        }
        _isGameContinue = false;
    }    

    public void CellMovingStatus(bool isMove)
    {
        _isCellMoving = isMove;
    }

    public void AddNmCellsToList(NonMovableCell nmCell)
    {
        nmCellList.Add(nmCell);
    }

    public void AddMCellsToList(MovableCell mCell)
    {
        mCellList.Add(mCell);
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
                StartCoroutine(DelayGameEnd(true));
            }
            else
            {
                if (!hasRemainingMovableCell)
                {
                    _gameEndType = GameEndType.LOSE;
                    StartCoroutine(DelayGameEnd(false));
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
            _popupHandler.ShowPopup<WinPopup>(true);
        }
        else
        {
            _popupHandler.ShowPopup<LosePopup>(true);
        }
    }    

    private void IncrementLevel()
    {
        //PlayerPrefs.SetInt(GameConstants.LEVEL_ID, (currentLevel += 1));
        //currentLevel = PlayerPrefs.GetInt(GameConstants.LEVEL_ID, 1);
        //GameConstants.CURRENT_LEVEL_CONFIG = GetLevelConfig(currentLevel);
    }

    public void Cleanup()
    {
        if (nmCellList.Count > 0) nmCellList.Clear();
        if (mCellList.Count > 0) mCellList.Clear();

        _isCellMoving = false;
        _isGameOver = false;
        _isGamePause = false;

        _gameEndType = GameEndType.NONE;

        GameHelper.Instance.StopListening(GameConstants.GameplayPause, HandleGameplayPauseStatus);
        GameHelper.Instance.StopListening(GameConstants.CellColorCompletion, CellColorCompletion);
    }
}
