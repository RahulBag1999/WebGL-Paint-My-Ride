using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : UiPopupBase
{
    [SerializeField] private Image _starL;
    [SerializeField] private Image _starMiddle;
    [SerializeField] private Image _starR;

    [SerializeField] private TMP_Text _coinText;

    private GameSettings _gameSettings;
    private GameThemeData _gameThemeData;

    private int _currentLevel;
    private int _currentTheme;
    private float _remainedTime;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();
        _gameThemeData = _essentialConfigData.AccessConfig<GameThemeData>();
    }

    internal override void Update()
    {
        base.Update();
    }

    internal override void Cleanup()
    {
        
    }

    internal override void HandlePopupToggleData(bool isView, object[] data)
    {
        if (isView) 
        {
            ResetStars();
            if (data.Length > 0)
            {
                _currentLevel = (int)data[0];
                _currentTheme = (int)data[1];
                _remainedTime = (float)data[2];

                _coinText.text = _gameSettings.coins.ToString();

                SetGameTheme();
                IncrementLevel();

                Debug.Log($"Stars :: {GetStars(_remainedTime / 100f)}");

                PlayStarAnimation(GetStars(_remainedTime / 100f));
            }
            GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Win");
        }
        else
        {
            
        }
    }

    public void PlayStarAnimation(int starCount)
    {
        StartCoroutine(AnimateStars(starCount));
    }

    private IEnumerator AnimateStars(int count)
    {
        yield return new WaitForSeconds(1f);
        
        if (count >= 1)
        {
            AnimateStar(_starL, _gameSettings.filledStar);
            yield return new WaitForSeconds(_gameSettings.delayBetweenStars);
        }

        if (count >= 2)
        {
            AnimateStar(_starMiddle, _gameSettings.filledStarMid);
            yield return new WaitForSeconds(_gameSettings.delayBetweenStars);
        }

        if (count >= 3)
        {
            AnimateStar(_starR, _gameSettings.filledStar);
        }
    }

    private void AnimateStar(Image star, Sprite sprite)
    {
        star.sprite = sprite;

        star.transform.localScale = Vector3.zero;

        Color c = star.color;
        c.a = 0;
        star.color = c;

        Sequence seq = DOTween.Sequence();

        seq.Append(star.DOFade(1f, 0.15f));

        seq.Join(
            star.transform.DOScale(1.4f, 0.3f)
                .SetEase(Ease.OutBack)
        );

        seq.Append(
            star.transform.DOScale(1f, 0.2f)
                .SetEase(Ease.InOutSine)
        );
    }

    private int GetStars(float timePercentage)
    {
        if (timePercentage >= 0.4f)
        {
            return 3;
        }
        else if (timePercentage >= 0.2f)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }   

    private void IncrementLevel()
    {
        PlayerDataHandler.Player.GameCurrency.UpdateCoin(GameConstants.COINS_WIN);
        PlayerDataHandler.Player.GameplayProgress.UpdateMaxUnlockedLevelId(_currentLevel);

        _currentLevel = PlayerDataHandler.Player.GameplayProgress.MaxUnlockedLevelId;
    }

    private void SetGameTheme()
    {
        _currentTheme = Utility.GetRandomNumber(_currentTheme, 0, _gameThemeData.gameThemeList.Count);
    }

    public void NextLevel()
    {
        Action OnChangeGameState = () =>
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.GAMEPLAY, new object[]
            {
                false,
                _currentLevel,
                _currentTheme
            } });
        };
        _popupHandler.HidePopup(() => { }, () => 
        {
            ScreenTransition.Instance.Play(
                OnStarted => 
                {

                }, 
                OnPartial => 
                {
                    OnChangeGameState?.Invoke();
                }, 
                OnCompleted => 
                {

                }
            );
        });
    }

    public void Home()
    {
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
        };

        _popupHandler.HidePopup(() => { }, () =>
        {
            ScreenTransition.Instance.Play(
                OnStarted =>
                {

                },
                OnPartial =>
                {
                    OnChangeGameState?.Invoke();
                },
                OnCompleted =>
                {

                }
            );
        });       
    }

    private void ResetStars()
    {
        _starL.sprite = _starR.sprite = _gameSettings.emptyStar;
        _starMiddle.sprite = _gameSettings.emptyStarMid;
    }
}