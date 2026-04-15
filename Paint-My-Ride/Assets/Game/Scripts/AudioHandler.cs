using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioHandler : Singleton<AudioHandler>
{
    [SerializeField] private AudioManager _audioManager;

    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _soundToggle;

    internal void Init(EssentialConfigData essentialConfigData)
    {
        _audioManager.Init(essentialConfigData);
    }

    public void HandleMusicState(bool musicState)
    {
        _musicToggle.isOn = musicState;
        _audioManager.MusicAudioMixerGroup.audioMixer.SetFloat(GameConstants.MUSIC_KEY, musicState ? 0f : -80f);
        PlayerDataHandler.Player.UserSettingsPreferences.UpdateMusicStatus(musicState);
    }

    public void HandleSfxState(bool sfxState)
    {
        _soundToggle.isOn = sfxState;
        _audioManager.SfxAudioMixerGroup.audioMixer.SetFloat(GameConstants.SFX_KEY, sfxState ? 0f : -80f);
        PlayerDataHandler.Player.UserSettingsPreferences.UpdateSFXStatus(sfxState);
    }

    internal void Cleanup()
    {
        _audioManager.Cleanup();
    }
}