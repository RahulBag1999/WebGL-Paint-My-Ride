using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public abstract class UiScreenBase : MonoBehaviour
{
    private Canvas _canvas;
    protected PopupHandler _popupHandler;
    protected EssentialConfigData _essentialConfigData;

    internal virtual void Init(PopupHandler popupHandler, EssentialConfigData essentialConfigData)
    {
        GetCanvas();
        _popupHandler = popupHandler;
        _essentialConfigData = essentialConfigData;
    }

    private void GetCanvas()
    {
        _canvas = GetComponent<Canvas>();
    }

    internal void SetScreenCanvasVisibility(bool isView)
    {
        if (Equals(_canvas, null))
        {
            GetCanvas();
        }
        _canvas.enabled = isView;
    }

    internal abstract void HandleGameStateChangeData(object[] data);

    internal abstract void Cleanup();

    public void PlayClickAudio()
    {
        GameHelper.Instance.InvokeAction(GameConstants.PlayAudioOneShot, "Click");
    }
}
