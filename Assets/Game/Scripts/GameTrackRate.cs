﻿using System.Collections.Generic;
using UnityEngine;

public static class GameTrackRate
{
    private static float gameplayTimeBoss;
    private static int gameplayDeathCount;
    private static List<ItemData> gameplayItemUsed = new List<ItemData>();

    private static float startTime;
    private static float endTime;

    public static float Time
    {
        get{ return gameplayTimeBoss; }
    }

    public static int DeathCount
    {
        get { return gameplayDeathCount; }
        set { gameplayDeathCount += value;}
    }

    public static List<ItemData> ItemUsed
    {
        get { return gameplayItemUsed; }
        set { gameplayItemUsed = value;}
    }

    public static float StartTimePlay
    {
        set { startTime = value; }
    }

    public static float EndTimePlay
    {
        set { endTime = value; }
    }

    public static float CurrentTime
    {
        get { return UnityEngine.Time.time - startTime; }
    }

    public static void CalculateTime()
    {
        gameplayTimeBoss += (endTime - startTime);
    }
}