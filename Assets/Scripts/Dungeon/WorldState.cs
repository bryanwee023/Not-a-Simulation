using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState
{

    public static int worldLevel= 0;

    public static float startTime;

    public static int roomsCleared = 0;

    public static bool paused = false;

    public static float atkScale { get { return 0.9f + worldLevel * 0.1f; } }
    public static float hpScale { get { return 0.77f + worldLevel * 0.23f; } }

    public static void Reset()
    {
        worldLevel = 0;
        startTime = Time.time;
        roomsCleared = 0;

        EnemyController.ResetCount();
    }

}
