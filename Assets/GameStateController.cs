using UnityEngine;
using System.Collections;
public enum GameState{
	Start,
	Game,
	Pause,
	End
}

public class GameStateController : MonoBehaviour {

private GameState _gameState;
public GUITransitioner gUITransitioner;
public Fade fadeSprite;
	
	// Use this for initialization
	void Start () {
		 _gameState = GameState.Start;
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(_gameState){
		
			case GameState.Start:
				Debug.Log("start");
				fadeSprite.FadeOut();
				StateChange(GameState.Game);
			break;
			
			case GameState.Game:
				if (Input.GetKeyDown(KeyCode.Escape)) StateChange(GameState.Pause);
			break;
			
			case GameState.Pause:
				if (Input.GetKeyDown(KeyCode.Escape)) StateChange(GameState.Game);
			break;
			
			case GameState.End:
				fadeSprite.FadeIn();
			break;
		
		}
	}
	
public void StateChange(GameState newState){
		_gameState = newState;
		
		switch(_gameState){
			case GameState.Start:
			break;
			
			case GameState.Game:
				Debug.Log("game");
				gUITransitioner.Pause(false);
			break;
			
			case GameState.Pause:
				Debug.Log("pause");
				gUITransitioner.Pause(true);
			break;
			
			case GameState.End:
				Debug.Log("end");
				gUITransitioner.Pause(true);
			break;
			
		}
	}
}
