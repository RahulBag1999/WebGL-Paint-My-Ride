using UnityEngine;
using PrimeTween;
using System.Collections.Generic;
using TMPro;

public class CoinCollectAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private RectTransform targetPoint;
    [SerializeField] private RectTransform coinBox;
    [SerializeField] private RectTransform coinIcon;
    [SerializeField] private Transform coinParent;
    [SerializeField] private GameObject coinPrefab;

    [Header("Settings")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float spreadRadius = 100f;
    [SerializeField] private float spawnDelay = 0.05f;

    [Header("Coin Counter")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text deductCoinText;

    private int currentCoins;
    private int targetCoins;
    private int rewardCoins;
    private int coinsAddedPerHit;
    private int remainingCoins;
    private int deductedCoins;

    private int coinCount;
    private float spawnTimer;
    private int spawnedCoins;
    private bool isPlaying;

    private readonly List<(GameObject coin, Sequence seq)> activeCoins = new();

    private Sequence coinBoxPunchSequence;

    private void Start()
    {
        currentCoins = (int)PlayerDataHandler.Player.GameCurrency.Coins;
        coinText.text = currentCoins.ToString("N0");
    }

    public void PlayAnimation(int totalRewardCoins)
    {
        StopAnimation();

        rewardCoins = totalRewardCoins;

        // Final wallet amount
        targetCoins = currentCoins + rewardCoins;

        // Visual coin count
        coinCount = Mathf.Clamp(rewardCoins, 5, 20);

        // Value per visual coin
        coinsAddedPerHit = Mathf.CeilToInt((float)rewardCoins / coinCount);

        remainingCoins = rewardCoins;

        deductCoinText.text = remainingCoins.ToString("N0");

        spawnTimer = 0f;
        spawnedCoins = 0;
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnedCoins < coinCount && spawnTimer >= spawnDelay)
        {
            spawnTimer = 0f;
            SpawnSingleCoin(spawnedCoins);
            spawnedCoins++;
        }

        if (spawnedCoins >= coinCount && activeCoins.Count == 0)
        {
            isPlaying = false;
            Debug.Log("Coin animation complete");
        }
    }

    private void SpawnSingleCoin(int index)
    {
        GameObject coin = Instantiate(coinPrefab, coinParent);
        RectTransform coinRect = coin.GetComponent<RectTransform>();

        coin.transform.SetAsLastSibling();

        RectTransform parentRect = coinParent as RectTransform;

        Vector2 startPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, spawnPoint.position),
            null,
            out startPos
        );

        Vector2 endPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, targetPoint.position),
            null,
            out endPos
        );

        coinRect.anchoredPosition = startPos;
        coinRect.localScale = Vector3.one;
        coinRect.localRotation = Quaternion.identity;

        Vector2 bounceDown = startPos + new Vector2(
            Random.Range(-30f, 30f),
            -Random.Range(40f, 80f)
        );

        Sequence seq = Sequence.Create()

            // Step 1: small drop
            .Chain(
                Tween.UIAnchoredPosition(
                    coinRect,
                    bounceDown,
                    0.15f,
                    Ease.InQuad))

            // Step 2: fly to target
            .Chain(
                Tween.UIAnchoredPosition(
                    coinRect,
                    endPos,
                    duration,
                    Ease.OutQuad))

            .Group(
                Tween.Scale(
                    coinRect,
                    Vector3.one * 0.6f,
                    duration))

            .Group(
                Tween.Rotation(
                    coinRect,
                    Quaternion.Euler(0f, 0f, 360f),
                    duration));

        activeCoins.Add((coin, seq));

        seq.OnComplete(() =>
        {
            activeCoins.RemoveAll(x => x.coin == coin);

            AddCoins(coinsAddedPerHit);

            PunchCoinBox();
            PunchCoinIcon();

            Destroy(coin);
        });
    }

    private void AddCoins(int amount)
    {
        int previousCoins = currentCoins;

        currentCoins += amount;

        // Prevent overshoot
        if (currentCoins > targetCoins)
            currentCoins = targetCoins;

        // Animate wallet number
        Tween.Custom(
            previousCoins,
            currentCoins,
            0.15f,
            value =>
            {
                coinText.text = Mathf.RoundToInt(value).ToString("N0");
            });

        // Wallet text zoom
        Sequence.Create()
            .Chain(
                Tween.Scale(
                    coinText.transform,
                    Vector3.one * 1.25f,
                    0.12f,
                    Ease.OutQuad))
            .Chain(
                Tween.Scale(
                    coinText.transform,
                    Vector3.one,
                    0.12f,
                    Ease.InQuad));

        // ======================
        // Deduct animation
        // ======================

        int previousRemaining = remainingCoins;

        remainingCoins -= amount;

        if (remainingCoins < 0)
            remainingCoins = 0;

        Tween.Custom(
            previousRemaining,
            remainingCoins,
            0.15f,
            value =>
            {
                deductCoinText.text = Mathf.RoundToInt(value).ToString("N0");
            });

        Sequence.Create()
            .Chain(
                Tween.Scale(
                    deductCoinText.transform,
                    Vector3.one * 1.2f,
                    0.1f,
                    Ease.OutQuad))
            .Chain(
                Tween.Scale(
                    deductCoinText.transform,
                    Vector3.one,
                    0.1f,
                    Ease.InQuad));

        if (remainingCoins <= 0)
        {
            remainingCoins = 0;
            deductCoinText.text = "0";

            // Small completion pop
            Sequence.Create()
                .Chain(
                    Tween.Scale(
                        deductCoinText.transform,
                        Vector3.one * 1.15f,
                        0.1f,
                        Ease.OutBack))
                .Chain(
                    Tween.Scale(
                        deductCoinText.transform,
                        Vector3.one,
                        0.1f,
                        Ease.InOutQuad));

            // Update actual wallet
            PlayerDataHandler.Player.GameCurrency.UpdateCoin(rewardCoins);
        }
    }    

    private void PunchCoinBox()
    {
        coinBoxPunchSequence.Stop();

        coinBox.localScale = Vector3.one;

        coinBoxPunchSequence = Sequence.Create()
            .Chain(
                Tween.Scale(
                    coinBox,
                    Vector3.one * 1.12f,
                    0.08f,
                    Ease.OutQuad))
            .Chain(
                Tween.Scale(
                    coinBox,
                    Vector3.one * 0.95f,
                    0.05f,
                    Ease.InQuad))
            .Chain(
                Tween.Scale(
                    coinBox,
                    Vector3.one,
                    0.08f,
                    Ease.OutBack));
    }

    private void PunchCoinIcon()
    {
        Sequence.Create()
            .Chain(
                Tween.Scale(
                    coinIcon,
                    Vector3.one * 1.25f,
                    0.08f,
                    Ease.OutBack))
            .Chain(
                Tween.Scale(
                    coinIcon,
                    Vector3.one,
                    0.10f,
                    Ease.OutQuad));
    }

    [ContextMenu("Stop")]
    public void StopAnimation()
    {
        isPlaying = false;

        foreach (var item in activeCoins)
        {
            item.seq.Stop();

            if (item.coin != null)
                Destroy(item.coin);
        }

        activeCoins.Clear();
    }
}