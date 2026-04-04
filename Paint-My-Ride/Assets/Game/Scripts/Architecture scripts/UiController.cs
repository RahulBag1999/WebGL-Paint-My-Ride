using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour, IController
{
    [SerializeField] private List<UiScreenBase> _uiScreens = new List<UiScreenBase>();

    private Action<GameStates, object> _stateChanged;
    private Dictionary<Type, UiScreenBase> _uiScreenCollection = new Dictionary<Type, UiScreenBase>();
    private UiScreenBase _currentScreen;
    bool _isGameOn;

    private PopupHandler _popupHandler;

    public void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData, Action<GameStates, object> stateChanged)
    {
        _isGameOn = false;
        _popupHandler = popupHandler;
        _stateChanged = stateChanged;
        for (int i = 0; i < _uiScreens.Count; i++)
        {
            _uiScreens[i].Init(_popupHandler, essentialConfigData);
            _uiScreenCollection.Add(_uiScreens[i].GetType(), _uiScreens[i]);
        }
        _currentScreen = null;
    }

    internal void ToggleScreen<T>(params object[] data) where T : UiScreenBase
    {
        if (!Equals(_currentScreen, null))
        {
            //Debug.Log("Previous screen type:: " + _currentScreen.GetType().FullName);
            _currentScreen.SetScreenCanvasVisibility(false);
        }
        _currentScreen = _uiScreenCollection[typeof(T)];
        Debug.Log("Current screen type:: " + _currentScreen.GetType().FullName);
        _currentScreen.SetScreenCanvasVisibility(true);
        if (data.Length > 0)
        {
            _currentScreen.HandleGameStateChangeData(data);
        }
    }

    public void ChangeGameState(GameStates newGameState, object data = null)
    {
        switch (newGameState)
        {
            case GameStates.SPLASH:
                ToggleScreen<SplashScreen>(data);
                break;

            case GameStates.LOADING:
                ToggleScreen<LoadingScreen>(data);
                break;

            case GameStates.HOME:
                ToggleScreen<HomeScreen>(data);
                break;

            case GameStates.GAMEPLAY:
                ToggleScreen<GameplayScreen>(data);
                break;

            case GameStates.RESULT:
                break;
        }
    }

    public void Cleanup()
    {
        for (int i = 0; i < _uiScreens.Count; i++)
        {
            _uiScreens[i].Cleanup();
        }
    }

    public void RegisterInputs()
    {

    }

    public void UnregisterInputs()
    {

    }
}
