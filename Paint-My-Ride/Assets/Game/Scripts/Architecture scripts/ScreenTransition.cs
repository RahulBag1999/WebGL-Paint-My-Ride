using UnityEngine;
using System;

public class ScreenTransition : Singleton<ScreenTransition>
{
    [Header("Left Cat (Bottom → Top)")]
    public RectTransform leftCat;
    public RectTransform leftHip;
    public RectTransform leftBar;

    [Header("Right Cat (Top → Bottom)")]
    public RectTransform rightCat;
    public RectTransform rightHip;
    public RectTransform rightBar;

    [Header("Animation")]
    public float moveSpeed = 800f;
    public float shrinkSpeed = 800f;
    public float leftTargetY;
    public float rightTargetY;

    private bool isPlaying = false;
    private bool followHip = true;
    private bool shrinkPhase = false;

    [Space]

    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private MultiSpriteAnimator _animator;

    // Callbacks
    private Action<string> onReachedTarget;
    private Action<string> onComplete;

    private bool targetInvoked = false;

    // Cached initial states
    private Vector2 leftCatStart, rightCatStart;
    private Vector2 leftBarStartPos, rightBarStartPos;
    private float leftBarStartHeight, rightBarStartHeight;

    protected override void OnInit()
    {
        base.OnInit();
        SetVisibility(false);

        leftCatStart = leftCat.anchoredPosition;
        rightCatStart = rightCat.anchoredPosition;

        leftBarStartPos = leftBar.anchoredPosition;
        rightBarStartPos = rightBar.anchoredPosition;

        leftBarStartHeight = leftBar.sizeDelta.y;
        rightBarStartHeight = rightBar.sizeDelta.y;
    }

    // Play with BOTH callbacks
    public void Play(Action<string> onStarted = null, Action<string> onReachedTargetAction = null, Action<string> onCompleteAction = null)
    {
        ResetState();
        SetVisibility(true);
        onStarted?.Invoke("Screen transition started");    

        onReachedTarget = onReachedTargetAction;
        onComplete = onCompleteAction;

        targetInvoked = false;

        isPlaying = true;
        followHip = true;
        shrinkPhase = false;

        _animator.Play("ScreenTransitionCatLeft", "Walk");
        _animator.Play("ScreenTransitionCatRight", "Walk");
    }

    private void SetVisibility(bool isVisible)
    {
        _canvas.enabled = isVisible;
    }

    private void Update()
    {
        if (!isPlaying) return;

        MoveCats();

        if (followHip)
        {
            UpdateBarsWithHip();
        }
        else if (shrinkPhase)
        {
            ShrinkBars();
        }
    }

    private void MoveCats()
    {
        Vector2 leftPos = leftCat.anchoredPosition;
        leftPos.y += moveSpeed * Time.deltaTime;

        Vector2 rightPos = rightCat.anchoredPosition;
        rightPos.y -= moveSpeed * Time.deltaTime;

        bool leftReached = leftPos.y >= leftTargetY;
        bool rightReached = rightPos.y <= rightTargetY;

        if (leftReached) leftPos.y = leftTargetY;
        if (rightReached) rightPos.y = rightTargetY;

        // Invoke target callback ONCE
        if (!targetInvoked && leftReached && rightReached)
        {
            targetInvoked = true;
            onReachedTarget?.Invoke("Screen transition full");
        }

        // Switch phase
        if (followHip && leftReached && rightReached)
        {
            followHip = false;
            shrinkPhase = true;
        }

        leftCat.anchoredPosition = leftPos;
        rightCat.anchoredPosition = rightPos;
    }

    private void UpdateBarsWithHip()
    {
        Vector2 leftScreen = RectTransformUtility.WorldToScreenPoint(null, leftHip.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, leftScreen, null, out Vector2 leftLocal);

        float canvasBottom = -_canvasRect.rect.height * 0.5f;
        float leftHeight = Mathf.Max(0, leftLocal.y - canvasBottom);
        SetHeight(leftBar, leftHeight);

        Vector2 rightScreen = RectTransformUtility.WorldToScreenPoint(null, rightHip.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, rightScreen, null, out Vector2 rightLocal);

        float canvasTop = _canvasRect.rect.height * 0.5f;
        float rightHeight = Mathf.Max(0, canvasTop - rightLocal.y);
        SetHeight(rightBar, rightHeight);
    }

    private void ShrinkBars()
    {
        float delta = shrinkSpeed * Time.deltaTime;

        float leftHeight = leftBar.sizeDelta.y - delta;
        float rightHeight = rightBar.sizeDelta.y - delta;

        Vector2 leftPos = leftBar.anchoredPosition;
        leftPos.y += delta;

        Vector2 rightPos = rightBar.anchoredPosition;
        rightPos.y -= delta;

        if (leftHeight <= 0 && rightHeight <= 0)
        {
            CompleteTransition();
            return;
        }

        SetHeight(leftBar, Mathf.Max(0, leftHeight));
        SetHeight(rightBar, Mathf.Max(0, rightHeight));

        leftBar.anchoredPosition = leftPos;
        rightBar.anchoredPosition = rightPos;
    }

    private void CompleteTransition()
    {
        ResetState();

        onComplete?.Invoke("Screen transition completed");
        onComplete = null;
        onReachedTarget = null;

        _animator.Stop("ScreenTransitionCatLeft");
        _animator.Stop("ScreenTransitionCatRight");

        SetVisibility(false);
    }

    private void ResetState()
    {
        leftCat.anchoredPosition = leftCatStart;
        rightCat.anchoredPosition = rightCatStart;

        leftBar.anchoredPosition = leftBarStartPos;
        rightBar.anchoredPosition = rightBarStartPos;

        SetHeight(leftBar, leftBarStartHeight);
        SetHeight(rightBar, rightBarStartHeight);

        isPlaying = false;
        followHip = true;
        shrinkPhase = false;
    }

    private void SetHeight(RectTransform bar, float height)
    {
        Vector2 size = bar.sizeDelta;
        size.y = height;
        bar.sizeDelta = size;
    }
}