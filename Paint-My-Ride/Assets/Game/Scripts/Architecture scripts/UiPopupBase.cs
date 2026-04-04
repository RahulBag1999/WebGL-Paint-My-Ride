using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
//[RequireComponent(typeof(AnimationPopup))]
public abstract class UiPopupBase : MonoBehaviour
{
    protected PopupHandler _popupHandler;
    //protected AnimationPopup animationPopup => _animationPopup;

    internal virtual void Init(PopupHandler popupHandler)
    {
        _popupHandler = popupHandler;
        //_animationPopup = GetComponent<AnimationPopup>();
    }

    internal void SetPopupVisibility(bool isView)
    {
        //Action OnCompleteAction = () => gameObject.SetActive(isView);

        //if (isView)
        //{
        //    OnCompleteAction();
        //    _animationPopup.StartTween();
        //}
        //else
        //    _animationPopup.StopTween(OnCompleteAction);

        gameObject.SetActive(isView);
    }

    internal abstract void HandlePopupToggleData(bool isView, object[] data);

    internal abstract void Cleanup();
}
