using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : UiScreenBase
{
    internal override void Cleanup()
    {

    }

    internal override void HandleGameStateChangeData(object[] data)
    {
        Invoke(nameof(GoToLoadingScreen), 2f);
    }

    public void GoToLoadingScreen()
    {
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.LOADING, null });
    }
}
