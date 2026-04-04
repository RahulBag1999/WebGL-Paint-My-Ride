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

    public Action OnCellColorCompletion;

    private List<NonMovableCell> nmCellList = new List<NonMovableCell>();
    private List<MovableCell> mCellList = new List<MovableCell>();

    private bool isCellMoving = false;
    private bool isGameOver = false;
    public bool IsCellMoving => isCellMoving;
    public bool IsGameOver => isGameOver;

    private GameSettings gameSettings;
    private LevelContainer levelContainer;
    private GameThemeData gameThemeData;

    private int currentLevel;
    private int currentThemeId = -1;    

    public  void Init(PopupHandler popupHandler, GridGenerator gridGenerator, EssentialConfigData essentialConfigData)
    {
        _gridGenerator = gridGenerator;
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;

        gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
        gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
        levelContainer = _essentialConfigData.AccessConfig<LevelContainer>();
    }

    private void OnEnable()
    {
        OnCellColorCompletion += CellColorCompletion;
    }

    private void OnDisable()
    {
        OnCellColorCompletion -= CellColorCompletion;
    }

    public void InitiateGameplay()
    {
        if (nmCellList.Count > 0) nmCellList.Clear();
        if (mCellList.Count > 0) mCellList.Clear();

        isGameOver = false;
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
            // UiController.Instance.ShowScreen<WinScreen>();
            IncrementLevel();
        }
        else
        {
            // UiController.Instance.ShowScreen<LoseScreen>();
        }
    }

    private void SetGameTheme()
    {
        //currentThemeId = GetRandomNumber(0, gameThemeData.gameThemeList.Count);
        //_gridGenerator.SetGridBg(gameThemeData.GetGameTheme(currentThemeId).gridBg);
    }

    private void IncrementLevel()
    {
        //PlayerPrefs.SetInt(GameConstants.LEVEL_ID, (currentLevel += 1));
        //currentLevel = PlayerPrefs.GetInt(GameConstants.LEVEL_ID, 1);
        //GameConstants.CURRENT_LEVEL_CONFIG = GetLevelConfig(currentLevel);
    }   

    public int GetRandomNumber(int minInclusive, int maxExclusive)
    {
        int newNumber;

        // Safety check (optional but recommended)
        if (maxExclusive - minInclusive <= 1)
        {
            Debug.LogWarning("Range too small to avoid repetition.");
            return minInclusive;
        }

        do
        {
            newNumber = UnityEngine.Random.Range(minInclusive, maxExclusive);
        }
        while (newNumber == currentThemeId);

        currentThemeId = newNumber;
        return newNumber;
    }
}
