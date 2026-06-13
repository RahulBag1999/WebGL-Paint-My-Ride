using System.Collections;
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

    private bool _canCountdown = true;
    private float _count = 0;

    private float _dotTimer = 0f;
    private int _dotCount = 0;

    private GameSettings _gameSettings;

    private RectTransform _barImageRect;

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);

        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();

        _barImageRect = _barImage.rectTransform;
    }

    internal override void HandleGameStateChangeData(object[] data)
    {
        // Set initial position BEFORE animation starts
        Vector2 pos = _barImageRect.anchoredPosition;
        pos.x = _startX;
        _barImageRect.anchoredPosition = pos;

        StartCoroutine(StartLoader());

        _animator.Play("LoadingScreenCat", "Walk");
    }

    private void Update()
    {
        //ParallaxBackground();

        // UV scrolling
        Rect uvRect = _barImage.uvRect;
        uvRect.x -= -_gameSettings.uvScrollSpeed * Time.deltaTime;
        _barImage.uvRect = uvRect;

        // Loading dots animation
        _dotTimer += Time.deltaTime;

        if (_dotTimer >= _gameSettings.dotDelay)
        {
            _dotTimer = 0f;

            _dotCount = (_dotCount + 1) % (_gameSettings.maxDots + 1);

            _loadingText.text = GameConstants.BASE_LOADING_TEXT + new string('.', _dotCount);
        }
    }

    private void ParallaxBackground()
    {
        _bg.uvRect = new Rect(_bg.uvRect.position + new Vector2(_gameSettings.x, _gameSettings.y) * _gameSettings.parallaxSpeed * Time.deltaTime, _bg.uvRect.size);
    }

    private IEnumerator StartLoader()
    {
        _count = 0;
        _canCountdown = true;

        while (_canCountdown)
        {
            yield return null;

            _count += Time.deltaTime;

            float progress = Mathf.Clamp01(_count / _gameSettings.loadingTime);

            // Move filler image from left to right
            Vector2 pos = _barImageRect.anchoredPosition;
            pos.x = Mathf.Lerp(_startX, _endX, progress);
            _barImageRect.anchoredPosition = pos;

            if (progress >= 1f)
            {
                _canCountdown = false;
            }
        }

        // Ensure final position
        Vector2 finalPos = _barImageRect.anchoredPosition;
        finalPos.x = _endX;
        _barImageRect.anchoredPosition = finalPos;

        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, new object[] { false } });

        yield return new WaitForSeconds(2f);
        _animator.Stop("LoadingScreenCat");
    }   

    internal override void Cleanup()
    {

    }
}