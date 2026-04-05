using TMPro;
using UnityEngine;

public class HomeScreen : UiScreenBase
{
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _coinText;

    private LevelContainer _levelContainer;
    private GameThemeData _gameThemeData;
    private LevelConfig _levelConfig;

    private int _currentLevel;
    private int _currentThemeId = -1;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);

        GameHelper.Instance.StartListening(GameConstants.CoinAmountUpdated, UpdateCoins);

        _levelContainer = _essentialConfigData.AccessConfig<LevelContainer>();
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
    }    
    
    internal override void HandleGameStateChangeData(object[] data)
    {
        int currentLevel = PlayerDataHandler.Player.GameplayProgress.MaxUnlockedLevelId;

        _levelConfig = FetchLastLevelConfig(currentLevel);

        if (_levelConfig == null)
        {
            return;
        }

        _currentLevel = currentLevel;
        GameConstants.CurrentLevelConfig = _levelConfig;

        _levelText.text = $"Level {_currentLevel + 1}";
        UpdateCoins(PlayerDataHandler.Player.GameCurrency.Coins);

        SetGameTheme();
    }

    public void PlayGame()
    {
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[] { _currentLevel, _currentThemeId } });
    }

    private void UpdateCoins(object obj)
    {
        _coinText.text = obj.ToString();
    }

    private LevelConfig FetchLastLevelConfig(int index)
    {
        return _levelContainer.GetLevelConfig(index);
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