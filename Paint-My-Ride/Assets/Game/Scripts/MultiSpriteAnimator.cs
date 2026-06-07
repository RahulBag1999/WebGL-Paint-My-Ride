using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RenderTargetType
{
    SpriteRenderer,
    UIImage
}

[Serializable]
public class SpriteAnimationChannel
{
    public string channelName;

    public RenderTargetType targetType;

    public SpriteRenderer spriteRenderer;
    public Image uiImage;

    public List<MultiSpriteAnimationData> animations;

    [HideInInspector] public Sprite[] frames;
    [HideInInspector] public float sampleRate;
    [HideInInspector] public bool loop;

    [HideInInspector] public int currentFrame;
    [HideInInspector] public float timer;
    [HideInInspector] public bool isPlaying;

    [HideInInspector] public Action onComplete;

    public void SetSprite(Sprite sprite)
    {
        if (targetType == RenderTargetType.SpriteRenderer)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = sprite;
        }
        else
        {
            if (uiImage != null)
                uiImage.sprite = sprite;
        }
    }
}

public class MultiSpriteAnimator : MonoBehaviour
{
    [SerializeField] private List<SpriteAnimationChannel> channels;
    [SerializeField] private float speedMultiplier = 1f;

    private void Update()
    {
        foreach (var ch in channels)
        {
            if (!ch.isPlaying || ch.frames == null || ch.frames.Length == 0)
                continue;

            float frameDuration = 1f / (ch.sampleRate * speedMultiplier);
            ch.timer += Time.deltaTime;

            while (ch.timer >= frameDuration)
            {
                ch.timer -= frameDuration;
                ch.currentFrame++;

                if (ch.currentFrame >= ch.frames.Length)
                {
                    if (ch.loop)
                    {
                        ch.currentFrame = 0;
                    }
                    else
                    {
                        ch.isPlaying = false;
                        ch.onComplete?.Invoke();
                        break;
                    }
                }

                if (ch.isPlaying)
                    ch.SetSprite(ch.frames[ch.currentFrame]);
            }
        }
    }

    // 🔹 Play (Inspector-driven)
    public void Play(string channelName, string animationName)
    {
        var ch = GetChannel(channelName);
        if (ch == null) return;

        var anim = GetAnimation(ch, animationName);
        if (anim == null) return;

        ch.frames = anim.frames;
        ch.sampleRate = anim.sampleRate;
        ch.loop = anim.loop;

        ch.currentFrame = 0;
        ch.timer = 0f;
        ch.isPlaying = true;
        ch.onComplete = null;

        ch.SetSprite(ch.frames[0]);
    }

    // 🔹 One-shot (non-loop)
    public void PlayOneShot(string channelName, string animationName, Action onComplete = null)
    {
        var ch = GetChannel(channelName);
        if (ch == null) return;

        var anim = GetAnimation(ch, animationName);
        if (anim == null) return;

        ch.frames = anim.frames;
        ch.sampleRate = anim.sampleRate;
        ch.loop = false;

        ch.currentFrame = 0;
        ch.timer = 0f;
        ch.isPlaying = true;
        ch.onComplete = onComplete;

        ch.SetSprite(ch.frames[0]);
    }

    public void Stop(string channelName)
    {
        var ch = GetChannel(channelName);
        if (ch != null)
            ch.isPlaying = false;
    }

    // ------------------------

    private SpriteAnimationChannel GetChannel(string name)
    {
        foreach (var ch in channels)
        {
            if (ch.channelName == name)
                return ch;
        }

        Debug.LogWarning($"Channel {name} not found!");
        return null;
    }

    private MultiSpriteAnimationData GetAnimation(SpriteAnimationChannel ch, string name)
    {
        if (ch.animations == null) return null;

        foreach (var anim in ch.animations)
        {
            if (anim.animationName == name)
                return anim;
        }

        Debug.LogWarning($"Animation {name} not found in channel {ch.channelName}");
        return null;
    }
}