using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseScreen : UiScreenBase
{
    public void Replay()
    {
        GameManager.Instance.StartGame();
    }

    public void Home()
    {
        GameManager.Instance.Home();
    }

    public override void Cleanup()
    {
        
    }

    public override void Hide()
    {
        
    }

    public override void Show()
    {
        
    }
}
