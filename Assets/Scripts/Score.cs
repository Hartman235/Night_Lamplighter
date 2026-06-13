using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public Text scoreText;
    public int totalScore;
    public int scoreMultiplier;

    [Header("Speed Settings")]
    [SerializeField] private PlayerController playerController;
    public float baseScoreRate = 0.1f;

    private int permanentExtraCoins = 0;
    private float temporaryCoinMultiplier = 1f;
    private Coroutine coinMultiplierCoroutine;

    private float scoreBuffer = 0f;
    private float currentRate;

    private void Start()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (playerController != null)
        {
            float speed = playerController.GetCurrentSpeed();
            float speedFactor = speed / 20f;
            currentRate = baseScoreRate * speedFactor * scoreMultiplier;
        }
        else
        {
            currentRate = baseScoreRate * scoreMultiplier;
        }
    }


    private void FixedUpdate()
    {
        scoreBuffer += currentRate;
        if (scoreBuffer >= 1f)
        {
            int pointsToAdd = Mathf.FloorToInt(scoreBuffer);
            totalScore += pointsToAdd;
            scoreBuffer -= pointsToAdd;
            scoreText.text = totalScore.ToString();
        }
    }

    public float GetCurrentScoreRate() => currentRate;

    public void AddPermanentCoinBonus(int extra)   
    {
        permanentExtraCoins += extra;
    }

    public void AddTemporaryCoinMultiplier(float multiplier, float duration)
    {
        if (coinMultiplierCoroutine != null)
            StopCoroutine(coinMultiplierCoroutine);
        temporaryCoinMultiplier = multiplier;
        coinMultiplierCoroutine = StartCoroutine(RevertCoinMultiplierAfterTime(duration));
    }

    private IEnumerator RevertCoinMultiplierAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        temporaryCoinMultiplier = 1f;
        coinMultiplierCoroutine = null;
    }

    public int GetBaseCoinValue() => 1 + permanentExtraCoins;
    public float GetTemporaryCoinMultiplier() => temporaryCoinMultiplier;
    public float GetCoinMultiplier()
    {
        return permanentExtraCoins * temporaryCoinMultiplier;
    }
}