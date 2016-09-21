using UnityEngine;
using System.Collections.Generic;

public class MineManager : MonoBehaviour
{


    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private List<ResettableMine> _minesToReset = new List<ResettableMine>();
    private List<GameObject> _particleSystems = new List<GameObject>();
    public static string gameObjectName = "MineManager";
    private static MineManager _instance;
    public static MineManager get()
    {
        if (_instance == null)
        {
            Logger.Log("MineManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<MineManager>();
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("MineManager::Awake", Logger.Level.DEBUG);
        _instance = this;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    //public TextAsset sceneFilePath3;

    [SerializeField]
    private GameObject _mine;

    public void detonate(ResettableMine mine)
    {
        Debug.Log(mine.gameObject.name + " detonates");
        _minesToReset.Add(mine);
        GameObject particleSystem = Resources.Load("ExplosionParticleSystem") as GameObject;
        GameObject instance = Instantiate(particleSystem, new Vector3(mine.transform.position.x, mine.transform.position.y + 10, mine.transform.position.z), mine.transform.rotation) as GameObject;
        _particleSystems.Add(instance);
    }

    public void resetSelectedMine(ResettableMine mine, bool reseting)
    {
        GameObject target = mine.gameObject;
        float x = target.transform.position.x;
        float z = target.transform.position.z;

        iTween.Stop(target, true);
        Destroy(target);
        if (reseting)
        {
            GameObject go = (GameObject)Instantiate(_mine, new Vector3(x, 0, z), Quaternion.identity);
            ResettableMine newMine = (ResettableMine)go.GetComponent<ResettableMine>();
            newMine.mineName = mine.mineName;
        }
    }

    public void resetAllMines()
    {
        Debug.Log("resetAllMines");
        foreach (ResettableMine mine in _minesToReset)
        {
            Debug.Log("reset " + mine.gameObject.name);
            resetSelectedMine(mine, true);
        }

        foreach (GameObject particleSystem in _particleSystems)
        {
            Debug.Log("Destroy " + particleSystem.gameObject.name);
            Destroy(particleSystem.gameObject);
        }

        _minesToReset.Clear();
        _particleSystems.Clear();
    }
}
