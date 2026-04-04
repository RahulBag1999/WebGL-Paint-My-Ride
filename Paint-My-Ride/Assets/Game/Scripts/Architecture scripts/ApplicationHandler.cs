using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationHandler : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
    [SerializeField] private UiController _uiController;

    private PopupHandler _popupHandler;
    private EssentialConfigData _essentialConfigData;

    private Action<GameStates, object> _stateChanged;
    private List<IController> _controllers = new List<IController>();

    internal void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        _stateChanged = OnGameStateChanged;
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;
        AddController(_gameController);
        AddController(_uiController);
        _stateChanged?.Invoke(GameStates.SPLASH, null);
        GameHelper.Instance.StartListening(GameConstants.ChangeGameState, HandleGameStateChange);

        Application.targetFrameRate = 60;
    }
   
    private void OnGameStateChanged(GameStates gameState, object data)
    {
        if (!Equals(GameConstants.CurrentGameState, GameStates.GAMEPLAY) && Equals(GameConstants.CurrentGameState, gameState))
        {
            return;
        }
        GameConstants.CurrentGameState = gameState;
        Debug.Log("Game State: " + GameConstants.CurrentGameState.ToString());
        for (int i = 0; i < _controllers.Count; i++)
        {
            _controllers[i].ChangeGameState(gameState, data);
            if (Equals(gameState, GameStates.QUIT))
            {
                _controllers[i].Cleanup();
                Cleanup();
            }
        }
    }

    private void AddController(IController controller)
    {
        _controllers.Add(controller);
        controller.Init(_popupHandler, _essentialConfigData, _stateChanged);
    }

    private void HandleGameStateChange(object data)
    {
        object[] objects = data as object[];
        GameStates newState = (GameStates)objects[0];
        object additionalData = objects[1];
        _stateChanged.Invoke(newState, additionalData);
    }

    private void Cleanup()
    {
        GameHelper.Instance.StopListening(GameConstants.ChangeGameState, HandleGameStateChange);
        _essentialConfigData.Cleanup();
        GameHelper.CleanUpResources();
    }
}