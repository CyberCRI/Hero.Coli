using UnityEngine;
using System.Collections;
using System.Xml;

public class MineManager : MonoBehaviour {
    
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "MineManager";
    private static MineManager _instance;
    public static MineManager get() {
        if(_instance == null) {
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
	public GameObject mine;

	public static bool isReseting;
	public static int detonatedMines;

	public Hero hero;

	// Use this for initialization
	void Start () {
		isReseting = false;
	}

  private Hero safeGetPerso()
  {
      if(null == hero)
      {
        Logger.Log("MineManager::safeGetPerso null == hero");
        hero = GameObject.Find("Perso").GetComponent<Hero>();
      }
      return hero;
  }

    public static void detonate(Mine mine , bool reseting)
    {
        detonatedMines++;
        resetSelectedMine(mine, reseting);
    }

	//public void resetSelectedMine(float id, GameObject target)
  public static void resetSelectedMine(Mine mine, bool reseting)
	{
    GameObject target = mine.gameObject;
		float x = target.transform.position.x; 
		float z = target.transform.position.z;
	
		iTween.Stop(target, true);
		Destroy(target);
        if (reseting == true)
        {
            GameObject go = (GameObject)Instantiate(_instance.mine, new Vector3(x, 0, z), Quaternion.identity);
            Mine newMine = (Mine)go.GetComponent<Mine>();
            newMine.mineName = mine.mineName;
        }

		detonatedMines--;

    if (isReseting && detonatedMines == 0)
    {
      isReseting = false;
    }
  }
}
