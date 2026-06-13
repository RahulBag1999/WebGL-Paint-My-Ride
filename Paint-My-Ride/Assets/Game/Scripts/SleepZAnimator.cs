using UnityEngine;
using PrimeTween;
using System.Collections.Generic;

public class SleepZAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform zPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnDelay = 0.5f;

    [Header("Movement")]
    [SerializeField] private Vector2 moveOffset = new Vector2(-80f, 150f);
    [SerializeField] private float duration = 2f;

    [Header("Scale")]
    [SerializeField] private float endScale = 0.5f;
    [SerializeField] private float maxScale = 1.1f;

    private bool isRunning = false;
    private float timer = 0f;

    private readonly List<(RectTransform z, Sequence seq)> activeSequences = new();

    private void Update()
    {
        if (!isRunning)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnDelay)
        {
            timer -= spawnDelay;
            SpawnZ();
        }
    }

    [ContextMenu("Pplay")]
    public void Play()
    {
        isRunning = true;
        timer = 0f;
    }

    public void Stop(bool killExisting = false)
    {
        isRunning = false;
        timer = 0f;

        if (killExisting)
        {
            foreach (var item in activeSequences)
            {
                item.seq.Stop();

                if (item.z != null)
                    Destroy(item.z.gameObject);
            }

            activeSequences.Clear();
        }
    }

    private void SpawnZ()
    {
        RectTransform z = Instantiate(zPrefab, spawnParent);

        z.anchoredPosition = Vector2.zero;

        // Start smaller
        z.localScale = Vector3.one * 0.4f;
        z.localRotation = Quaternion.identity;

        CanvasGroup cg = z.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = z.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 1f;

        float randomRotation = Random.Range(-15f, 15f);

        Sequence rotationSequence = Sequence.Create()
            .Chain(
                Tween.LocalRotation(
                    z,
                    Quaternion.Euler(0f, 0f, randomRotation),
                    duration * 0.5f,
                    Ease.InOutSine))
            .Chain(
                Tween.LocalRotation(
                    z,
                    Quaternion.identity,
                    duration * 0.5f,
                    Ease.InOutSine));

        Sequence seq = Sequence.Create()

            // Grow while moving
            .Group(
                Tween.Scale(
                    z,
                    Vector3.one * maxScale,
                    duration,
                    Ease.OutBack))

            .Group(
                Tween.UIAnchoredPosition(
                    z,
                    moveOffset,
                    duration,
                    Ease.OutSine))

            .Group(
                Tween.Alpha(
                    cg,
                    0f,
                    duration,
                    Ease.OutQuad))

            .Group(rotationSequence);

        activeSequences.Add((z, seq));

        seq.OnComplete(() =>
        {
            activeSequences.RemoveAll(x => x.z == z);

            if (z != null)
                Destroy(z.gameObject);
        });
    }

    private void OnDestroy()
    {
        Stop(true);
    }
}