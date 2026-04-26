using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHandler : MonoBehaviour
{
    [SerializeField] private List<UiPopupBase> _uiPopups = new List<UiPopupBase>();

    private Dictionary<Type, UiPopupBase> _uiScreenCollection = new Dictionary<Type, UiPopupBase>();
    private Stack<UiPopupBase> _currentActivePopups = new Stack<UiPopupBase>();

    public void Init(EssentialConfigData essentialConfigData)
    {
        for (int i = 0; i < _uiPopups.Count; i++)
        {
            _uiPopups[i].Init(this, essentialConfigData);
            _uiScreenCollection.Add(_uiPopups[i].GetType(), _uiPopups[i]);
        }
    }

    internal void ShowPopup<T>(bool isRenderOverExistingPopups, Action onComplete = null, params object[] data) where T : UiPopupBase
    {
        if (!isRenderOverExistingPopups && _currentActivePopups.Count > 0)
        {
            foreach (var popup in _currentActivePopups)
            {
                _currentActivePopups.Peek().SetPopupVisibility(false, onComplete);
                _currentActivePopups.Pop();
            }
        }
        _currentActivePopups.Push(_uiScreenCollection[typeof(T)]);
        _currentActivePopups.Peek().SetPopupVisibility(true, onComplete);
        _currentActivePopups.Peek().HandlePopupToggleData(true, data);
    }

    internal void HidePopup(Action OnStarted = null, Action OnComplete = null)
    {
        OnStarted?.Invoke();
        _currentActivePopups?.Peek().SetPopupVisibility(false, OnComplete);
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