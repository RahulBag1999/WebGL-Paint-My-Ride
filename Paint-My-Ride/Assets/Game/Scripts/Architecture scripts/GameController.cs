using System;
using UnityEngine;

public class GameController : MonoBehaviour, IController
{
    [SerializeField] private GameplayHelper _gameplayHelper;
    [SerializeField] private GridGenerator _gridGenerator;
    [SerializeField] private AudioHandler _audioHandler;
    [SerializeField] private CloudManager _cloudManager;

    private PopupHandler _popupHandler;
    private EssentialConfigData _essentialConfigData;

    private Action<GameStates, object> _stateChanged;

    public void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData, Action<GameStates, object> stateChanged)
    {
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;
        _stateChanged = stateChanged;

        _audioHandler.Init(_essentialConfigData);
        _gridGenerator.Init(_gameplayHelper, _essentialConfigData);
        _gameplayHelper.Init(_popupHandler, _essentialConfigData);
    }

    public void ChangeGameState(GameStates newGameState, object data = null)
    {
        switch (newGameState)
        {
            case GameStates.SPLASH:
                break;

            case GameStates.LOADING:
                break;

            case GameStates.HOME:
                _audioHandler.HandleMusicState(PlayerDataHandler.Player.UserSettingsPreferences.MusicState);
                _audioHandler.HandleSfxState(PlayerDataHandler.Player.UserSettingsPreferences.SfxState);

                GameHelper.Instance.InvokeAction(GameConstants.PlayAudio, "Background");

                _gridGenerator.Cleanup();
                _gameplayHelper.Cleanup();

                _cloudManager.UpdateView(true);
                break;

            case GameStates.GAMEPLAY:
                _cloudManager.UpdateView(false);
                object[] dataObjects = (object[])data;

                if ((bool)dataObjects[0])
                {
                    _gameplayHelper.InitiateGameplay((PreResultData)dataObjects[1]);
                }
                else
                {
                    _gameplayHelper.Cleanup();
                    _gameplayHelper.InitiateGameplay((int)dataObjects[1], (int)dataObjects[2]);
                    _gridGenerator.SetGridBg((int)dataObjects[2]);
                    _gridGenerator.GenerateAllGrid(GameConstants.CurrentLevelConfig);
                }                
                break;

            case GameStates.RESULT:
                object[] resultObjects = (object[])data;

                _gameplayHelper.DetermineGameEnd((GameEndType)resultObjects[0], (GameLoseType)resultObjects[1]);
                _gameplayHelper.Cleanup();
                break;

            case GameStates.PRESULT:
                object[] resultDataObjects = (object[])data;
                _popupHandler.ShowPopup<TimeUpPopup>(true, null, resultDataObjects[0]);
                break;
        }
    }

    public void RegisterInputs()
    {

    }

    public void UnregisterInputs()
    {

    }

    public void Cleanup()
    {
        _audioHandler.Cleanup();
    }
}
