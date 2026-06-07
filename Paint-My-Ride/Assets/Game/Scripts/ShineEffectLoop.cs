using UnityEngine;
using PrimeTween;
using Coffee.UIExtensions;

[RequireComponent(typeof(ShinyEffectForUGUI))]
public class ShineEffectLoop : MonoBehaviour
{
    private ShinyEffectForUGUI shinyEffect;

    [SerializeField] private float duration = 1f;
    [SerializeField] private float waitTime = 0.5f;
    [SerializeField] private Ease ease = Ease.Linear;

    private Sequence _sequence;

    private void Awake()
    {
        shinyEffect = GetComponent<ShinyEffectForUGUI>();
    }

    private void Start()
    {
        _sequence = Sequence.Create(cycles: -1)
            .ChainCallback(() => shinyEffect.location = 0f)
            .Chain(
                Tween.Custom(
                    0f,
                    1f,
                    duration,
                    value => shinyEffect.location = value,
                    ease
                )
            )
            .ChainDelay(waitTime);
    }

    private void OnDestroy()
    {
        _sequence.Stop();
    }
}