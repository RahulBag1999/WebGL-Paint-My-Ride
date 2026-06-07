using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : UiScreenBase
{
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _coinText;

    [SerializeField] private Canvas _homeBgCanvas;
    [SerializeField] private Button _playButton;
    [SerializeField] private MultiSpriteAnimator _msa;

    private GameThemeData _gameThemeData;

    private int _currentLevel;
    private int _currentThemeId = -1;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);

        GameHelper.Instance.StartListening(GameConstants.CoinAmountUpdated, UpdateCoins);

        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();

        UpdateHomeBgCanvas(false);
    }    
    
    internal override void HandleGameStateChangeData(object[] data)
    {
        int currentLevel = PlayerDataHandler.Player.GameplayProgress.MaxUnlockedLevelId;

        _currentLevel = currentLevel;

        _levelText.text = $"Level {_currentLevel + 1}";
        UpdateCoins(PlayerDataHandler.Player.GameCurrency.Coins);

        SetGameTheme();
        UpdateHomeBgCanvas(true);

        _msa.Play("HomeCat", "Idle");
    }

    private void UpdateHomeBgCanvas(bool isEnabled)
    {
        _homeBgCanvas.enabled = isEnabled;
    }

    public void PlayGame()
    {
        Action changeGameStateAction = () =>
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[]
            {
                false,
                _currentLevel,
                _currentThemeId
            } });
        };     

        TransitionHelper.Instance.Play(
            () => 
            {
                _playButton.interactable = false;
            }, 
            () => 
            {
                UpdateHomeBgCanvas(false);
                changeGameStateAction?.Invoke();
            }, 
            () => 
            {
                _msa.Stop("HomeCat");
                _playButton.interactable = true;
            });
    }

    private void UpdateCoins(object obj)
    {
        _coinText.text = obj.ToString();
    }    

    private void SetGameTheme()
    {
        _currentThemeId = Utility.GetRandomNumber(_currentThemeId, 0, _gameThemeData.gameThemeList.Count);
    }

    internal override void Cleanup()
    {
        GameHelper.Instance.StopListening(GameConstants.CoinAmountUpdated, UpdateCoins);
    }
}