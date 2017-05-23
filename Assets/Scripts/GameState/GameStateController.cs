using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
	Start,
	MainMenu,
	Game,
	Pause,
	End,
	Default
}

//specifies which game state is to be set after a computation
//Start, MainMenu, Game, Pause, End: GameState states as targets
//NoTarget: no specific state is required
//NoAction: no computation happened and therefore no state change shoud happen
public enum GameStateTarget
{
	Start,
	MainMenu,
	Game,
	Pause,
	End,
	NoTarget,
	NoAction
}

public class GameStateController : MonoBehaviour
{

	//////////////////////////////// singleton fields & methods ////////////////////////////////
	private const string gameObjectName = "GameStateController";
	private static GameStateController _instance;

	public static GameStateController get ()
	{
		if (_instance == null) {
			Debug.LogWarning ("GameStateController get was badly initialized");
			_instance = GameObject.Find (gameObjectName).GetComponent<GameStateController> ();
		}

		return _instance;
	}

	void Awake ()
	{
		// Debug.Log(this.GetType() + " Awake");
		if ((_instance != null) && (_instance != this)) {
			Debug.LogError (this.GetType () + " has two running instances");
		} else {
			_instance = this;
			initializeIfNecessary ();
		}
	}

	void OnDestroy ()
	{
		// Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
		_instance = (_instance == this) ? null : _instance;
	}

	private bool _initialized = false;

	private void initializeIfNecessary ()
	{
		if (!_initialized) {
			_initialized = true;
			_gameState = GameState.Start;
			resetPauseStack ();
			reinitializeLoadingVariables ();
			// SceneManager.sceneLoaded += SceneLoaded;
		}
	}

	void Start ()
	{
		// Debug.Log(this.GetType() + " Start");
		_scorekeeper = new Scorekeeper ();
		I18n.register (_scorekeeper);
		loadLevels ();
	}

	////////////////////////////////////////////////////////////////////////////////////////////

	public const string _masterScene = "Master-Demo-0.1";
	public const string _interfaceScene = "Interface1.0";
	public const string _bacteriumScene = "Bacterium1.0";

	private Scorekeeper _scorekeeper;

	public const string keyPrefix = "KEY.";
	// public const string _inventoryKey = keyPrefix + "INVENTORY";
	public const string _craftingKey = keyPrefix + "CRAFTING";
	public const string _pauseKey = keyPrefix + "PAUSE";
	public const string _sandboxKey = keyPrefix + "SANDBOX";
	public const string _forgetDevicesKey = keyPrefix + "FORGETDEVICES";

	private GameState _gameState;
	public GUITransitioner gUITransitioner;

	private Fade _fadeSprite;
	private GameObject _intro, _endWindow, _finalScoreboard, _pauseIndicator;
	private UILabel _chaptersLabel, _ownTimesLabel, _ownRecordsLabel, _worldRecordsLabel;
	private ContinueButton _introContinueButton;
	private EndMainMenuButton _endMainMenuButton, _finalScoreboardQuitButton;
	private MainMenuManager _mainMenu;

	private static int _pausesStacked = 0;
	private bool _isGameLevelPrepared = false;
	private GameState _stateBeforeMainMenu = GameState.Game;

	// private bool _finishLoadLevels = false;
	private bool _finishedLoadingLevels = false;
	private bool[] _loadingFlags = new bool[3] { false, false, false };
	public const int ilmIndex = 0, plmIndex = 1, wlmIndex = 2;

	public Teleporter teleporter;

	// bool isInterfaceLoaded = false;
	// bool isPlayerLoaded = false;
	// bool isWorldLoaded = false;

	private void reinitializeLoadingVariables ()
	{
		// isInterfaceLoaded = false;
		// isPlayerLoaded = false;
		// isWorldLoaded = false;

		_loadingFlags = new bool[3] { false, false, false };
		_finishedLoadingLevels = false;
		_isGameLevelPrepared = false;
	}

	// void SceneLoaded(Scene scene, LoadSceneMode m)
	// {
	// Debug.Log(this.GetType() + " SceneLoaded(" + scene + ", " + m + ")");

	//     int level = scene.buildIndex;

	//     isInterfaceLoaded = isInterfaceLoaded || (3 == level);
	//     isPlayerLoaded = isPlayerLoaded || (2 == level);
	//     isWorldLoaded = isWorldLoaded || (1 == level) || (4 == level) || (5 == level);

	//     if (isInterfaceLoaded && isPlayerLoaded && isWorldLoaded)
	//     {
	//         Debug.Log(this.GetType() + " all scenes loaded => _finishLoadLevels = true");
	//         // finishLoadLevels();
	//         _finishLoadLevels = true;
	//     }
	// }


	public void setSceneLoaded (int index)
	{
		if (index < _loadingFlags.Length && index >= 0) {
			_loadingFlags [index] = true;

			bool allLoaded = true;
			foreach (bool flag in _loadingFlags) {
				allLoaded &= flag;
			}
			if (allLoaded) {
				// Debug.Log(this.GetType() + " all scenes loaded => _finishLoadLevels = true");
				finishLoadLevels ();
			}
		} else {
			Debug.LogError (this.GetType () + " incorrect index " + index);
		}
	}

	private void loadLevels ()
	{
		// Debug.Log(this.GetType() + " loadLevels");
		SceneManager.LoadScene (_interfaceScene, LoadSceneMode.Additive);
		SceneManager.LoadScene (_bacteriumScene, LoadSceneMode.Additive);
		SceneManager.LoadScene (MemoryManager.get ("loadLevels").configuration.getSceneName (), LoadSceneMode.Additive);
	}

	private void finishLoadLevels ()
	{
		// Debug.Log(this.GetType() + " finishLoadLevels");

		// get the linkers
		InterfaceLinkManager ilm = InterfaceLinkManager.get ();
		PlayerLinkManager blm = PlayerLinkManager.get ();
		WorldLinkManager wlm = WorldLinkManager.get ();

		// Debug.Log(this.GetType() + " initialization: ilm=" + ilm + ", blm=" + blm + ", wlm=" + wlm);
		ilm.initialize ();
		blm.initialize ();
		wlm.initialize ();

		// Debug.Log(this.GetType() + " finishInitialize");
		ilm.finishInitialize ();
		blm.finishInitialize ();
		wlm.finishInitialize ();

		_finishedLoadingLevels = true;

		MainMenuManager.get ().open ();

		// Debug.Log(this.GetType() + " finishLoadLevels done");
	}

	public static bool isPause ()
	{
		return (GameState.Pause == _instance._gameState);
	}

	private static void resetPauseStack ()
	{
		_pausesStacked = 0;
	}

	public static int getPausesInStackCount ()
	{
		return _pausesStacked;
	}

	private int pushPauseInStack ()
	{
		_pausesStacked++;
		// Debug.Log(this.GetType() + " pushPauseInStack() returns " + _pausesStacked);
		return _pausesStacked;
	}

	public int popPauseInStack ()
	{
		// Debug.Log(this.GetType() + " popPauseInStack() starts with _pausesStacked==" + _pausesStacked);
		if (_pausesStacked > 0) {
			_pausesStacked--;
		} else {
			Debug.LogWarning (this.GetType () + " popPauseInStack tried to pop a pause from empty stack");
			_pausesStacked = 0;
		}
		// Debug.Log(this.GetType() + " popPauseInStack() returns _pausesStacked==" + _pausesStacked);
		return _pausesStacked;
	}

	public void tryUnlockPause ()
	{
		// Debug.Log(this.GetType() + " tryUnlockPause() with previous _pausesStacked=" + _pausesStacked);
		if (0 == popPauseInStack ()) {
			changeState (GameState.Game);
		}
		// Debug.Log(this.GetType() + " tryUnlockPause() with final _pausesStacked=" + _pausesStacked);
	}

	public void tryLockPause ()
	{
		// Debug.Log(this.GetType() + " tryLockPause() with previous _pausesStacked=" + _pausesStacked);
		pushPauseInStack ();
		changeState (GameState.Pause);
		// Debug.Log(this.GetType() + " tryLockPause() with final _pausesStacked=" + _pausesStacked);
	}

	//TODO optimize for frequent calls & refactor out of GameStateController
	public static KeyCode getKeyCode (string localizationKey)
	{
		return (KeyCode)Enum.Parse (typeof(KeyCode), Localization.Localize (localizationKey));
	}

	//TODO optimize for frequent calls
	public static bool isShortcutKeyDown (string localizationKey, bool restricted = false)
	{
		return (!restricted || GameConfiguration.isAdmin) && Input.GetKeyDown (getKeyCode (localizationKey));
	}

	//TODO optimize for frequent calls
	public static bool isShortcutKey (string localizationKey, bool restricted = false)
	{
		return (!restricted || GameConfiguration.isAdmin) && Input.GetKey (getKeyCode (localizationKey));
	}

	private static GameState getStateFromTarget (GameStateTarget target)
	{
		GameState result = GameState.Game;

		switch (target) {
		case GameStateTarget.Start:
			result = GameState.Start;
			break;
		case GameStateTarget.MainMenu:
			result = GameState.MainMenu;
			break;
		case GameStateTarget.Game:
			result = GameState.Game;
			break;
		case GameStateTarget.Pause:
			result = GameState.Pause;
			break;
		case GameStateTarget.End:
			result = GameState.End;
			break;
		case GameStateTarget.NoTarget:
			result = GameState.Default;
			break;
		case GameStateTarget.NoAction:
			result = GameState.Default;
			break;
		default:
			Debug.LogWarning ("GameStateController getStateFromTarget unknown target state " + target);
			result = GameState.Default;
			break;
		}

		return result;
	}

	public void endGame ()
	{
		// Debug.Log(this.GetType() + " endGame");
		reinitializeLoadingVariables ();
		_mainMenu.setNewGame ();
		goToMainMenuFrom (GameState.MainMenu);
	}

	private void prepareGameLevelIfNecessary ()
	{
		if (!_isGameLevelPrepared) {
			//TODO put this code into a separate implementation of a "Level" interface
			//with methods such as "OnStartState", "OnGameState(bool isPause)" and so on
			//so that those modals don't appear on every level
			//Also: put specific interface elements into level scene and then move them to interface hierarchy

			_mainMenu.setResume ();

			//TODO remove this temporary hack
			switch (MemoryManager.get ("prepareGameLevelIfNecessary").configuration.getMode ()) {
			case GameConfiguration.GameMode.ADVENTURE:
				_fadeSprite.gameObject.SetActive (true);
				ModalManager.setModal (_intro, true, _introContinueButton.gameObject, _introContinueButton.GetType ().Name);
				changeState (GameState.Pause);
				break;
			case GameConfiguration.GameMode.SANDBOX:
				_fadeSprite.gameObject.SetActive (false);
				break;
			default:
				Debug.LogWarning (this.GetType () + " Update unknown game mode=" + MemoryManager.get ("prepareGameLevelIfNecessary 2").configuration.getMode ());
				break;
			}
			_isGameLevelPrepared = true;
		}
	}

	public void goToMainMenu ()
	{
		// Debug.Log(this.GetType() + " goToMainMenu");
		goToMainMenuFrom (_gameState);
	}

	public void goToMainMenuFrom (GameState state)
	{
		_stateBeforeMainMenu = state;
		// Debug.Log(this.GetType() + " goToMainMenuFrom(" + state + ") with mainMenu=" + mainMenu);
		_mainMenu.open ();
		changeState (GameState.MainMenu);
	}

	public void leaveMainMenu ()
	{
		// Debug.Log(this.GetType() + " leaveMainMenu");

		//restart
		if (GameState.MainMenu == _stateBeforeMainMenu) {
			GameStateController.restart ();
			//resume or new game
		} else {
			_mainMenu.close ();
			changeState (_stateBeforeMainMenu);
		}
	}

	private class CheckpointShortcut
	{
		public int index;
		public KeyCode keyCode;

		public CheckpointShortcut (int idx, KeyCode kc)
		{
			this.index = idx;
			this.keyCode = kc;
		}
	}

	private CheckpointShortcut[] _shortcuts = new CheckpointShortcut[15] {
		new CheckpointShortcut (0, KeyCode.Alpha0),
		new CheckpointShortcut (1, KeyCode.Alpha1),
		new CheckpointShortcut (2, KeyCode.Alpha2),
		new CheckpointShortcut (3, KeyCode.Alpha3),
		new CheckpointShortcut (4, KeyCode.Alpha4),
		new CheckpointShortcut (5, KeyCode.Alpha5),
		new CheckpointShortcut (6, KeyCode.Alpha6),
		new CheckpointShortcut (7, KeyCode.Alpha7),
		new CheckpointShortcut (8, KeyCode.Alpha8),
		new CheckpointShortcut (9, KeyCode.Alpha9),
		new CheckpointShortcut (10, KeyCode.Keypad0),
		new CheckpointShortcut (11, KeyCode.Keypad1),
		new CheckpointShortcut (12, KeyCode.Keypad2),
		new CheckpointShortcut (13, KeyCode.Keypad3),
		new CheckpointShortcut (14, KeyCode.Keypad4)
	};

	public void goToCheckpoint (int index)
	{
		if (null != teleporter) {
			teleporter.teleport (index);
		}
	}

	// Update is called once per frame
	void Update ()
	{

		// if (_finishLoadLevels)
		// {
		// Debug.Log(this.GetType() + " _finishLoadLevels = true => finishLoadLevels()");
		//     _finishLoadLevels = false;
		//     finishLoadLevels();
		// }

		if (_finishedLoadingLevels) {

			// TODO replace by per-checkpoint teleportation
			if (GameConfiguration.isAdmin) {
				foreach (CheckpointShortcut sc in _shortcuts) {
					if (Input.GetKeyDown (sc.keyCode)) {
						// Debug.Log(this.GetType() + " pressed shortcut to teleport the character to checkpoint " + sc.index);
						goToCheckpoint (sc.index);
					}
				}
			}

			switch (_gameState) {

			case GameState.Start:

				_endWindow.SetActive (false);
				_finalScoreboard.SetActive (false);

				_mainMenu.setNewGame ();
				if (GameConfiguration.RestartBehavior.GAME == MemoryManager.get ("Update").configuration.restartBehavior) {
					leaveMainMenu ();
				} else {
					goToMainMenuFrom (GameState.Game);
				}
				break;

			case GameState.MainMenu:
				if ((Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0)
				     || (Input.GetButtonDown ("Horizontal") && Input.GetAxisRaw ("Horizontal") > 0)) {
					_mainMenu.selectPrevious ();
				} else if ((Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") < 0)
				          || (Input.GetButtonDown ("Horizontal") && Input.GetAxisRaw ("Horizontal") < 0)) {
					_mainMenu.selectNext ();
				} else if (Input.GetButtonUp ("Submit") || Input.GetKeyUp (KeyCode.Return) || Input.GetKeyUp (KeyCode.KeypadEnter)) {
					// Debug.Log(this.GetType() + " MainMenu Return/KeypadEnter");
					_mainMenu.getCurrentItem ().click ();
				} else if (Input.GetButtonDown ("Cancel")) {
					_mainMenu.escape ();
				}
				break;

			case GameState.Game:

				prepareGameLevelIfNecessary ();

                //pause
				if (isShortcutKeyDown (_pauseKey) || Input.GetButtonDown("Pause")) {
				// Debug.Log(this.GetType() + " Update - Escape/Pause key pressed");
					ModalManager.setModal (_pauseIndicator, false);
					changeState (GameState.Pause);
				}
                //main menu
                else if (Input.GetButtonDown ("Cancel")) {
					goToMainMenuFrom (GameState.Game);
				}
				//suicide
				else if (Input.GetButtonDown ("Suicide")) {
					Character.get().kill(new CustomData(CustomDataTag.SOURCE, CustomDataValue.SUICIDEBUTTON.ToString()));
				}
                //crafting
				else if (((isShortcutKeyDown (_craftingKey) || Input.GetButtonDown ("Crafting")) && CraftZoneManager.isOpenable ()) && canPressCraftShortcut ()) {
					// Debug.Log(this.GetType() + " Update craft key pressed");
					gUITransitioner.GoToScreen (GUITransitioner.GameScreen.screen3);
				}
                    //TODO fix this feature                    
                    // else if(isShortcutKeyDown(_forgetDevicesKey))
                    // {
                    //     Inventory.get ().switchDeviceKnowledge();
                    // }
				break;

			case GameState.Pause:
				if (Input.GetButtonDown ("Cancel")) {
					goToMainMenuFrom (GameState.Pause);
				} else {
					GameStateTarget newState = ModalManager.manageKeyPresses ();
					if (GameStateTarget.NoAction != newState) {
						if (
							GameStateTarget.NoTarget != newState
							&& GameStateTarget.Pause != newState) {
							changeState (getStateFromTarget (newState));
						}
					} else {
						switch (gUITransitioner._currentScreen) {
						case GUITransitioner.GameScreen.screen1:
							break;
						case GUITransitioner.GameScreen.screen2:
                                    // if (isShortcutKeyDown(_inventoryKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
                                    // {
                                    //     Debug.Log(this.GetType() + " Update out of inventory key pressed");
                                    //     gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen1);
                                    // }
                                    // else
							if (((isShortcutKeyDown (_craftingKey) || Input.GetKeyDown ("Crafting")) && CraftZoneManager.isOpenable ()) && canPressCraftShortcut ()) {
								// Debug.Log(this.GetType() + " Update inventory to craft key pressed");
								gUITransitioner.GoToScreen (GUITransitioner.GameScreen.screen3);
							}
							break;
						case GUITransitioner.GameScreen.screen3:
                                    // if (isShortcutKeyDown(_inventoryKey) && Inventory.isOpenable())
                                    // {
                                    //     Debug.Log(this.GetType() + " Update craft to inventory key pressed");
                                    //     gUITransitioner.GoToScreen(GUITransitioner.GameScreen.screen2);
                                    // }
                                    // else 
							if (
								(isShortcutKeyDown (_craftingKey) || Input.GetButtonDown ("Crafting") || Input.GetKeyDown (KeyCode.Return) || Input.GetKeyUp (KeyCode.KeypadEnter)) && canPressCraftShortcut ()
								&& canPressCraftShortcut ()) {
								// Debug.Log(this.GetType() + " Update out of craft key pressed");
								gUITransitioner.GoToScreen (GUITransitioner.GameScreen.screen1);
							}
							break;
						default:
							Debug.LogWarning (this.GetType () + " Update unknown screen " + gUITransitioner._currentScreen);
							break;
						}
					}
				}
				break;

			case GameState.End:
				if (Input.GetButtonDown ("Cancel")) {
					goToMainMenuFrom (GameState.End);
				}
				break;

			default:
				break;
			}
		}
	}

	private bool canPressCraftShortcut ()
	{
		// can press shortcut when no cutscene, no modal, no infowindow, no step by step tutorial
		bool result = !CutScene.isPlaying () && !ModalManager.isDisplayed () && !StepByStepTutorial.isPlaying ();
		return result;
	}

	/// <summary>
	/// Starts the game directly at a checkpoint
	/// </summary>
	public void loadWithCheckpoint (int checkpointIndex, GameConfiguration.GameMap gameMap)
	{
		if (GameConfiguration.gameMap != gameMap)
			setAndSaveLevelName (gameMap, "loadWithCheckPoint");
		MemoryManager.get ().checkpointIndex = checkpointIndex;
		// The checkpoint index is checked by the WorldLinkManager at the start
		internalRestart ();
	}

	public void goToOtherGameMode ()
	{
		// Debug.Log(this.GetType() + " goToOtherGameMode");
		GameConfiguration.GameMap destination =
			(GameConfiguration.getMode (GameConfiguration.gameMap) == GameConfiguration.GameMode.ADVENTURE) ?
                GameConfiguration.GameMap.SANDBOX2 :
                GameConfiguration.GameMap.TUTORIAL1;

		setAndSaveLevelName (destination, "goToOtherGameMode");
		// RedMetricsManager.get().sendRichEvent(TrackingEvent.SWITCH, new CustomData(CustomDataTag.GAMELEVEL, destination.ToString()));
		internalRestart ();
	}

	public void triggerEnd ()
	{
		float endTime = Time.unscaledTime;
		GUITransitioner.showGraphs (false, GUITransitioner.GRAPH_HIDER.ENDGAME);
		MemoryManager.get ().sendCompletionEvent ();

		_fadeSprite.gameObject.SetActive (true);
		_fadeSprite.FadeIn (0.5f);

		StartCoroutine (waitFadeAndDisplayEndWindow (1.0f, endTime));
	}

	private IEnumerator waitFadeAndDisplayEndWindow (float waitTime, float endTime)
	{
		yield return new WaitForSeconds (waitTime);

		// simple variant
		// ModalManager.setModal(endWindow, true, endMainMenuButton.gameObject, endMainMenuButton.GetType().AssemblyQualifiedName);

		// scoreboard variant
		_scorekeeper.finish (endTime, new UILabel[] {
			_chaptersLabel,
			_ownTimesLabel,
			_ownRecordsLabel,
			_worldRecordsLabel
		});
		ModalManager.setModal (_finalScoreboard, true, _finalScoreboardQuitButton.gameObject, _finalScoreboardQuitButton.GetType ().AssemblyQualifiedName);
	}

	public void resetPlayerPrefsAndRestart ()
	{
		Debug.LogWarning (this.GetType () + " PlayerPrefs.DeleteAll");
		PlayerPrefs.DeleteAll ();
		restart ();
	}

	public void changeState (GameState newState)
	{
		_gameState = newState;
		// Debug.Log(this.GetType() + " StateChange _gameState=" + _gameState);

		switch (_gameState) {
		case GameState.Start:
			break;

		case GameState.MainMenu:
			gUITransitioner.Pause (true);
			break;

		case GameState.Game:
			gUITransitioner.Pause (false);
			break;

		case GameState.Pause:
			gUITransitioner.Pause (true);
			break;

		case GameState.End:
			gUITransitioner.Pause (true);
			break;

		default:
			Debug.LogWarning (this.GetType () + " changeState unexpected game state " + newState);
			break;
		}
	}

	public void setAndSaveLevelName (GameConfiguration.GameMap newMap, string cause = null)
	{
		if (!string.IsNullOrEmpty (cause)) {
			// Debug.Log(this.GetType() + " setAndSaveLevelName by " + cause);
		} else {
			// Debug.Log(this.GetType() + " setAndSaveLevelName");
		}

		switch (newMap) {
		// case GameConfiguration.GameMap.ADVENTURE1:
		// case GameConfiguration.GameMap.SANDBOX1:
		case GameConfiguration.GameMap.SANDBOX2:
		case GameConfiguration.GameMap.TUTORIAL1:
                //saving level name into MemoryManager
                //because GameStateController current instance will be destroyed during restart
                //whereas MemoryManager won't
			GameConfiguration.gameMap = newMap;
			break;
		default:
			Debug.LogWarning (this.GetType () + " setAndSaveLevelName unmanaged level=" + newMap);
			break;
		}
	}

	public GameState getState ()
	{
		return _gameState;
	}

	public static void restart ()
	{
		RedMetricsManager.get ().sendRichEvent (TrackingEvent.RESTART);
		internalRestart ();
	}

	private static void internalRestart ()
	{
		// Debug.Log(this.GetType() + " restart");
		//TODO reload scene but reset all of its components without using MemoryManager
		//note: ways to transfer data from one scene to another
		//get data from previous instance
		//other solution: make all fields static
		//other solution: fix initialization through LinkManagers
		//that automatically take new GameStateController object
		//and leave old GameStateController object with old dead links to destroyed objects

		_instance._initialized = false;
		MemoryManager.get ("internalRestart").configuration.restartBehavior = GameConfiguration.RestartBehavior.GAME;

		// manual reset
		iTween.Stop ();
		iTweenPath.paths.Clear ();
		CutScene.reset ();
		DisplayedDevice.reset ();
		PickablePlasmid.reset ();
		AvailableDisplayedBioBrick.reset ();
		NanobotsPickUpHandler.reset ();
		StepByStepTutorial.reset ();
		EnemiesManager.reset ();
		I18n.reset ();

		SceneManager.LoadScene (_masterScene);
	}

	public void FadeScreen (bool fade, float speed)
	{
		_fadeSprite.gameObject.SetActive (true);
		if (fade) {
			_fadeSprite.FadeIn (speed);
		} else {
			_fadeSprite.FadeOut (speed);
		}
	}

	public static void collideChapter (int index, float time)
	{
		// Debug.Log("GameStateController collideChapter(" + index + ", " + time + ")");
		_instance._scorekeeper.collideChapter (index, time);
	}

	public static void setChapter (int index, float time)
	{
		// Debug.Log("GameStateController startChapter(" + index + ", " + time + ")");
		_instance._scorekeeper.startChapter (index, time);
	}
    
	// dev method
	public static void unlockAll ()
	{
		Debug.Log ("GameStateController unlockAll");
		_instance._scorekeeper.unlockAll ();
	}

	public void setInterfaceElements (
		Fade fadeSprite,
		GameObject intro, GameObject endWindow, GameObject finalScoreboard, GameObject pauseIndicator,
		UILabel chaptersLabel, UILabel ownTimesLabel, UILabel ownRecordsLabel, UILabel worldRecordsLabel,
		ContinueButton introContinueButton,
		EndMainMenuButton endMainMenuButton, EndMainMenuButton finalScoreboardQuitButton,
		MainMenuManager mainMenu,
		MapChapterUnlocker unlocker)
	{
		// Debug.Log(this.GetType() + " setInterfaceElements");

		_fadeSprite = fadeSprite;
		_intro = intro;
		_endWindow = endWindow;
		_finalScoreboard = finalScoreboard;
		_pauseIndicator = pauseIndicator;
		_chaptersLabel = chaptersLabel;
		_ownTimesLabel = ownTimesLabel;
		_ownRecordsLabel = ownRecordsLabel;
		_worldRecordsLabel = worldRecordsLabel;
		_introContinueButton = introContinueButton;
		_endMainMenuButton = endMainMenuButton;
		_finalScoreboardQuitButton = finalScoreboardQuitButton;
		_mainMenu = mainMenu;
		_scorekeeper.setMapChapterUnlocker (unlocker);
	}
}
