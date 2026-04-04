public class HomeScreen : UiScreenBase
{
    private LevelContainer _levelContainer;

    private int _currentLevel;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);

        _levelContainer = _essentialConfigData.AccessConfig<LevelContainer>();
    }

    internal override void Cleanup()
    {

    }
    private LevelConfig _levelConfig;
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
    }

    public void PlayGame()
    {
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, /*new object[] { _currentLevel }*/null });
    }

    private LevelConfig FetchLastLevelConfig(int index)
    {
        return _levelContainer.GetLevelConfig(index);
    }
}