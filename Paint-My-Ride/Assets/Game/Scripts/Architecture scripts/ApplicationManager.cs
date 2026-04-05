using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    [SerializeField] private ApplicationHandler _applicationHandler;
    [SerializeField] private PopupHandler _popupHandler;

    private PlayerSaveData _playerSaveData;
    private EssentialConfigData _essentialConfigData;

    private void Awake()
    {
        _playerSaveData = PlayerDataHandler.LoadPlayerData();
        _essentialConfigData = Resources.Load<EssentialConfigData>(nameof(EssentialConfigData));
        _essentialConfigData.Init();
        _popupHandler.Init(_essentialConfigData);
        _applicationHandler.Init(_popupHandler, _essentialConfigData);
    }

    private void SaveGame()
    {
        if (_playerSaveData.IsFirstTime)
        {
            _playerSaveData.SetIsFirstTime(false);
        }
        PlayerDataHandler.SavePlayerData();
    }

    private void OnDisable()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }
}