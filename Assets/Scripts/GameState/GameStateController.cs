using UnityEngine;
using System.Collections;
using System;

public enum GameState{
	Start,
	Game,
	Pause,
	End
}

public class GameStateController : MonoBehaviour {

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "GameStateController";
	private static GameStateController _instance;
	public static GameStateController get() {
		if (_instance == null)
		{
			Logger.Log("GameStateController::get was badly initialized", Logger.Level.WARN);
			_instance = GameObject.Find(gameObjectName).GetComponent<GameStateController>();
		}

		return _instance;
	}
  ////////////////////////////////////////////////////////////////////////////////////////////

  public static string keyPrefix = "KEY.";
  private static string _inventoryKey = keyPrefix+"INVENTORY";
  private static string _craftingKey = keyPrefix+"CRAFTING";
  private static string _pauseKey = keyPrefix+"PAUSE";


  private GameState _gameState;
  public GUITransitioner gUITransitioner;
  public Fade fadeSprite;
  public GameObject intro, end, pauseIndicator;
  private int _pausesStacked = 0;
  public int getPausesInStackCount(){
    return _pausesStacked;
  }
  public int pushPauseInStack()
  {
    _pausesStacked++;
        Logger.Log("pushPauseInStack() returns "+_pausesStacked, Logger.Level.INFO);
    return _pausesStacked;
  }
  public int popPauseInStack()
  {
        Logger.Log("popPauseInStack() starts with _pausesStacked=="+_pausesStacked, Logger.Level.DEBUG);
    if(_pausesStacked > 0)
    {
      _pausesStacked--;
    }
    else
    {
      Logger.Log("GameStateController::popPauseInStack tried to pop a pause from empty stack", Logger.Level.WARN);
      _pausesStacked = 0;
    }
        Logger.Log("popPauseInStack() returns _pausesStacked=="+_pausesStacked, Logger.Level.INFO);
    return _pausesStacked;
  }
  public void tryUnlockPause()
  {
        Logger.Log("tryUnlockPause() with previous pausesStacked="+_pausesStacked, Logger.Level.DEBUG);
    if(0 == popPauseInStack())
    {
      changeState(GameState.Game);
    }
        Logger.Log("tryUnlockPause() with final pausesStacked="+_pausesStacked, Logger.Level.INFO);
  }
  public void tryLockPause()
  {
        Logger.Log("tryLockPause() with previous pausesStacked="+_pausesStacked, Logger.Level.DEBUG);
    pushPauseInStack();
    changeState(GameState.Pause);
        Logger.Log("tryLockPause() with final pausesStacked="+_pausesStacked, Logger.Level.INFO);
  }


	void Awake() {
    _instance = this;
		Application.LoadLevelAdditive("Interface1.0");
		Application.LoadLevelAdditive("Bacterium1.0");
		Application.LoadLevelAdditive("World1.0");
		//Application.LoadLevelAdditive("test-Challenge3");
	}
	// Use this for initialization
	void Start () {
		_gameState = GameState.Start;
		pushPauseInStack();
    
    I18n.changeLanguageTo(I18n.Language.French);
    Logger.Log("GameStateController::Start game starts in "+Localization.Localize("MAIN.LANGUAGE"), Logger.Level.INFO);
	}

    //TODO optimize for frequent calls & refactor out of GameStateController
    public static KeyCode getKeyCode(string localizationKey)
    {
      return (KeyCode) Enum.Parse(typeof(KeyCode), Localization.Localize(localizationKey));
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKeyDown(string localizationKey)
    {
      return Input.GetKeyDown(getKeyCode(localizationKey));
    }

    //TODO optimize for frequent calls
    public static bool isShortcutKey(string localizationKey)
    {
      return Input.GetKey(getKeyCode(localizationKey));
    }
	
	// Update is called once per frame
	void Update () {
		switch(_gameState){
		
			case GameState.Start:
        fadeSprite.gameObject.SetActive(true);
			  intro.SetActive(true);
        end.SetActive(false);
        changeState(GameState.Pause);
        break;
			
			case GameState.Game:
                //pause
        if (Input.GetKeyDown(KeyCode.Escape) || isShortcutKeyDown(_pauseKey))
        {
          ModalManager.setModal(pauseIndicator, false);
          changeState(GameState.Pause);
        } 
                //inventory
                //TODO add DNA damage accumulation management when player equips/unequips too often
        else if(isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
        {
          gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
        }
                //crafting
        else if(isShortcutKeyDown(_craftingKey) && CraftZoneManager.isOpenable())
        {
          gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
        }
			  break;
			
			case GameState.Pause:
        switch(gUITransitioner._currentScreen)
        {
          case GUITransitioner.GameScreen.screen1:
            if ((Input.GetKeyDown(KeyCode.Escape) || isShortcutKeyDown(_pauseKey)) && (0 == getPausesInStackCount()))
            {
              ModalManager.unsetModal();
              changeState(GameState.Game);
            }
            break;
          case GUITransitioner.GameScreen.screen2:
            if(isShortcutKeyDown(_inventoryKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
              gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
            }
            else if(isShortcutKeyDown(_craftingKey) && CraftZoneManager.isOpenable())
            {
              gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen3);
            }
            break;
          case GUITransitioner.GameScreen.screen3:
            if(isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
            {
              gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
            }
            else if(isShortcutKeyDown(_craftingKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
              gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
            }
            break;
          default:
            Logger.Log("GameStateController::Update unknown screen "+gUITransitioner._currentScreen, Logger.Level.WARN);
            break;
        }
			  break;
			
			case GameState.End:
				gUITransitioner.TerminateGraphs();
				fadeSprite.gameObject.SetActive(true);
				fadeSprite.FadeIn();
				gUITransitioner.Pause(true);
				pushPauseInStack();
				end.SetActive(true);
        break;	

       default:
        break;
		}
	}
	
	public void changeState(GameState newState){

		_gameState = newState;
    Logger.Log("GameStateController::StateChange _gameState="+_gameState, Logger.Level.INFO);
		
		switch(_gameState){
			case GameState.Start:
			  break;
			
			case GameState.Game:
				gUITransitioner.Pause(false);
			  break;
			
			case GameState.Pause:
				gUITransitioner.Pause(true);
			  break;
			
			case GameState.End:
				gUITransitioner.Pause(true);
			  break;
			
		}
	}

  public static void restart()
  {
    Application.LoadLevel("Master-Demo-0.1");
  }
	
}
