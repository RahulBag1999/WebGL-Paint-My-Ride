using UnityEngine;

[CreateAssetMenu(fileName = "MultiSpriteAnimationData", menuName = "Scriptable Objects/" + nameof(MultiSpriteAnimationData))]
public class MultiSpriteAnimationData : ScriptableObject
{
    public string animationName;
    public Sprite[] frames;
    public float sampleRate = 12f;
    public bool loop = true;
}