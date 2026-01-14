using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UiScreenBase : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetCanvasVisibility(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.blocksRaycasts = isVisible;

        if (isVisible) Show();
        else Hide();
    }

    public abstract void Show();
    public abstract void Hide();

    public abstract void Cleanup();
}

