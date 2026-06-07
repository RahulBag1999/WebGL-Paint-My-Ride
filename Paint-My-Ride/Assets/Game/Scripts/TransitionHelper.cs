using EasyTransition;
using System;
using UnityEngine;

public class TransitionHelper : Singleton<TransitionHelper>
{
    private TransitionManager _transitionManager;

    [SerializeField] private TransitionSettings _transitionSettings;

    protected override void OnInit()
    {
        base.OnInit();

        gameObject.AddComponent<TransitionManager>();
        _transitionManager = TransitionManager.Instance();
    }

    public void Play(Action onStart, Action onCutPoint, Action onEnd, float delay = 0f)
    {
        _transitionManager.onTransitionBegin += () => onStart?.Invoke();
        _transitionManager.onTransitionCutPointReached += () => onCutPoint?.Invoke();
        _transitionManager.onTransitionEnd += () => 
        {
            onEnd?.Invoke();
            Cleanup(onStart, onCutPoint, onEnd);
        };

        _transitionManager.Transition(_transitionSettings, delay);
    }

    private void Cleanup(Action onStart, Action onCutPoint, Action onEnd)
    {
        _transitionManager.onTransitionBegin -= () => onStart?.Invoke();
        _transitionManager.onTransitionCutPointReached -= () => onCutPoint?.Invoke();
        _transitionManager.onTransitionEnd -= () => onEnd?.Invoke();
    }
}
