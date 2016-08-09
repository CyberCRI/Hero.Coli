using UnityEngine;

public class GUITransitioner : MonoBehaviour {


  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "GUITransitioner";
  private static GUITransitioner _instance;
  public static GUITransitioner get() {
    if(_instance == null) {
      Logger.Log("GUITransitioner::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<GUITransitioner>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("GUITransitioner::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////
	
	private ReactionEngine _reactionEngine;
  public InventoryAnimator animator;
	
	private float _timeDelta = 0.2f;
	
    private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float _deltaTime = 0f;
	
	private bool _pauseTransition = false;
	private float _timeScale = 1.0f;
	
	//regulates at which speed the game will get back to its original speed when pause is switched off
	private float _unpauseAcceleration = 0.25f;
	
	public GameScreen _currentScreen = GameScreen.screen1;
	public enum GameScreen {
		screen1,
		screen2,
		screen3
	};
	
	
	//public GameObject _mainCameraObject;
  public BoundCamera mainBoundCamera;
	
	public GameObject worldScreen;
	public GameObject craftScreen;
	private DevicesDisplayer _devicesDisplayer;


	public VectrosityPanel celliaGraph;
	public VectrosityPanel roomGraph;

  public Hero hero;
  public CellControl control;

	public ArrowAnimationManager arrowManager;		//Manager for the arrow Animation
	
	// Use this for initialization
	void Start () {

    _reactionEngine = ReactionEngine.get();
    _devicesDisplayer = DevicesDisplayer.get();

		SetScreen2(false);
		SetScreen3(false);
		SetScreen1(true);
		
		
		_devicesDisplayer.UpdateScreen();
		_currentScreen = GameScreen.screen1;
		
		_timeAtLastFrame = Time.realtimeSinceStartup;

		arrowManager = new ArrowAnimationManager();
	}
	
	
	private void SetScreen1(bool active) {
		if(active) ZoomOut();
		worldScreen.SetActive(active);
		//_craftScreen.SetActive(!active);
	}
	
	private void SetScreen2(bool active) {
		if(active) ZoomIn();
		worldScreen.SetActive(active);
	}
	
	private void SetScreen3(bool active) {
		craftScreen.SetActive(active);
	}
	
  /*
   * "Defensive programming" method
   * Cannot work during Awake
	private void checkCamera() {
		if(_mainBoundCamera == null) {
			_mainBoundCamera = GameObject.Find ("Main Camera").GetComponent<BoundCamera>() as BoundCamera;
		}
	}
 */ 
	
	private void ZoomIn() {
		//checkCamera();
		mainBoundCamera.SetZoom(true);
		//Logger.Log("main camera"+_mainBoundCamera, Logger.Level.WARN);
	}

	private void ZoomOut() {
		//checkCamera();
		mainBoundCamera.SetZoom(false);
	}
		
  public void showGraphs(bool show) {
  	roomGraph.gameObject.SetActive(show);
    celliaGraph.gameObject.SetActive(show);
  }

    public void Pause (bool pause)
    {
        _pauseTransition = !pause;
        if (_reactionEngine == null) {
            _reactionEngine = ReactionEngine.get ();
        }
        _reactionEngine.Pause (pause);
        _timeScale = pause ? 0 : 1;
        if (pause) {
            Time.timeScale = 0;
        }

        roomGraph.setPause (pause);
        celliaGraph.setPause (pause);  
        hero.Pause (pause);
        control.Pause (pause);
        EnemiesManager.Paused = pause;
    }

  public void GoToScreen(GameScreen destination) {
    Logger.Log("GUITransitioner::GoToScreen("+destination+")");
    if(destination == GameScreen.screen1) {
      if(_currentScreen == GameScreen.screen2) {
         Logger.Log("2->1", Logger.Level.INFO);
         //2 -> 1
         //set zoom1
         //remove inventory device, deviceID
         //add graphs
         //move devices and potions?
         
       } else if(_currentScreen == GameScreen.screen3) {
         Logger.Log("3->1");
         //3 -> 1
         //set zoom1
         //remove craft screen
         //add graphs
         //add potions
         //add devices
         //add life/energy
         //add medium info
         SetScreen3(false);
         SetScreen1(true);
       }
       GameStateController.get().tryUnlockPause();
       ZoomOut();
       _currentScreen = GameScreen.screen1;


    } else if (destination == GameScreen.screen2) {
			if(animator.isPlaying == true)
			{
				animator.reset();
			}
      if(_currentScreen == GameScreen.screen1) {
         Logger.Log("GUITransitioner::GoToScreen 1->2", Logger.Level.INFO);
         //1 -> 2
         //set zoom2
         //add inventory device, deviceID
         //remove graphs
         //move devices and potions?
         GameStateController.get().tryLockPause();
       } else if(_currentScreen == GameScreen.screen3) {
        Logger.Log("GUITransitioner::GoToScreen 3->2", Logger.Level.INFO);
         //3 -> 2
         //set zoom2
         //remove craft screen
         //add inventory device, deviceID
         //add potions
         //add devices
         //add life/energy
         //add medium info
         SetScreen3(false);
         SetScreen2(true);
       }
       
       ZoomIn();
       _currentScreen = GameScreen.screen2;      


    } else if (destination == GameScreen.screen3) {
      if(_currentScreen == GameScreen.screen1) {
        Logger.Log("GUITransitioner::GoToScreen 1->3", Logger.Level.INFO);
         //1 -> 3
         //remove everything
         //add device inventory, parameters
         //remove graphs
         //move devices and potions?
         SetScreen1(false);
         SetScreen3(true);
         GameStateController.get().tryLockPause();
         
       } else if(_currentScreen == GameScreen.screen2) {
        Logger.Log("GUITransitioner::GoToScreen 2->3", Logger.Level.INFO);
         //2 -> 3
         //remove everything
         //add craft screen
         SetScreen2(false);
         SetScreen3(true);
         
       } 
       ZoomIn();         
       _currentScreen = GameScreen.screen3;

    } else {
      Logger.Log("GuiTransitioner::GoToScreen("+destination+"): error: unmanaged destination", Logger.Level.ERROR);
    }

    _devicesDisplayer.UpdateScreen();
    TooltipManager.displayTooltip();
  }

  public void SwitchScreen(GameScreen alternate1, GameScreen alternate2) {
      Logger.Log ("GuiTransitioner::SwitchScreen("+alternate1+","+alternate2+")");
    if (_currentScreen == alternate1) {
      GoToScreen(alternate2);
    } else if (_currentScreen == alternate2) {
      GoToScreen(alternate1);
    } else {
      Logger.Log ("GuiTransitioner::SwitchScreen("+alternate1+","+alternate2+"): error: unmanaged alternate", Logger.Level.WARN);
    }
  }
	
	// Update is called once per frame
	void Update () {
		
		_timeAtCurrentFrame = Time.realtimeSinceStartup;
        _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
		
		if(_deltaTime > _timeDelta) {
			if (Input.GetKey(KeyCode.Alpha1)) {
        GoToScreen(GameScreen.screen1);
			} else if (Input.GetKey(KeyCode.Alpha2)) {
        GoToScreen(GameScreen.screen2);
			} else if (Input.GetKey(KeyCode.Alpha3)) {
        GoToScreen(GameScreen.screen3);
			}
      _timeAtLastFrame = _timeAtCurrentFrame;
		}

		arrowManager.launchAnimation ();
	}
	
	void LateUpdate () {
	  if(_pauseTransition) {
		if ((Time.timeScale > 0.99f)) {
		  Time.timeScale = 1;
		  _pauseTransition = false;
		} else {
	      Logger.Log ("GUITransitioner::LateUpdate Time.timeScale="+Time.timeScale
			+", _timeScale="+_timeScale+", _timeDelta="+_timeDelta, Logger.Level.TRACE);
	      Time.timeScale = Mathf.Lerp(Time.timeScale, _timeScale, _deltaTime*_unpauseAcceleration);
		}
	  }
	}
}
