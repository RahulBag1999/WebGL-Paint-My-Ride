using UnityEngine;
using UnityEngine.UI;

public class WinPopup : UiPopupBase
{
    [SerializeField] private Image _starLeft;
    [SerializeField] private Image _starRight;
    [SerializeField] private Image _starMiddle;

    private GameSettings _gameSettings;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
    }

    internal override void Cleanup()
    {
        
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        
    }

    public void NextLevel()
    {

    }

    public void Home()
    {
        _popupHandler.HidePopup();
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
    }
}