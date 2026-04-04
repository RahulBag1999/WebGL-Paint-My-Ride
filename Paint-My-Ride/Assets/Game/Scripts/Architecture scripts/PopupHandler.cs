using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHandler : MonoBehaviour
{
    [SerializeField] private List<UiPopupBase> _uiPopups = new List<UiPopupBase>();

    private Dictionary<Type, UiPopupBase> _uiScreenCollection = new Dictionary<Type, UiPopupBase>();
    private Stack<UiPopupBase> _currentActivePopups = new Stack<UiPopupBase>();

    public void Init()
    {
        for (int i = 0; i < _uiPopups.Count; i++)
        {
            _uiPopups[i].Init(this);
            _uiScreenCollection.Add(_uiPopups[i].GetType(), _uiPopups[i]);
        }
    }

    internal void ShowPopup<T>(bool isRenderOverExistingPopups, params object[] data) where T : UiPopupBase
    {
        if (!isRenderOverExistingPopups && _currentActivePopups.Count > 0)
        {
            foreach (var popup in _currentActivePopups)
            {
                _currentActivePopups.Peek().SetPopupVisibility(false);
                _currentActivePopups.Pop();
            }
        }
        _currentActivePopups.Push(_uiScreenCollection[typeof(T)]);
        _currentActivePopups.Peek().SetPopupVisibility(true);
        _currentActivePopups.Peek().HandlePopupToggleData(true, data);
    }

    internal void HidePopup()
    {
        _currentActivePopups?.Peek().SetPopupVisibility(false);
        _currentActivePopups?.Peek().HandlePopupToggleData(false, null);
        _currentActivePopups?.Pop();
    }

    public void Cleanup()
    {

    }

    public void ChangeGameState(GameStates newGameState, object data = null)
    {

    }

    public void RegisterInputs()
    {

    }

    public void UnregisterInputs()
    {

    }
}