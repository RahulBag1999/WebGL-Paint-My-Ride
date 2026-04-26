using UnityEngine;

public class HandPointer : MonoBehaviour
{
    [SerializeField] private MultiSpriteAnimator _anim;
    [SerializeField] private SpriteRenderer _renderer;

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
        transform.position = pos;
    }

    public void Cleanup()
    {
        Destroy(gameObject);
    }
}