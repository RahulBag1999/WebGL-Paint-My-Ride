using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : UiScreenBase
{
    public void Home()
    {
        GameManager.Instance.Home();
    }

    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }

    public override void Cleanup()
    {
        
    }

    public override void Show()
    {
        
    }

    public override void Hide()
    {
        
    }
}
