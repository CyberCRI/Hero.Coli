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
            Debug.LogWarning("WorldLinkManager get was badly initialized");
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
        if ((_instance != null) && (_instance != this))
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
        if (!_initialized)
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
    [SerializeField]
    private Teleporter teleporter;
    [SerializeField]
    private EndGameCollider endGameCollider;
    [SerializeField]
    private InfoWindowCollisionTrigger rfpTutorialTrigger;
    [SerializeField]
    private GameObject _assetsLibrary;

    public override void initialize()
    {
        base.initialize();

        activateAllInArray(false);

        if (null != _assetsLibrary)
        {
            _assetsLibrary.SetActive(false);
        }
    }

    public override void finishInitialize()
    {

        base.finishInitialize();

        GameObject character = Character.get().gameObject;
        if (null == character)
        {
            Debug.LogError(this.GetType() + " Character not found!");
        }

        // activate all map
        teleporter.activateAll(true);
        NanobotsCounter.initialize();
        teleporter.initialize();
        // deactivate all map
        teleporter.activateAll(false);

        // TODO remove StartZoneSetter
        // GameObject go = GameObject.Find("StartZoneSetter");
        // if (null != go)
        // {
        //     go.GetComponent<SwitchZoneOnOff>().triggerSwitchZone();
        // }

        teleporter.teleport(MemoryManager.get().checkpointIndex);
        // We reset the checkpointindex at its default value since we no longer need it after a teleportation
        MemoryManager.get().checkpointIndex = 0;

        /*
		if (null != startPosition)
		{
			CellControl.get(gameObjectName).teleport(startPosition.transform.position, startPosition.transform.rotation);
			startPosition.gameObject.SetActive(false);
		}
		*/

        //specific code for adventure1
        if (null != rfpTutorialTrigger && null != character)
        {
            rfpTutorialTrigger.characterCollider = character.GetComponent<CapsuleCollider>();
        }

        GameStateController.get().teleporter = teleporter;

		BoundCamera.instance.Reset ();
    }
}
