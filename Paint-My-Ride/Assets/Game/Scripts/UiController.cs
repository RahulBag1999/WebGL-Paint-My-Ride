using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : Singleton<UiController>
{
    [SerializeField] private List<UiScreenBase> _uiScreenList = new List<UiScreenBase>();
    private Dictionary<Type, UiScreenBase> _uiScreenDict = new Dictionary<Type, UiScreenBase>();

    private UiScreenBase _currentScreen;

    protected override void OnInit()
    {
        base.OnInit();

        foreach (var uiScreen in _uiScreenList)
        {
            _uiScreenDict.Add(uiScreen.GetType(), uiScreen);
        }
        _currentScreen = null;
    }

    public void ShowScreen<T>() where T : UiScreenBase
    {
        if (_currentScreen != null)
        {
            _currentScreen.SetCanvasVisibility(false);
        }
        _currentScreen = _uiScreenDict[typeof(T)];
        _currentScreen.SetCanvasVisibility(true);
        Debug.Log("Current screen type:: " + _currentScreen.GetType().FullName);
    }
}
