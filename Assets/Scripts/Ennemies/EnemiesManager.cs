using UnityEngine;
using System.Collections.Generic;

public static class EnemiesManager
{
    private static LinkedList<BigBadGuy> _enemies = new LinkedList<BigBadGuy> ();
    private static bool paused;

    public static void reset()
    {
        _enemies.Clear();
        paused = false;
    }

    public static void register(BigBadGuy bbg)
    {
        // Debug.Log("EnemiesManager register("+bbg+")");
        _enemies.AddLast(bbg);
    }

    public static void unregister(BigBadGuy bbg)
    {
        bool result = _enemies.Remove(bbg);
        // Debug.Log("EnemiesManager unregister("+bbg+") returned "+result);
    }

    public static bool Paused {
        get {
            return paused;
        }
        set {
            // Debug.Log("EnemiesManager Paused set to "+value);
            if (paused != value) {
                PauseAll (value);
            }
        }
    }

    private static void PauseAll (bool isPause)
    {
        // Debug.Log("EnemiesManager Pausell("+isPause+")");
        foreach (BigBadGuy bbg in _enemies) {
            if(null != bbg) {
                bbg.Pause (isPause);
            }
        }
        paused = isPause;
    }
}
