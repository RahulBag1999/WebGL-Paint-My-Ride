using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : UiScreenBase
{
    [SerializeField] private Image header;
    [SerializeField] private Image coinHolder;
    [SerializeField] private Image pause;
    [SerializeField] private Image undo;
    [SerializeField] private Image gridBg;
    [SerializeField] private Image gameBg;
    [SerializeField] private Image gameBgOverlay;

    internal override void Cleanup()
    {

    }

    internal override void HandleGameStateChangeData(object[] data)
    {

    }
}