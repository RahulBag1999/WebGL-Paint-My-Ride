using UnityEngine;
using PrimeTween;

public class ScaleAnimator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform _target;

    [Header("Scale Settings")]
    [SerializeField] private float _initialScale = 1f;
    [SerializeField] private float _finalScale = 1.1f;

    [Header("Animation")]
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    private Tween _scaleTween;

    private void Awake()
    {
        if (_target == null)
            _target = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Play();
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Play()
    {
        Stop();

        _target.localScale = Vector3.one * _initialScale;

        _scaleTween = Tween.Scale(
            _target,
            endValue: Vector3.one * _finalScale,
            duration: _duration,
            ease: _ease,
            cycles: -1,
            cycleMode: CycleMode.Yoyo
        );
    }

    public void Stop()
    {
        if (_scaleTween.isAlive)
        {
            _scaleTween.Stop();
        }

        if (_target != null)
        {
            _target.localScale = Vector3.one * _initialScale;
        }
    }
}