using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public Text scoreText;
    private int totalScore;

    public int scoreMultiplier;
    
    [Header("Speed Settings")]
    [SerializeField] private PlayerController playerController; // Ссылка на игрока
    public float baseScoreRate = 0.1f; // Базовый множитель
    
    private float scoreBuffer = 0f;
    private float currentRate;

    private void Start()
    {
        // Автоматически находим игрока, если не назначен
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        // Считаем текущий темп набора очков
        if (playerController != null)
        {
            float speed = playerController.GetCurrentSpeed();
            // Чем выше скорость, тем быстрее счет
            float speedFactor = speed / 20f; // 20 - базовая скорость
            currentRate = baseScoreRate * speedFactor * scoreMultiplier;
        }
        else
        {
            currentRate = baseScoreRate * scoreMultiplier;
        }
    }

    private void FixedUpdate()
    {
        // Добавляем очки с учетом текущего темпа
        scoreBuffer += currentRate;
        
        if (scoreBuffer >= 1f)
        {
            int pointsToAdd = Mathf.FloorToInt(scoreBuffer);
            totalScore += pointsToAdd;
            scoreBuffer -= pointsToAdd;
            scoreText.text = totalScore.ToString();
        }
    }
    
    public float GetCurrentScoreRate()
    {
        return currentRate;
    }
}