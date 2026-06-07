using UnityEngine;
using PrimeTween;
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
        currentSequence.Stop();
    }

    #endregion

    #region CORE

    private void PlayInternal(bool reverse, Action onComplete)
    {
        currentSequence.Stop();

        currentSequence = Sequence.Create();

        foreach (var item in items)
        {
            if (item.target == null)
                continue;

            Sequence elementSequence = Sequence.Create();

            foreach (var anim in item.animations)
            {
                switch (anim.type)
                {
                    case AnimationType.Move:
                        {
                            Vector2 moveStart = reverse ? anim.moveTo : anim.moveFrom;
                            Vector2 moveEnd = reverse ? anim.moveFrom : anim.moveTo;

                            item.target.anchoredPosition = moveStart;

                            elementSequence.Group(
                                Tween.UIAnchoredPosition(
                                    item.target,
                                    moveEnd,
                                    anim.duration,
                                    ease: anim.ease));

                            break;
                        }

                    case AnimationType.Scale:
                        {
                            Vector3 scaleStart = reverse ? anim.scaleTo : anim.scaleFrom;
                            Vector3 scaleEnd = reverse ? anim.scaleFrom : anim.scaleTo;

                            item.target.localScale = scaleStart;

                            elementSequence.Group(
                                Tween.Scale(
                                    item.target,
                                    scaleEnd,
                                    anim.duration,
                                    ease: anim.ease));

                            break;
                        }

                    case AnimationType.Fade:
                        {
                            if (item.canvasGroup == null)
                            {
                                item.canvasGroup = item.target.GetComponent<CanvasGroup>();

                                if (item.canvasGroup == null)
                                    item.canvasGroup = item.target.gameObject.AddComponent<CanvasGroup>();
                            }

                            float fadeStart = reverse ? anim.fadeTo : anim.fadeFrom;
                            float fadeEnd = reverse ? anim.fadeFrom : anim.fadeTo;

                            item.canvasGroup.alpha = fadeStart;

                            elementSequence.Group(
                                Tween.Alpha(
                                    item.canvasGroup,
                                    fadeEnd,
                                    anim.duration,
                                    ease: anim.ease));

                            break;
                        }
                }
            }

            currentSequence.Group(elementSequence);
        }

        currentSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    #endregion
}