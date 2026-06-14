using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : UiScreenBase
{
    [SerializeField] private TMP_Text levelNoText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text targetText;

    [SerializeField] private Image header;
    [SerializeField] private Image levelHolder;
    [SerializeField] private Image coinHolder;
    [SerializeField] private Image targetGridBg;
    [SerializeField] private Image pause;
    [SerializeField] private Image undo;
    [SerializeField] private Image gameBg;
    [SerializeField] private Image gameBgOverlay;

    [SerializeField] private Button undoButton;
    [SerializeField] private ButtonEffect pauseButton;
    [SerializeField] private UIAnimator uiAnim;

    private int _gameThemeId;
    private int _currentLevelId;
    private GameThemeData _gameThemeData;
    private GameThemeData.GameTheme _gameTheme;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();

        GameHelper.Instance.StartListening(GameConstants.OnTimerUpdate, UpdateTimer);
        GameHelper.Instance.StartListening(GameConstants.CoinAmountUpdated, UpdateCoins);
        GameHelper.Instance.StartListening(GameConstants.UndoAvailabilityChanged, UpdateUndoButton);
        GameHelper.Instance.StartListening(GameConstants.TutorialStep, UpdateTutorialStep);
    }    

    internal override void Cleanup()
    {
        GameHelper.Instance.StopListening(GameConstants.OnTimerUpdate, UpdateTimer);
        GameHelper.Instance.StopListening(GameConstants.CoinAmountUpdated, UpdateCoins);
        GameHelper.Instance.StopListening(GameConstants.UndoAvailabilityChanged, UpdateUndoButton);
        GameHelper.Instance.StopListening(GameConstants.TutorialStep, UpdateTutorialStep);
    }

    internal override void HandleGameStateChangeData(object[] data)
    {
        object[] dataObjects = (object[])data[0];

        if(dataObjects.Length > 0)
        {
            if ((bool)dataObjects[0])
                return;

            _currentLevelId = (int)dataObjects[1];
            _gameThemeId = (int)dataObjects[2];

            levelNoText.text = (_currentLevelId + 1).ToString();
            _gameTheme = _gameThemeData.GetGameTheme(_gameThemeId);

            UpdateGameBoardStyle();
        }
        UpdateCoins(PlayerDataHandler.Player.GameCurrency.Coins);
        UpdateUndoButton(false);
        uiAnim.Play();
    }

    private void UpdateCoins(object obj)
    {
        coinText.text = ((long)obj).ToString();
    }

    private void UpdateTimer(object obj)
    {
        int totalSeconds = (int)obj;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateUndoButton(object obj)
    {
        undoButton.interactable = (bool)obj;
    }

    private void UpdateTutorialStep(object obj)
    {
        object[] dataObjects = obj as object[];
        int step = (int)dataObjects[1];
    }

    private void UpdateGameBoardStyle()
    {
        header.sprite = _gameTheme.header;
        levelHolder.sprite = _gameTheme.levelHolder;
        coinHolder.sprite = _gameTheme.coinHolder;
        targetGridBg.sprite = _gameTheme.targetGridBg;
        pause.sprite = _gameTheme.pause;
        undo.sprite = _gameTheme.undo;
        gameBg.sprite = _gameTheme.gameBg;
        gameBgOverlay.sprite = _gameTheme.gameBgOverlay;

        SetFontAsset(_gameTheme.fontAsset);
    }
    
    private void SetFontAsset(TMP_FontAsset asset)
    {
        levelNoText.font = asset;
        levelText.font = asset;
        timerText.font = asset;
        coinText.font = asset;
        targetText.font = asset;
    }

    public void UndoMove()
    {
        GameHelper.Instance.InvokeAction(GameConstants.UndoMovableCell);
    }

    public void Pause()
    {
        pauseButton.PlayEffect(() => 
        {
            _popupHandler.ShowPopup<PausePopup>(true, null, new object[] { _currentLevelId, _gameThemeId });
        });        
    }    
}