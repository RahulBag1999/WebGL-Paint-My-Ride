using System;
using TMPro;
using UnityEngine;

public class LosePopup : UiPopupBase
{
    private GameThemeData _gameThemeData;
    private int _currentLevel;
    private int _currentTheme;

    [SerializeField] private MultiSpriteAnimator _anim;
    [SerializeField] private TMP_Text _headerCoinText;

    [SerializeField] private ButtonEffect _homeButton;
    [SerializeField] private ButtonEffect _replayButton;

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

                _headerCoinText.text = PlayerDataHandler.Player.GameCurrency.Coins.ToString();
            }
            GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Lose");

            _anim.Play("Cat", "Idle");
        }
    }

    public void Replay()
    {
        SetGameTheme();
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[]
            {
                false,
                _currentLevel,
                _currentTheme
            }});
        };

        _replayButton.PlayEffect(() => 
        {
            _popupHandler.HidePopup(() =>
            {

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

                });

                _anim.Stop("Cat");
            });
            }); 
    }

    public void Home()
    {
        Action OnChangeGameState = () =>
        {
            GameHelper.Instance.InvokeAction(GameConstants.OnUpdateTutorialCanvas, false);
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
        };

        _homeButton.PlayEffect(() => 
        {
            _popupHandler.HidePopup(() =>
            {

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

                });
                _anim.Stop("Cat");
            });
            });        
    }

    private void SetGameTheme()
    {
        _currentTheme = Utility.GetRandomNumber(_currentTheme, 0, _gameThemeData.gameThemeList.Count);
        GameConstants.CurrentGameThemeId = _currentTheme;
    }
}
