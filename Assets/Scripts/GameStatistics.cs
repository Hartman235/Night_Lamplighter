using UnityEngine;
using System;

[System.Serializable]
public class GameStatistics
{
    public int totalGamesPlayed;        // Всего игр
    public int totalCoinsCollected;     // Всего монет
    public int totalBatteriesCollected; // Всего батареек
    public float totalDistanceRun;      // Общая дистанция
    public float totalPlayTime;          // Общее время в игре
    public int bestDistance;             // Лучший результат (уже есть как recordScore)
    
    // Для сериализации
    public string lastUpdated;
    
    public GameStatistics()
    {
        totalGamesPlayed = 0;
        totalCoinsCollected = 0;
        totalBatteriesCollected = 0;
        totalDistanceRun = 0;
        totalPlayTime = 0;
        bestDistance = 0;
        lastUpdated = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    }
    
    // Обновить время последнего обновления
    public void UpdateTimestamp()
    {
        lastUpdated = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    }
}