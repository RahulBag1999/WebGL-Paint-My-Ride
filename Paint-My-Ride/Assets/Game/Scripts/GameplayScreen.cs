using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayScreen : UiScreenBase
{
    public TMP_Text currentLevelText;

    public void Home()
    {
        UiController.Instance.ShowScreen<HomeScreen>();
    }

    public void RestartLevel()
    {
        GameManager.Instance.StartGame();
    }

    public override void Cleanup()
    {
        
    }

    public override void Show()
    {
        currentLevelText.text = $"LEVEL {GameConstants.CURRENT_LEVEL_CONFIG.levelId}";
    }

    public override void Hide()
    {
        
    }
}
