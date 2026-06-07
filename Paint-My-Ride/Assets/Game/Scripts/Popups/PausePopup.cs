using UnityEngine;
using System;

public class PausePopup : UiPopupBase
{
    [SerializeField] private SleepZAnimator _sleepAnimator;

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
            if(data.Length > 0)
            {
                _currentLevel = (int)data[0];
                _currentTheme = (int)data[1];
            }
            GameHelper.Instance.InvokeAction(GameConstants.GameplayPause, true);

            _sleepAnimator.Play();
        }
        else
        {
            GameHelper.Instance.InvokeAction(GameConstants.GameplayPause, false);
        }
    }   

    public void Restart()
    {
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[]
            {
                false,
                _currentLevel,
                _currentTheme
            } });
        };

        SetGameTheme();
        _popupHandler.HidePopup(() => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.GameplayRestart, true);
        }, 
        () => 
        {
            TransitionHelper.Instance.Play(
            () =>
            {
                GameHelper.Instance.InvokeAction(GameConstants.GameplayRestart, true);
            },
            () =>
            {
                OnChangeGameState?.Invoke();
            },
            () =>
            {
                GameHelper.Instance.InvokeAction(GameConstants.GameplayRestart, false);
                _sleepAnimator.Stop();
            });
        });        
    }

    private void SetGameTheme()
    {
        _currentTheme = Utility.GetRandomNumber(_currentTheme, 0, _gameThemeData.gameThemeList.Count);
    }

    public void Home()
    {
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.OnUpdateTutorialCanvas, false);
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
        };
        _popupHandler.HidePopup(
            () =>
            {
                GameHelper.Instance.InvokeAction(GameConstants.GameplayRestart, true);
            },
            () =>
            {
                TransitionHelper.Instance.Play(
                () =>
                {
                    
                },
                () =>
                {
                    OnChangeGameState?.Invoke();
                },
                () =>
                {
                    _sleepAnimator.Stop();
                });
            });
    }

    public void MusicToggle(bool state)
    {
        if (state) 
        {
            Debug.Log("Music on");
        }
        else
        {
            Debug.Log("Music off");
        }
        AudioHandler.Instance.HandleMusicState(state);
    }

    public void SoundToggle(bool state)
    {
        if (state)
        {
            Debug.Log("Sound on");            
        }
        else
        {
            Debug.Log("Sound off");
        }
        AudioHandler.Instance.HandleSfxState(state);
    }

    public void Close()
    {
        _popupHandler.HidePopup(
            () => 
            {

            }, 
            () => 
            {
                if (GameConstants.IsLevelTutorial)
                {
                    GameHelper.Instance.InvokeAction(GameConstants.OnUpdateTutorialCanvas, true);
                }
            });
    }
}