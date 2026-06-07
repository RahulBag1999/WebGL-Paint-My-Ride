using UnityEngine;
using UnityEngine.UI;

public class HandPointer : MonoBehaviour
{
    [SerializeField] private MultiSpriteAnimator _anim;
    [SerializeField] private Image _renderer;

    private GameSettings _gameSettings;

    private void Start()
    {
        _anim.Play("HandPoint", "Idle");
    }

    public void Init(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public void UpdateHandpointer(int tutStep)
    {
        SetPosition(_gameSettings.tutorialDataList[tutStep - 1].handPos);
    }

    public void SetVisibility(bool isVisible)
    {
        _renderer.enabled = isVisible;

        if (isVisible)
        {
            _anim.Play("HandPoint", "Idle");
        }
        else
        {
            _anim.Stop("HandPoint");
        }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y);
    }

    public void Cleanup()
    {
        Destroy(gameObject);
    }
}