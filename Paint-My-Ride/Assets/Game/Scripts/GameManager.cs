using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GridGenerator gridGenerator;

    public Action OnCellColorCompletion;

    private List<NonMovableCell> nmCellList = new List<NonMovableCell>();
    private List<MovableCell> mCellList = new List<MovableCell>();

    private bool isCellMoving = false;
    private bool isGameOver = false;
    public bool IsCellMoving => isCellMoving;
    public bool IsGameOver => isGameOver;

    private GameSettings gameSettings;
    private LevelContainer levelContainer;

    private int currentLevel;

    protected override void OnInit()
    {
        base.OnInit();
        gameSettings = Resources.Load<GameSettings>(nameof(GameSettings));
        levelContainer = Resources.Load<LevelContainer>(nameof(LevelContainer));
    }

    private void OnEnable()
    {
        OnCellColorCompletion += CellColorCompletion;
    }

    private void OnDisable()
    {
        OnCellColorCompletion -= CellColorCompletion;
    }

    private void Start()
    {
        UiController.Instance.ShowScreen<HomeScreen>();
    }

    public void StartGame()
    {
        if(nmCellList.Count > 0) nmCellList.Clear();
        if(mCellList.Count > 0) mCellList.Clear();

        isGameOver = false;
        currentLevel = PlayerPrefs.GetInt(GameConstants.LEVEL_ID, 1);

        if(currentLevel > 0)
        {
            GameConstants.CURRENT_LEVEL_CONFIG = GetLevelConfig(currentLevel);
            gridGenerator.GenerateAllGrid(GameConstants.CURRENT_LEVEL_CONFIG);
            UiController.Instance.ShowScreen<GameplayScreen>();
        }
    }

    public void CellMoving(bool isMove)
    {
        isCellMoving = isMove;
    }

    public void AddNmCellsToList(NonMovableCell nmCell)
    {
        nmCellList.Add(nmCell);
    }

    public void AddMCellsToList(MovableCell mCell)
    {
        mCellList.Add(mCell);
    }

    private void CellColorCompletion()
    {
        bool hasAllCellColored = nmCellList.All(x => x.HasCellColored());
        bool hasAllCellColorMatched = nmCellList.All(x => x.HasColorMatched());
        bool hasRemainingMovableCell = mCellList.Any(x => x.IsMovable());

        if (hasAllCellColored)
        {
            if (hasAllCellColorMatched)
            {
                //win
                StartCoroutine(DelayGameEnd(true));
            }        
            else
            {
                if (!hasRemainingMovableCell)
                {
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

    private IEnumerator DelayGameEnd(bool hasWon)
    {
        isGameOver = true;

        yield return new WaitForSeconds(hasWon ? gameSettings.gameWinDelay : gameSettings.gameLoseDelay);
        if (hasWon)
        {
            UiController.Instance.ShowScreen<WinScreen>();
            IncrementLevel();
        }
        else
        {
            UiController.Instance.ShowScreen<LoseScreen>();
        }
    }

    private void IncrementLevel()
    {
        PlayerPrefs.SetInt(GameConstants.LEVEL_ID, (currentLevel += 1));
        currentLevel = PlayerPrefs.GetInt(GameConstants.LEVEL_ID, 1);
        GameConstants.CURRENT_LEVEL_CONFIG = GetLevelConfig(currentLevel);
    }

    private LevelConfig GetLevelConfig(int lvlID)
    {
        return levelContainer.levelConfigs.Find(x => x.levelId == lvlID);
    }

    public void Home()
    {
        UiController.Instance.ShowScreen<HomeScreen>();
    }

    public void NextLevel()
    {
        StartGame();
    }
}
