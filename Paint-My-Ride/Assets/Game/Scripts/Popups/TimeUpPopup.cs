using System;

public class TimeUpPopup : UiPopupBase
{
    private PreResultData _preResultData;
    private GameSettings _gameSettings;

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
        });

        PlayerDataHandler.Player.GameCurrency.UpdateCoin(-_gameSettings.levelFailContinueCoin);
    }
}