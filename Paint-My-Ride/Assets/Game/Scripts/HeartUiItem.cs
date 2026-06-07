using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class HeartUiItem : MonoBehaviour
{
    [SerializeField] private float stepHeight = 120f;
    [SerializeField] private float stepWidth = 80f;
    [SerializeField] private float stepDuration = 0.5f;

    private RectTransform rect;
    private Image img;

    private Sequence sequence;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    public void Play(Vector2 startPos, int direction)
    {
        rect.anchoredPosition = startPos;

        float width = Random.Range(stepWidth * 0.8f, stepWidth * 1.2f);
        float duration = Random.Range(stepDuration * 0.8f, stepDuration * 1.2f);

        rect.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);

        float scale1 = 0.8f;
        float scale2 = 0.6f;
        float scale3 = 0.3f;

        int dir = direction; // +1 or -1

        sequence = Sequence.Create();

        // 🔹 Step 1
        sequence.Group(Tween.UIAnchoredPosition(
            rect,
            startPos + new Vector2(dir * width, stepHeight),
            duration,
            Ease.Linear));

        sequence.Group(Tween.Rotation(
            rect,
            Quaternion.Euler(0, 0, -20f * dir),
            duration));

        sequence.Group(Tween.Scale(
            rect,
            Vector3.one * scale1,
            duration));

        // 🔹 Step 2
        sequence.ChainDelay(duration);

        sequence.Group(Tween.UIAnchoredPosition(
            rect,
            startPos + new Vector2(-dir * width, stepHeight * 2),
            duration,
            Ease.Linear));

        sequence.Group(Tween.Rotation(
            rect,
            Quaternion.Euler(0, 0, 20f * dir),
            duration));

        sequence.Group(Tween.Scale(
            rect,
            Vector3.one * scale2,
            duration));

        // 🔹 Step 3
        sequence.ChainDelay(duration);

        sequence.Group(Tween.UIAnchoredPosition(
            rect,
            startPos + new Vector2(dir * width * 0.5f, stepHeight * 3),
            duration,
            Ease.Linear));

        sequence.Group(Tween.Rotation(
            rect,
            Quaternion.Euler(0, 0, -15f * dir),
            duration));

        sequence.Group(Tween.Scale(
            rect,
            Vector3.one * scale3,
            duration));

        // 🔥 Fade starts after Step 1 and lasts through Step 2 + Step 3
        sequence.Insert(
            duration,
            Tween.Alpha(
                img,
                0f,
                duration * 2f));

        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void StopAnimation()
    {
        sequence.Stop();

        Destroy(gameObject);
    }
}