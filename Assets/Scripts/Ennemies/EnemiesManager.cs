using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesManager
{
    private static LinkedList<BigBadGuy> _enemies = new LinkedList<BigBadGuy> ();
    private static bool paused;

    public static void register(BigBadGuy bbg)
    {
        Logger.Log("EnemiesManager::register("+bbg+")");
        _enemies.AddLast(bbg);
    }

    public static void unregister(BigBadGuy bbg)
    {
        bool result = _enemies.Remove(bbg);
        Logger.Log("EnemiesManager::unregister("+bbg+") returned "+result);
    }

    public static bool Paused {
        get {
            return paused;
        }
        set {
            Logger.Log("EnemiesManager::Paused set to "+value);
            if (paused != value) {
                PauseAll (value);
            }
        }
    }

    private static void PauseAll (bool isPause)
    {
        Logger.Log("EnemiesManager::Pausell("+isPause+")");
        foreach (BigBadGuy bbg in _enemies) {
            if(null != bbg) {
                bbg.Pause (isPause);
            }
        }
        paused = isPause;
    }
}
