using UnityEngine;
using UnityEngine.UI;

public class TransitionColor : MonoBehaviour
{
    [SerializeField] private Image _bg;
    [SerializeField] private GameThemeData _themeData;
    
    void Start()
    {
        //_bg.color = _themeData.GetGameTheme(GameConstants.CurrentGameThemeId).themeColor;
    }
}