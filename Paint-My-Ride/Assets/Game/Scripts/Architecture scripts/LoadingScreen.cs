using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : UiScreenBase
{
    [SerializeField] private RawImage _bg;

    [Header("Loader")]
    [SerializeField] private RawImage _barImage;

    [Header("Text")]
    [SerializeField] private TMP_Text _loadingText;

    [Header("Animation")]
    [SerializeField] private MultiSpriteAnimator _animator;

    [Header("Filler Movement")]
    [SerializeField] private float _startX = -783f;
    [SerializeField] private float _endX = 0f;

    private float startTime;
    private int _dotCount = 0;

    private GameSettings _gameSettings;

    private RectTransform _barImageRect;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);

        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();

        _barImageRect = _barImage.rectTransform;

        startTime = Time.time;
    }

    internal override void HandleGameStateChangeData(object[] data)
    {
        // Set initial position BEFORE animation starts
        Vector2 pos = _barImageRect.anchoredPosition;
        pos.x = _startX;
        _barImageRect.anchoredPosition = pos;

        StartLoaderAsync(this.GetCancellationTokenOnDestroy()).Forget();
        ScrollUvAsync(this.GetCancellationTokenOnDestroy()).Forget();
        AnimateDotsAsync(this.GetCancellationTokenOnDestroy()).Forget();

        _animator.Play("LoadingScreenCat", "Walk");
    }

    private async UniTaskVoid ScrollUvAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Rect uvRect = _barImage.uvRect;
            uvRect.x += _gameSettings.uvScrollSpeed * Time.deltaTime;
            _barImage.uvRect = uvRect;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private async UniTaskVoid AnimateDotsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(
                System.TimeSpan.FromSeconds(_gameSettings.dotDelay),
                DelayType.DeltaTime,
                cancellationToken: token);

            _dotCount = (_dotCount + 1) % (_gameSettings.maxDots + 1);

            _loadingText.text =
                GameConstants.BASE_LOADING_TEXT +
                new string('.', _dotCount);
        }
    }

    private void ParallaxBackground()
    {
        _bg.uvRect = new Rect(_bg.uvRect.position + new Vector2(_gameSettings.x, _gameSettings.y) * _gameSettings.parallaxSpeed * Time.deltaTime, _bg.uvRect.size);
    }

    private async UniTaskVoid StartLoaderAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            float progress = Mathf.Clamp01(
                (Time.time - startTime) / _gameSettings.loadingTime);

            Vector2 pos = _barImageRect.anchoredPosition;
            pos.x = Mathf.Lerp(_startX, _endX, progress);
            _barImageRect.anchoredPosition = pos;

            if (progress >= 1f)
                break;
        }

        // Ensure final position
        Vector2 finalPos = _barImageRect.anchoredPosition;
        finalPos.x = _endX;
        _barImageRect.anchoredPosition = finalPos;

        GameHelper.Instance.InvokeAction(
            GameConstants.ChangeGameState,
            new object[] { GameStates.HOME, new object[] { false } });

        await UniTask.Delay(
            TimeSpan.FromSeconds(2f),
            DelayType.DeltaTime,
            cancellationToken: token);

        _animator.Stop("LoadingScreenCat");
    }

    internal override void Cleanup()
    {

    }
}