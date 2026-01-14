using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeScreen : UiScreenBase
{
    public TMP_Text gameNameText;

    private GameSettings gameSettings;

    protected override void Awake()
    {
        base.Awake();
        gameSettings = Resources.Load<GameSettings>(nameof(GameSettings));
    }

    private void Start()
    {
        gameNameText.text = gameSettings.gameName;
    }

    public void PlayGame()
    {
        GameManager.Instance.StartGame();
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
