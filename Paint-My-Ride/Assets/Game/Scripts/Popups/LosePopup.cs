using System;
using UnityEngine;

public class LosePopup : UiPopupBase
{
    private GameThemeData _gameThemeData;
    private int _currentLevel;
    private int _currentTheme;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
    }

    internal override void Cleanup()
    {
        
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        if (isView)
        {
            if (data.Length > 0)
            {
                _currentLevel = (int)data[0];
                _currentTheme = (int)data[1];
            }
            GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Lose");
        }
    }

    public void TryAgain()
    {
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[]
            {
                false,
                _currentLevel,
                _currentTheme
            }});
        };

        SetGameTheme();
        _popupHandler.HidePopup(() => { }, () => 
        {
            ScreenTransition.Instance.Play(
                OnStarted => { }, 
                OnPartial => 
                {
                    OnChangeGameState?.Invoke();
                }, 
                OnCompleted => { }
            );
        });        
    }

    public void Home()
    {
        Action OnChangeGameState = () =>
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
        };
        _popupHandler.HidePopup(() => { }, () => 
        {
            ScreenTransition.Instance.Play(
                OnStarted => { }, 
                OnPartial => 
                {
                    OnChangeGameState?.Invoke();
                }, 
                OnCompleted => { }
            );
        });
    }

    private void SetGameTheme()
    {
        _currentTheme = Utility.GetRandomNumber(_currentTheme, 0, _gameThemeData.gameThemeList.Count);
    }
}
