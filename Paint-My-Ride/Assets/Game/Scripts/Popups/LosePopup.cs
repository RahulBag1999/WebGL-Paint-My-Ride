using UnityEngine;

public class LosePopup : UiPopupBase
{
    internal override void Cleanup()
    {
        
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        
    }

    public void TryAgain()
    {

    }

    public void Home()
    {
        _popupHandler.HidePopup();
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
    }
}
