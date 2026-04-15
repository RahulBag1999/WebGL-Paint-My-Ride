using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class UIAnimator : MonoBehaviour
{
    public enum AnimationType
    {
        Move,
        Scale,
        Fade
    }

    [Serializable]
    public class AnimationData
    {
        public AnimationType type;

        public float duration = 0.4f;
        public float delay = 0f;
        public Ease ease = Ease.OutBack;

        // Move
        public Vector2 moveFrom;
        public Vector2 moveTo;

        // Scale
        public Vector3 scaleFrom = Vector3.zero;
        public Vector3 scaleTo = Vector3.one;

        // Fade
        public float fadeFrom = 0f;
        public float fadeTo = 1f;
    }

    [Serializable]
    public class UIAnimItem
    {
        public RectTransform target;
        public List<AnimationData> animations = new List<AnimationData>();

        [HideInInspector] public CanvasGroup canvasGroup;
    }

    [SerializeField] private List<UIAnimItem> items = new List<UIAnimItem>();

    private Sequence currentSequence;

    #region PUBLIC API

    public void Play(Action onComplete = null)
    {
        PlayInternal(false, onComplete);
    }

    public void PlayReverse(Action onComplete = null)
    {
        PlayInternal(true, onComplete);
    }

    public void Kill()
    {
        currentSequence?.Kill();
    }

    #endregion

    #region CORE

    private void PlayInternal(bool reverse, Action onComplete)
    {
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        foreach (var item in items)
        {
            if (item.target == null) continue;

            Sequence elementSequence = DOTween.Sequence();

            foreach (var anim in item.animations)
            {
                Tween tween = null;

                switch (anim.type)
                {
                    case AnimationType.Move:
                        Vector2 moveStart = reverse ? anim.moveTo : anim.moveFrom;
                        Vector2 moveEnd = reverse ? anim.moveFrom : anim.moveTo;

                        item.target.anchoredPosition = moveStart;

                        tween = item.target.DOAnchorPos(moveEnd, anim.duration)
                            .SetEase(anim.ease);
                        break;

                    case AnimationType.Scale:
                        Vector3 scaleStart = reverse ? anim.scaleTo : anim.scaleFrom;
                        Vector3 scaleEnd = reverse ? anim.scaleFrom : anim.scaleTo;

                        item.target.localScale = scaleStart;

                        tween = item.target.DOScale(scaleEnd, anim.duration)
                            .SetEase(anim.ease);
                        break;

                    case AnimationType.Fade:
                        if (item.canvasGroup == null)
                        {
                            item.canvasGroup = item.target.GetComponent<CanvasGroup>();
                            if (item.canvasGroup == null)
                                item.canvasGroup = item.target.gameObject.AddComponent<CanvasGroup>();
                        }

                        float fadeStart = reverse ? anim.fadeTo : anim.fadeFrom;
                        float fadeEnd = reverse ? anim.fadeFrom : anim.fadeTo;

                        item.canvasGroup.alpha = fadeStart;

                        tween = item.canvasGroup.DOFade(fadeEnd, anim.duration)
                            .SetEase(anim.ease);
                        break;
                }

                if (tween != null)
                {
                    tween.SetDelay(anim.delay);
                    elementSequence.Join(tween);
                }
            }

            currentSequence.Join(elementSequence);
        }

        currentSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    #endregion
}