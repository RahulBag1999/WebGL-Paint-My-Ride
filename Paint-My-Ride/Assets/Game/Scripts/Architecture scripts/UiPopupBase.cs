using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(UIAnimator))]
public abstract class UiPopupBase : MonoBehaviour
{
    protected PopupHandler _popupHandler;
    protected EssentialConfigData _essentialConfigData;
    protected UIAnimator _uiAnimator;

    internal virtual void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;

        GetUiAnimator();
    }

    private void GetUiAnimator()
    {
        _uiAnimator = GetComponent<UIAnimator>();
    }

    internal virtual void Update() { }

    internal void SetPopupVisibility(bool isView, Action onComplete)
    {
        Action OnCompleteAction = () => 
        {
            gameObject.SetActive(isView);
            onComplete?.Invoke();
        }; 

        if (isView)
        {
            OnCompleteAction?.Invoke();
            _uiAnimator.Play();
        }
        else
        {
            _uiAnimator.PlayReverse(OnCompleteAction);
        }
    }

    internal abstract void HandlePopupToggleData(bool isView, object[] data);

    internal abstract void Cleanup();

    public void PlayClickAudio()
    {
        GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Click");
    }
}
