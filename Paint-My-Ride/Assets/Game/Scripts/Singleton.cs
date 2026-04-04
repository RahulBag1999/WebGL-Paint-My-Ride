using UnityEngine;

/// <summary>
/// Generic, persistent Singleton. Put this on a component you want only one of.
/// Usage: public class AudioManager : Singleton<AudioManager> { }
/// </summary>
[DefaultExecutionOrder(-100)] // initialize early
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    private static bool _applicationIsQuitting;

    /// <summary>Is the singleton currently created?</summary>
    public static bool IsInitialized => _instance != null;

    /// <summary>
    /// Access the instance. If none exists, one will be created on a new GameObject.
    /// Returns null after the app begins quitting.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting) return null;

            if (_instance == null)
            {
                // Try find existing in scene first
                _instance = FindObjectOfType<T>();

                // If still none, create a new GameObject and attach
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name + " (Singleton)");
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // Another instance already exists => destroy this duplicate
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;
        DontDestroyOnLoad(gameObject);
        OnInit(); // optional hook for derived classes
    }

    /// <summary>Optional init hook for subclasses instead of Awake().</summary>
    protected virtual void OnInit() { }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
