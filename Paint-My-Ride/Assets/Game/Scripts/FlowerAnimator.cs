using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerAnimator : MonoBehaviour
{
    [SerializeField] private List<Image> set1 = new();
    [SerializeField] private List<Image> set2 = new();

    [SerializeField] private GameThemeData themeData;
    [SerializeField] private CanvasGroup canvasGroup;

    private MultiSpriteAnimationData dataSet1;
    private MultiSpriteAnimationData dataSet2;

    private int themeIndex = 0;

    private int currentFrame1;
    private int currentFrame2;

    private float timer1;
    private float timer2;

    private bool isAnimationPlaying = true;

    public void Init(int themeId)
    {
        if(themeId == 0 || themeId == 2)
        {
            themeIndex = themeId;
            canvasGroup.alpha = 1;
            var theme = themeData.GetGameTheme(themeIndex);

            dataSet1 = theme.propsDataSet1;
            dataSet2 = theme.propsDataSet2;

            ApplyFrame(set1, dataSet1, 0);
            ApplyFrame(set2, dataSet2, 0);

            PlayAllAnimations();
        }
        else
        {
            StopAllAnimations();
            canvasGroup.alpha = 0;
            return;
        }
    }

    private void Update()
    {
        if (!isAnimationPlaying)
            return;

        UpdateAnimation(
            dataSet1,
            ref timer1,
            ref currentFrame1,
            set1);

        UpdateAnimation(
            dataSet2,
            ref timer2,
            ref currentFrame2,
            set2);
    }

    private void UpdateAnimation(
        MultiSpriteAnimationData animationData,
        ref float timer,
        ref int currentFrame,
        List<Image> targets)
    {
        if (animationData == null ||
            animationData.frames == null ||
            animationData.frames.Length == 0)
            return;

        timer += Time.deltaTime;

        float frameDuration = 1f / animationData.sampleRate;

        if (timer < frameDuration)
            return;

        timer -= frameDuration;

        currentFrame++;

        if (currentFrame >= animationData.frames.Length)
        {
            if (animationData.loop)
            {
                currentFrame = 0;
            }
            else
            {
                currentFrame = animationData.frames.Length - 1;
            }
        }

        ApplyFrame(targets, animationData, currentFrame);
    }

    private void ApplyFrame(
        List<Image> targets,
        MultiSpriteAnimationData animationData,
        int frameIndex)
    {
        Sprite sprite = animationData.frames[frameIndex];

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
                targets[i].sprite = sprite;
        }
    }

    public void StopAllAnimations()
    {
        isAnimationPlaying = false;
    }

    public void PlayAllAnimations()
    {
        isAnimationPlaying = true;
    }

    public void RestartAllAnimations()
    {
        currentFrame1 = 0;
        currentFrame2 = 0;

        timer1 = 0f;
        timer2 = 0f;

        isAnimationPlaying = true;

        if (dataSet1 != null && dataSet1.frames.Length > 0)
            ApplyFrame(set1, dataSet1, 0);

        if (dataSet2 != null && dataSet2.frames.Length > 0)
            ApplyFrame(set2, dataSet2, 0);
    }
}