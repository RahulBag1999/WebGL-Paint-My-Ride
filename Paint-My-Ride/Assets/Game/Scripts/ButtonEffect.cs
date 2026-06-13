using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System;
public class ButtonEffect : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Animation")]
    [SerializeField] private float squeezeScaleX = 1.15f;
    [SerializeField] private float squeezeScaleY = 0.85f;
    [SerializeField] private float squeezeDuration = 0.08f;

    [SerializeField] private float bounceScale = 1.08f;
    [SerializeField] private float bounceDuration = 0.15f;

    private Sequence currentSequence;

    public Ease ease1 = Ease.OutQuad;
    public Ease ease2 = Ease.OutQuad;
    public Ease ease3 = Ease.OutBack;
    public Ease ease4 = Ease.OutElastic;

    public void PlayEffect(Action onComplete = null)
    {
        currentSequence.Stop();

        target.localScale = Vector3.one;

        currentSequence = Sequence.Create()

            // Squeeze
            .Group(Tween.ScaleX(
                target,
                endValue: squeezeScaleX,
                duration: squeezeDuration,
                ease: ease1))

            .Group(Tween.ScaleY(
                target,
                endValue: squeezeScaleY,
                duration: squeezeDuration,
                ease: ease2))

            // Bubble bounce
            .Chain(Tween.Scale(
                target,
                endValue: Vector3.one * bounceScale,
                duration: bounceDuration,
                ease: ease3))

            // Callback
            .ChainCallback(() =>
            {
                onComplete?.Invoke();
            })

            // Return to normal
            .Chain(Tween.Scale(
                target,
                endValue: Vector3.one,
                duration: bounceDuration,
                ease: ease4));
    }
}