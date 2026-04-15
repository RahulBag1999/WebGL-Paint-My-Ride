using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = nameof(AudioConfig), menuName = "Scriptable Objects/" + nameof(AudioConfig))]
public class AudioConfig : EssentialConfigScriptableObject
{
    [SerializeField] private List<Sound> _audioList = new List<Sound>();

    public List<Sound> AudioList => _audioList;

    public override void Init()
    {
        
    }
}

[System.Serializable]
public class Sound
{
    [Header("Audio Name")]
    public string name;

    [Header("Audio Clip")]
    public AudioClip clip;

    [Header("Audio Type")]
    public AudioType audioType;

    [Header("Audio Volume")]
    public float volume = 1;

    [Header("Audio Pitch")]
    [Range(-3f, 3f)]
    public float pitch = 1;

    [Header("Will Audio Loop ? ")]
    public bool loop = false;

    [Header("Will Audio play on Awake ? ")]
    public bool playOnAwake = false;
}