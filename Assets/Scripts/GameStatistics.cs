using UnityEngine;
using System;

[System.Serializable]
public class GameStatistics
{
    public int totalGamesPlayed;        
    public int totalCoinsCollected;    
    public int totalBatteriesCollected; 
    public float totalDistanceRun;     
    public float totalPlayTime;          
    public int bestDistance;             
    
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
    
    public void UpdateTimestamp()
    {
        lastUpdated = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    }
}