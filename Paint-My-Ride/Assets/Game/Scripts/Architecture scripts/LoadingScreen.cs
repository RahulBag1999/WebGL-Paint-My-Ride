using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : UiScreenBase
{
    [SerializeField] private RawImage _barImage;
    [SerializeField] private RectTransform _barMaskRectTransform;
    [SerializeField] private TMP_Text _loadingText;

    private bool _canCountdown = true;
    private float _barMaskWidth;
    private float _count = 0;

    private float _dotTimer = 0f;
    private int _dotCount = 0;
    private string _baseText = "Loading";

    private GameSettings _gameSettings;    

    internal override void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        base.Init(popupHandler, essentialConfigData);
        _gameSettings = _essentialConfigData.AccessConfig<GameSettings>();

        _barMaskWidth = _barMaskRectTransform.sizeDelta.x;
    }

    private void Update()
    {
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
            _loadingText.text = _baseText + new string('.', _dotCount);
        }
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

        yield return new WaitForSeconds(0.5f);
        GameHelper.Instance.InvokeAction(GameConstants.ChangeGameState, new object[] { GameStates.HOME, new object[] { false } });
    }    

    internal override void HandleGameStateChangeData(object[] data)
    {
        StartCoroutine(StartLoader());
    }

    internal override void Cleanup()
    {
        
    }
}