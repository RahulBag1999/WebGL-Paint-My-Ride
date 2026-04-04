internal static class GameConstants
{
    internal static GameStates CurrentGameState = GameStates.NONE;
    internal static LevelConfig CurrentLevelConfig;

    #region Action_Keys
    internal const string ChangeGameState = "ChangeGameState";
    #endregion Action_Keys
}