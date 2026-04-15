using Newtonsoft.Json;
using System;

public sealed class PlayerSaveData
{
    [JsonProperty(PropertyName = "isFirstTime")]
    private bool _isFirstTime = true;

    [JsonProperty(PropertyName = "gameCurrency")]
    private GameCurrency _gameCurrency;

    [JsonProperty(PropertyName = "userSettingsPreferences")]
    private UserSettingsPreferences _userSettingsPreferences;

    [JsonProperty(PropertyName = "gameplayProgress")]
    private GameplayProgress _gameplayProgress;

    [JsonProperty(PropertyName = "lastUpdateTime")]
    private DateTime _lastUpdateTime;
    private TimeSpan _currentLoginTimeDiff;

    [JsonIgnore] public bool IsFirstTime => _isFirstTime;
    [JsonIgnore] public ref GameCurrency GameCurrency => ref _gameCurrency;
    [JsonIgnore] public ref UserSettingsPreferences UserSettingsPreferences => ref _userSettingsPreferences;
    [JsonIgnore] public ref GameplayProgress GameplayProgress => ref _gameplayProgress;
    [JsonIgnore] public DateTime LastUpdateTime => _lastUpdateTime;
    [JsonIgnore] public TimeSpan CurrentLoginTimeDiff => _currentLoginTimeDiff;

    public PlayerSaveData()
    {
        _gameCurrency = new GameCurrency();
        _gameplayProgress = new GameplayProgress();
        _userSettingsPreferences = new UserSettingsPreferences();
    }

    public void SetIsFirstTime(bool isFirstTime)
    {
        _isFirstTime = isFirstTime;
    }

    public void UpdateLastUpdateTime()
    {
        _lastUpdateTime = DateTime.UtcNow;
    }

    public void UpdateCurrentLoginTimeDiff(TimeSpan currentLoginTimeDiff)
    {
        _currentLoginTimeDiff = currentLoginTimeDiff;
    }
}

[Serializable]
public sealed class GameCurrency
{
    [JsonProperty(PropertyName = "coinCount")]
    private long _coinCount = 0;

    [JsonIgnore]
    public long Coins => _coinCount;

    /// <summary>
    /// Update the amount of cash the player has.
    /// </summary>
    /// <param name="coinCount">Amount of coin to be increased or deducted.
    /// A positive value will add cash.
    /// A negatve value will deduct cash.</param>
    public void UpdateCoin(long coinCount)
    {
        _coinCount += coinCount;
        if (_coinCount < 0)
        {
            _coinCount = 0;
        }
        GameHelper.Instance.InvokeAction(GameConstants.CoinAmountUpdated, _coinCount);
    }
}

[Serializable]
public sealed class UserSettingsPreferences
{
    [JsonProperty(PropertyName = "Music")]
    private bool _musicState = true;

    [JsonIgnore] public bool MusicState => _musicState;


    [JsonProperty(PropertyName = "SFX")]
    private bool _sfxState = true;

    [JsonIgnore] public bool SfxState => _sfxState;

    public void UpdateMusicStatus(bool state)
    {
        _musicState = state;
    }

    public void UpdateSFXStatus(bool state)
    {
        _sfxState = state;
    }
}

public sealed class GameplayProgress
{
    [JsonProperty(PropertyName = "TotalLevels")]
    private int _totalLevels = 0;

    [JsonProperty(PropertyName = "MaxUnlockedLevelId")]
    private int _maxUnlockedLevelId = 0;

    [JsonIgnore]
    public int MaxUnlockedLevelId => _maxUnlockedLevelId;

    public void UpdateTotalLevels(int totalLevels)
    {
        _totalLevels = totalLevels;
    }

    public void UpdateMaxUnlockedLevelId(int currentLevelId)
    {
        int nextLevel = currentLevelId + 1;

        if (nextLevel >= _totalLevels)
        {
            _maxUnlockedLevelId = 0;
        }
        else
        {
            if (nextLevel > _maxUnlockedLevelId)
            {
                _maxUnlockedLevelId = nextLevel;
            }
        }
    }
}
