using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public static class PlayerDataHandler
{
    private static PlayerSaveData _player = new PlayerSaveData();
    private const string _playerSaveDataKey = "playerData";

    [JsonIgnore] public static ref PlayerSaveData Player => ref _player;

    public static void SavePlayerData()
    {
        SavePlayerDataLocal();
        try
        {

        }
        catch (Exception e)
        {
            Debug.LogWarning("Unable to save to cloud");
        }
    }

    private static void SavePlayerDataLocal()
    {
        _player.UpdateLastUpdateTime();
        string json = JsonConvert.SerializeObject(_player);
        Debug.Log("Saved locally at Persistent Data path:: " + Application.persistentDataPath);
        string filePath = Path.Combine(Application.persistentDataPath, _playerSaveDataKey + ".json");
        File.WriteAllText(filePath, json);
    }

    public static PlayerSaveData LoadPlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _playerSaveDataKey + ".json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            _player = JsonConvert.DeserializeObject<PlayerSaveData>(json);
            if (Equals(_player, null))
            {
                ResetPlayer();
            }
            Debug.LogWarning("Player data loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("Player data file not found at: " + filePath);
            ResetPlayer();
        }
        return _player;
    }

    private static void ResetPlayer()
    {
        _player = new PlayerSaveData();
        _player.SetIsFirstTime(true);
    }
}
