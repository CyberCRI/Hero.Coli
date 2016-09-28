using UnityEngine;


/*DESCRIPTION
 * This class create the links between the World's Scene, classes and GameObject and the others
 * */

public class WorldLinkManager : LinkManager
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "WorldLinkManager";
    private static WorldLinkManager _instance;

    public static WorldLinkManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("WorldLinkManager::get was badly initialized");
            GameObject go = GameObject.Find(gameObjectName);
            if (go)
            {
                _instance = go.GetComponent<WorldLinkManager>();
            }
            else
            {
                Debug.LogError(gameObjectName + " not found");
            }
        }
        return _instance;
    }
    
    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
        {            
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;  
    private void initializeIfNecessary()
    {
        if(!_initialized)
        {
            _initialized = true;
        }
    }

    new void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        base.Start();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    protected override int getLMIndex()
    {
        return GameStateController.wlmIndex;
    }

    public MineManager mineManager;
    public Transform startPosition;

    public override void initialize()
    {
        base.initialize ();

        GameObject perso = Hero.get().gameObject;
        if (null == perso)
        {
            Logger.Log("WorldLinkManager: error: Hero not found!", Logger.Level.ERROR);
        }

        if (null != startPosition)
        {
            CellControl.get(gameObjectName).teleport(startPosition.transform.position, startPosition.transform.rotation);
            startPosition.gameObject.SetActive(false);
        }

        //specific code for adventure1
        GameObject endGameColliderGameObject = GameObject.Find("TutoEnd");
        if (null != endGameColliderGameObject)
        {
            EndGameCollider endGameCollider = endGameColliderGameObject.GetComponent<EndGameCollider>();
            endGameCollider.hero = perso;
            endGameCollider.endInfoPanel = GameStateController.get().endWindow;
            endGameCollider.endMainMenuButton = GameStateController.get().endMainMenuButton;
            Logger.Log("EndGameCollider.infoPanel" + endGameCollider.endInfoPanel, Logger.Level.INFO);
        }

        //specific code for adventure1
        GameObject tutoRFPGameObject = GameObject.Find("TutoRFP");
        if (null != tutoRFPGameObject && null != perso)
        {
            InfoWindowCollisionTrigger tutoRFP = tutoRFPGameObject.GetComponent<InfoWindowCollisionTrigger>();
            tutoRFP.heroCollider = perso.GetComponent<CapsuleCollider>();
        }

        //specific code for tutorial1
        endGameColliderGameObject = GameObject.Find("Tutorial1End");
        if (null != endGameColliderGameObject)
        {
            EndGameCollider endGameCollider = endGameColliderGameObject.GetComponent<EndGameCollider>();
            endGameCollider.hero = perso;
            endGameCollider.endInfoPanel = GameStateController.get().endWindow;
            endGameCollider.endMainMenuButton = GameStateController.get().endMainMenuButton;
            Logger.Log("EndGameCollider.infoPanel" + endGameCollider.endInfoPanel, Logger.Level.INFO);
        }
    }

    public override void finishInitialize()
    {
        base.finishInitialize ();
    }
}
