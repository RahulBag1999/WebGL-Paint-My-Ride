using System;
using System.Collections.Generic;

public sealed class GameHelper
{
    private static GameHelper _instance;
    private Dictionary<string, Action<object>> _myEventDictionary;

    private GameHelper()
    {
        _myEventDictionary = new Dictionary<string, Action<object>>();
    }

    internal static GameHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameHelper();
            }
            return _instance;
        }
    }

    internal void InvokeAction(string eventAction, object _obj = null)
    {
        if (_myEventDictionary.ContainsKey(eventAction))
        {
            _myEventDictionary[eventAction]?.Invoke(_obj);
        }
    }

    internal void StartListening(string eventName, Action<object> listener)
    {
        if (!_myEventDictionary.ContainsKey(eventName) && listener != null)
        {
            _myEventDictionary.Add(eventName, listener);
        }
        else
        {
            _myEventDictionary[eventName] -= listener;
            _myEventDictionary[eventName] += listener;
        }
    }

    internal void StopListening(string eventName, Action<object> listener = null)
    {
        if (_myEventDictionary.ContainsKey(eventName) && listener != null)
        {
            _myEventDictionary[eventName] -= listener;
        }
    }

    public static void CleanUpResources()
    {
        _instance._myEventDictionary.Clear();
        _instance._myEventDictionary = null;
        _instance = null;
    }
}