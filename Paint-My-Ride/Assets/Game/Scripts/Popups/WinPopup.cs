using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : UiPopupBase
{
    [SerializeField] private Image _starL;
    [SerializeField] private Image _starMiddle;
    [SerializeField] private Image _starR;

    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _headerCoinText;
    [SerializeField] private MultiSpriteAnimator _anim;
    [SerializeField] private HeartSpawner _heartSpawner;
    [SerializeField] private CoinCollectAnimation _coinCollectAnim;

    [SerializeField] private ButtonEffect _homeButton;
    [SerializeField] private ButtonEffect _nextLevelButton;

    public Transform heartParent;

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

                _headerCoinText.text = PlayerDataHandler.Player.GameCurrency.Coins.ToString();
                _coinText.text = _gameSettings.coins.ToString();

                SetGameTheme();
                IncrementLevel();

                Debug.Log($"Stars :: {GetStars(_remainedTime / 100f)}");

                PlayStarAnimation(GetStars(_remainedTime / 100f));
            }
            GameHelper.Instance.InvokeAction(GameConstants.OnUpdateTutorialCanvas, false);
            GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Win");
            _anim.Play("Cat", "Idle");
            _heartSpawner.Play();
        }
        else
        {
            
        }
    }

    public void PlayStarAnimation(int starCount)
    {
        AnimateStarsAsync(starCount, this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid AnimateStarsAsync(int count, CancellationToken token)
    {
        await UniTask.Delay(
            TimeSpan.FromSeconds(1f),
            DelayType.DeltaTime,
            cancellationToken: token);

        if (count >= 1)
        {
            AnimateStar(_starL, _gameSettings.filledStar);

            await UniTask.Delay(
                TimeSpan.FromSeconds(_gameSettings.delayBetweenStars),
                DelayType.DeltaTime,
                cancellationToken: token);
        }

        if (count >= 2)
        {
            AnimateStar(_starMiddle, _gameSettings.filledStarMid);

            await UniTask.Delay(
                TimeSpan.FromSeconds(_gameSettings.delayBetweenStars),
                DelayType.DeltaTime,
                cancellationToken: token);
        }

        if (count >= 3)
        {
            AnimateStar(_starR, _gameSettings.filledStar);
        }

        _coinCollectAnim.PlayAnimation(_gameSettings.coins);
    }

    private void AnimateStar(Image star, Sprite sprite)
    {
        star.sprite = sprite;

        star.transform.localScale = Vector3.zero;

        Color c = star.color;
        c.a = 0f;
        star.color = c;

        Sequence.Create()
            .Group(Tween.Alpha(
                star,
                1f,
                0.15f))

            .Group(Tween.Scale(
                star.transform,
                Vector3.one * 1.4f,
                0.3f,
                Ease.OutBack))

            .Chain(Tween.Scale(
                star.transform,
                Vector3.one,
                0.2f,
                Ease.InOutSine));
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
        //PlayerDataHandler.Player.GameCurrency.UpdateCoin(GameConstants.COINS_WIN);
        PlayerDataHandler.Player.GameplayProgress.UpdateMaxUnlockedLevelId(_currentLevel);

        _currentLevel = PlayerDataHandler.Player.GameplayProgress.MaxUnlockedLevelId;
    }

    private void SetGameTheme()
    {
        _currentTheme = Utility.GetRandomNumber(_currentTheme, 0, _gameThemeData.gameThemeList.Count);
        GameConstants.CurrentGameThemeId = _currentTheme;
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

        _nextLevelButton.PlayEffect(() => 
        {
            _popupHandler.HidePopup(() => { }, () =>
            {
                TransitionHelper.Instance.Play(
                () =>
                {

                },
                () =>
                {
                    OnChangeGameState?.Invoke();
                },
                () =>
                {
                    HideCat();
                    _heartSpawner.StopAll();
                });

                HideCat();
            });
        });        
    }

    public void Home()
    {
        Action OnChangeGameState = () => 
        {
            GameHelper.Instance.InvokeAction(GameConstants.OnUpdateTutorialCanvas, false);
            GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, null });
        };

        _homeButton.PlayEffect(() => 
        {
            _popupHandler.HidePopup(() =>
            {

            },
        () =>
        {
            TransitionHelper.Instance.Play(
            () =>
            {

            },
            () =>
            {
                OnChangeGameState?.Invoke();
            },
            () =>
            {
                _heartSpawner.StopAll();
            });

            HideCat();
        });
        });        
    }

    private void HideCat()
    {
        _anim.Stop("Cat");
    }

    private void ResetStars()
    {
        _starL.sprite = _starR.sprite = _gameSettings.emptyStar;
        _starMiddle.sprite = _gameSettings.emptyStarMid;
    }
}