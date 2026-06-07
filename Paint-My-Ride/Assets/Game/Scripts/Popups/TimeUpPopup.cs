using System;
using UnityEngine;

public class TimeUpPopup : UiPopupBase
{
    private PreResultData _preResultData;
    private GameSettings _gameSettings;

    [SerializeField] private MultiSpriteAnimator _anim;

    internal override void Cleanup()
    {
        
    }

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        if (isView) 
        {
            _preResultData = (PreResultData)data[0];

            _anim.Play("Cat", "Idle");
        }
    }

    public void KeepPlaying()
    {
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[] 
            {
                true, 
                _preResultData 
            } });
        };
        
        _popupHandler.HidePopup(() => { }, () => 
        {
            OnChangeGameState?.Invoke();

            _anim.Stop("Cat");
        });

        PlayerDataHandler.Player.GameCurrency.UpdateCoin(-_gameSettings.levelFailContinueCoin);
    }

    public void Close()
    {
        _popupHandler.HidePopup(
            () => 
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
                    GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
                },
                () =>
                {
                    _anim.Stop("Cat");
                });
            });
    }
}