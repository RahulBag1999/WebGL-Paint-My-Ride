internal static class GameConstants
{
    internal static GameStates CurrentGameState = GameStates.NONE;
    internal static LevelConfig CurrentLevelConfig;
    internal static int CurrentGameThemeId = 0;

    #region Action_Keys
    internal const string ChangeGameState = nameof(ChangeGameState); 
    internal const string OnTimerUpdate = nameof(OnTimerUpdate);
    internal const string CoinAmountUpdated = nameof(CoinAmountUpdated);
    internal const string GameplayPause = nameof(GameplayPause);
    internal const string CellColorCompletion = nameof(CellColorCompletion);
    #endregion Action_Keys
}