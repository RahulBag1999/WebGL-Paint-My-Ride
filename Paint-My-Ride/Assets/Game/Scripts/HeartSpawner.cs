using UnityEngine;

public class HeartSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private float _spawnDelayMin = 0.2f;
    [SerializeField] private float _spawnDelayMax = 0.4f;

    private int spawnIndex = 0;

    private float timer;
    private float currentDelay;
    private bool isPlaying = false;

    [ContextMenu("Play")]
    public void Play()
    {
        isPlaying = true;
        timer = 0f;
        SetNextDelay();
    }

    public void Stop()
    {
        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying) return;

        timer += Time.deltaTime;

        if (timer >= currentDelay)
        {
            timer = 0f;
            Spawn();
            SetNextDelay();
        }
    }

    private void SetNextDelay()
    {
        currentDelay = Random.Range(_spawnDelayMin, _spawnDelayMax);
    }

    private void Spawn()
    {
        GameObject heart = Instantiate(_heartPrefab, _parent);

        RectTransform parentRect = _parent as RectTransform;

        //Vector2 spawnPos = new Vector2(
        //    Random.Range(-50f, 50f),
        //    -300f
        //);

        int direction = (spawnIndex % 2 == 0) ? 1 : -1;

        heart.GetComponent<HeartUiItem>().Play(_parent.position, direction);

        spawnIndex++;
    }

    // Stop everything instantly (including active hearts)
    public void StopAll()
    {
        isPlaying = false;

        foreach (Transform child in _parent)
        {
            HeartUiItem item = child.GetComponent<HeartUiItem>();
            if (item != null)
            {
                item.StopAnimation();
            }
        }
    }
}