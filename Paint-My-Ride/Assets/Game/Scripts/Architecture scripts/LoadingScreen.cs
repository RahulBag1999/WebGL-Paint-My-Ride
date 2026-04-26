using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : UiScreenBase
{
    [SerializeField] private RawImage _bg;
    [SerializeField] private RawImage _barImage;
    [SerializeField] private RectTransform _barMaskRectTransform;
    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private MultiSpriteAnimator _animator;

    private bool _canCountdown = true;
    private float _barMaskWidth;
    private float _count = 0;

    private float _dotTimer = 0f;
    private int _dotCount = 0;    

    private GameSettings _gameSettings;    

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();

        _barMaskWidth = _barMaskRectTransform.sizeDelta.x;
    }

    private void Update()
    {
        ParallaxBackground();

        // UV scrolling
        Rect uvRect = _barImage.uvRect;
        uvRect.x -= _gameSettings.uvScrollSpeed * Time.deltaTime;
        _barImage.uvRect = uvRect;

        // Dot animation
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

            Vector2 barMaskSizeDelta = _barMaskRectTransform.sizeDelta;
            barMaskSizeDelta.x = progress * _barMaskWidth;
            _barMaskRectTransform.sizeDelta = barMaskSizeDelta;

            if (progress >= 1f)
            {
                _canCountdown = false;
            }
        }
        
        Vector2 finalSize = _barMaskRectTransform.sizeDelta;
        finalSize.x = _barMaskWidth;
        _barMaskRectTransform.sizeDelta = finalSize;

        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, new object[] { false } });
        _animator.Stop("CatMiddle");
    }    

    internal override void HandleGameStateChangeData(object[] data)
    {
        StartCoroutine(StartLoader());
        _animator.Play("CatMiddle", "Walk");
    }

    internal override void Cleanup()
    {
        
    }
}