using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class PausePopup : UiPopupBase
{
    [SerializeField] private Image[] zImages; // Assign your Z images
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float delayBetween = 0.3f;

    private GameThemeData _gameThemeData;
    private int _currentLevel;
    private int _currentTheme;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
    }

    internal override void Cleanup()
    {
        
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        if (isView)
        {
            if(data.Length > 0)
            {
                _currentLevel = (int)data[0];
                _currentTheme = (int)data[1];
            }
            //PlayLoop();
            GameHelper.Instance.InvokeAction(GameConstants.GameplayPause, true);
        }
        else
        {
            GameHelper.Instance.InvokeAction(GameConstants.GameplayPause, false);
        }
    }

    private void PlayLoop()
    {
        for (int i = 0; i < zImages.Length; i++)
        {
            AnimateZ(zImages[i], i * delayBetween);
        }
    }

    private void AnimateZ(Image z, float delay)
    {
        z.transform.localScale = Vector3.zero;

        Color c = z.color;
        c.a = 0;
        z.color = c;

        Sequence seq = DOTween.Sequence();

        seq.SetDelay(delay)
            .AppendCallback(() =>
            {
                // Reset before animation
                z.transform.localScale = Vector3.zero;
                Color col = z.color;
                col.a = 0;
                z.color = col;
            })
            .Append(z.DOFade(1f, duration * 0.3f)) // fade in
            .Join(z.transform.DOScale(1f, duration).SetEase(Ease.OutBack))
            .Join(z.transform.DOLocalMoveY(z.transform.localPosition.y + 30f, duration))// scale up
            .Append(z.DOFade(0f, duration * 0.5f)) // fade out
            .OnComplete(() =>
            {
                AnimateZ(z, 0); // loop
            });
    }

    public void Restart()
    {
        Action OnComplete = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[]
            {
                false,
                _currentLevel,
                _currentTheme
            } });
        };

        SetGameTheme();        
        _popupHandler.HidePopup(OnComplete);
    }

    private void SetGameTheme()
    {
        _currentTheme = Utility.GetRandomNumber(_currentTheme, 0, _gameThemeData.gameThemeList.Count);
    }

    public void Home()
    {
        Action OnComplete = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
        };
        _popupHandler.HidePopup(OnComplete);        
    }

    public void MusicToggle(bool state)
    {
        if (state) 
        {
            Debug.Log("Music on");
        }
        else
        {
            Debug.Log("Music off");
        }
        AudioHandler.Instance.HandleMusicState(state);
    }

    public void SoundToggle(bool state)
    {
        if (state)
        {
            Debug.Log("Sound on");            
        }
        else
        {
            Debug.Log("Sound off");
        }
        AudioHandler.Instance.HandleSfxState(state);
    }

    public void Close()
    {
        _popupHandler.HidePopup();
    }
}