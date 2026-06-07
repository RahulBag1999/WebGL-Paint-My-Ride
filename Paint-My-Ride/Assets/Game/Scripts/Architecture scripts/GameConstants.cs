internal static class GameConstants
{
    internal const int COINS_WIN = 20;
    internal const string MUSIC_KEY = "Music_Volume";
    internal const string SFX_KEY = "SFX_Volume";

    internal const string BASE_LOADING_TEXT = "Loading";

    internal static GameStates CurrentGameState = GameStates.NONE;
    internal static LevelConfig CurrentLevelConfig;
    internal static int CurrentGameThemeId = 0;
    internal static byte LevelLoseLifeCount = 1;
    internal static bool IsLevelTutorial = false;

    #region Action_Keys
    internal const string ChangeGameState = nameof(ChangeGameState); 
    internal const string OnTimerUpdate = nameof(OnTimerUpdate);
    internal const string CoinAmountUpdated = nameof(CoinAmountUpdated);
    internal const string GameplayPause = nameof(GameplayPause);
    internal const string CellColorCompletion = nameof(CellColorCompletion);
    internal const string UndoMovableCell = nameof(UndoMovableCell);
    internal const string UndoAvailabilityChanged = nameof(UndoAvailabilityChanged);
    internal const string TutorialStep = nameof(TutorialStep);
    internal const string OnUpdateTutorialCanvas = nameof(OnUpdateTutorialCanvas);
    internal const string GameplayRestart = nameof(GameplayRestart);
    internal const string PlayAudio = nameof(PlayAudio);
    internal const string StopAudio = nameof(StopAudio);
    internal const string PlayAudioOneShot = nameof(PlayAudioOneShot);
    #endregion Action_Keys
}