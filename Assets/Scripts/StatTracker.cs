using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour
{
    static float s_startTime;
    static int   s_playerDeaths;
    static int   s_enemiesKilled;

    public static int PlayerDeaths
    {
        get => s_playerDeaths;
    }

    public static int EnemiesKilled
    {
        get => s_enemiesKilled;
    }

    public static void ResetStats()
    {
        s_playerDeaths  = 0;
        s_enemiesKilled = 0;
        s_startTime     = Time.time;
    }

    public static float GetPlayTime()
    {
        return Time.time - s_startTime;
    }

    public static void IncrementPlayerDeath()
    {
        s_playerDeaths++;
    }

    public static void IncrementEnemiesKilled()
    {
        s_enemiesKilled++;
    }
}
