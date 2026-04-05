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
    [SerializeField] private Image coinHolder;
    [SerializeField] private Image targetGridBg;
    [SerializeField] private Image pause;
    [SerializeField] private Image undo;
    [SerializeField] private Image gameBg;
    [SerializeField] private Image gameBgOverlay;

    [SerializeField] private Button undoButton;

    private int _gameThemeId;
    private GameThemeData _gameThemeData;
    private GameThemeData.GameTheme _gameTheme;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
        GameHelper.Instance.StartListening(GameConstants.OnTimerUpdate, UpdateTimer);
        GameHelper.Instance.StartListening(GameConstants.CoinAmountUpdated, UpdateCoins);
    }

    internal override void Cleanup()
    {
        GameHelper.Instance.StopListening(GameConstants.OnTimerUpdate, UpdateTimer);
        GameHelper.Instance.StopListening(GameConstants.CoinAmountUpdated, UpdateCoins);
    }

    internal override void HandleGameStateChangeData(object[] data)
    {
        object[] dataObjects = (object[])data[0];

        if(dataObjects.Length > 0)
        {
            levelNoText.text = (((int)dataObjects[0]) + 1).ToString();
            _gameThemeId = (int)dataObjects[1];

            _gameTheme = _gameThemeData.GetGameTheme(_gameThemeId);

            UpdateGameBoardStyle();
        }
        UpdateCoins(PlayerDataHandler.Player.GameCurrency.Coins);
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

    private void UpdateGameBoardStyle()
    {
        header.sprite = _gameTheme.header;
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

    public void Pause()
    {
        _popupHandler.ShowPopup<PausePopup>(true);
    }
}