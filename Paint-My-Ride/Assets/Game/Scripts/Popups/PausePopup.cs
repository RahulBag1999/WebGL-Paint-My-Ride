using UnityEngine;
using UnityEngine.UI;

public class PausePopup : UiPopupBase
{
    internal override void Cleanup()
    {
        
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        if (isView)
        {
            GameHelper.Instance.InvokeAction(GameConstants.GameplayPause, true);
        }
        else
        {
            GameHelper.Instance.InvokeAction(GameConstants.GameplayPause, false);
        }
    }

    public void Restart()
    {

    }

    public void Home()
    {
        _popupHandler.HidePopup();
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
    }

    public void Close()
    {
        _popupHandler.HidePopup();
    }
}