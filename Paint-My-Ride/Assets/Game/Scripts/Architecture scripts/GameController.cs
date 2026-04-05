using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, IController
{
    [SerializeField] private GameplayHelper _gameplayHelper;
    [SerializeField] private GridGenerator _gridGenerator;

    private PopupHandler _popupHandler;
    private EssentialConfigData _essentialConfigData;

    private Action<GameStates, object> _stateChanged;

    public void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData, Action<GameStates, object> stateChanged)
    {
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;
        _stateChanged = stateChanged;

        _gridGenerator.Init(_gameplayHelper, _essentialConfigData);
        _gameplayHelper.Init(_popupHandler, _gridGenerator, _essentialConfigData);
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
                _gridGenerator.Cleanup();
                _gameplayHelper.Cleanup();
                break;

            case GameStates.GAMEPLAY:
                object[] dataObjects = (object[])data;

                _gameplayHelper.InitiateGameplay((int)dataObjects[0]);
                _gridGenerator.SetGridBg((int)dataObjects[1]);
                _gridGenerator.GenerateAllGrid(GameConstants.CurrentLevelConfig);
                break;

            case GameStates.RESULT:
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

    }
}
