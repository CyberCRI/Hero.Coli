using UnityEngine;
using System.Collections;
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
    
  private GameState _gameState;
  public GUITransitioner gUITransitioner;
  public Fade fadeSprite;
  public UIPanel  introPanel, endPanel;
  public bool dePauseForbidden;

	void Awake() {
    _instance = this;
		Application.LoadLevelAdditive("Interface1.0");
		Application.LoadLevelAdditive("Bacterium1.0");
		Application.LoadLevelAdditive("World1.0");
	}
	// Use this for initialization
	void Start () {


		 _gameState = GameState.Start;
		 dePauseForbidden = true;
	   //dePauseForbidden = false;
		 //StateChange(GameState.Game);
	}
	
	// Update is called once per frame
	void Update () {
		switch(_gameState){
		
			case GameState.Start:
        fadeSprite.gameObject.SetActive(true);
			  introPanel.gameObject.SetActive(true);
				gUITransitioner.Pause(true);
        break;
			
			case GameState.Game:
				if (Input.GetKeyDown(KeyCode.Escape)) changeState(GameState.Pause);
			  break;
			
			case GameState.Pause:
				if (dePauseForbidden == false){
					if (Input.GetKeyDown(KeyCode.Escape)) changeState(GameState.Game);
				}
			  break;
			
			case GameState.End:
				gUITransitioner.TerminateGraphs();
				fadeSprite.gameObject.SetActive(true);
				fadeSprite.FadeIn();
				gUITransitioner.Pause(true);
				dePauseForbidden = true;
				endPanel.gameObject.SetActive(true);
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
			//gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
				gUITransitioner.Pause(false);
				dePauseForbidden = false;
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
