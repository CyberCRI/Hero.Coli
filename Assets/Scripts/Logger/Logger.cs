using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
 \brief a Logger class that manages all logs
 \details Press J to log everything at 'Interactive' level; press J again to switch off
  Press H to log everything above 'Trace' level; press H again to switch off
 \author Raphael GOUJET
 \mail raphael.goujet@gmail.com 
 */
public class Logger : MonoBehaviour {
  private static Logger _singleton = null;
  public bool interactiveDebug = true;
  public const string defaultSeparator = ", ";
  public int logLevelIdx;

/*!
 \brief log levels
 \details
    ALL gets everything logged
	ONSCREEN gets printed in the Logger window
	INTERACTIVE gets logged when Interactive mode is on - press J
    TRACE for step by step follow up on computation
    DEBUG for functions calls
    INFO for events that help finding out the sequence of events leading to a bug
    WARN for unhandled, unexpected events that don't stop the program
    ERROR only for crashes or events threatening the program flow
 
 */
  public enum Level {
    ALL,          // 0 gets everything logged
	  ONSCREEN,     // 1 gets printed in the Logger window
	  INTERACTIVE,  // 2 gets logged when Interactive mode is on - press J
    TRACE,        // 3 for step by step follow up on computation
    DEBUG,        // 4 for functions calls
    INFO,         // 5 for events that help finding out the sequence of events leading to a bug
    TEMP,         // 6 temporary development logs, printed out as warning logs
    WARN,         // 7 for unhandled, unexpected events that don't stop the program
    ERROR         // 8 only for crashes or events threatening the program flow
  }
  private static Level _level = Level.INFO; // initialized in Awake()
  private static Level _previousLevel;
	
  private float _timeAtLastFrame = 0f;
  private float _timeAtCurrentFrame = 0f;
  private float _deltaTime = 0f;	
  private float _deltaTimeThreshold = 0.2f;
	
  private List<string> _messages = new List<string>();
	
  public static bool isLevel(Level level) {
	return level == _level;
  }	
	
  public static bool isInteractive() {
    return (_singleton != null) && _singleton.interactiveDebug;
  }	

  //TODO "inline" this
  public static void Log(string debugMsg, Level level = Level.DEBUG) {
	if(level == Level.ONSCREEN) {
	  pushMessage(debugMsg);
	} else if(level >= _level || (isInteractive() && (level == Level.INTERACTIVE))) {
      string timedMsg = level.ToString()+" "+DateTime.Now.ToString("HH:mm:ss:ffffff") +" "+debugMsg;
      if (level == Level.WARN || level == Level.TEMP) {
        Debug.LogWarning(timedMsg);
      } else if (level == Level.ERROR) {
        Debug.LogError(timedMsg);
      } else {
        Debug.Log(timedMsg);
      }
    }
  }

  public static string ToString<T>(ICollection<T> objects, string separator = defaultSeparator) {
    T[] array = new T[objects.Count];
    objects.CopyTo(array, 0);
    return string.Join(separator, Array.ConvertAll(array, o => o.ToString()));
  }
	
  public static string ToString<T>(TreeNode<T> tree, string separator = defaultSeparator) {
		if(tree==null) {
			return "";
		} else {
			string left = ToString<T>(tree.getLeftNode()), right = ToString<T>(tree.getRightNode());
			string resultString = tree.getData().ToString();
			resultString = !string.IsNullOrEmpty(left)?resultString+", "+left:resultString;
			resultString = !string.IsNullOrEmpty(right)?resultString+", "+right:resultString;
			return resultString;
		}
  }

  public static string ToString<TKey,TValue>(IEnumerable<KeyValuePair<TKey, TValue>> enumerable, string separator = defaultSeparator)
  {
    string beginString = enumerable.GetType().ToString()+"[";
    string middleString = "";
    string endString = "]";

    if(enumerable != null)
    {
      foreach (KeyValuePair<TKey, TValue> element in enumerable)
      {
        if(middleString != "")
          middleString += separator;

        middleString += (element.Key + ": "+ element.Value);
      }
    }
    return beginString + middleString + endString;
  }
	
  private static void pushMessage(string msg) {
	_singleton._messages.Add(msg);
  }
	
  public static List<string> popAllMessages() {
	List<string> copy = new List<string>(_singleton._messages);
	_singleton._messages.Clear();
	return copy;
  }
	
  public void Awake() {
	if(_singleton == null) {
	  _singleton = this;
	  switch(logLevelIdx) {
		case 0: _level = Level.ALL;
			break;
		case 1: _level = Level.ONSCREEN;
			break;
		case 2: _level = Level.INTERACTIVE;
			break;
		case 3: _level = Level.TRACE;
			break;
		case 4: _level = Level.DEBUG;
			break;
		case 5: _level = Level.INFO;
			break;
    case 6: _level = Level.TEMP;
      break;
    case 7: _level = Level.WARN;
      break;
		case 8: _level = Level.ERROR;
			break;
	  }
	  _previousLevel = _level;
	}
  }

  /*
  public void Update() {
    _timeAtCurrentFrame = Time.realtimeSinceStartup;
    _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
	
    if(_deltaTime > _deltaTimeThreshold) {
      if (Input.GetKey(KeyCode.J)) {
        interactiveDebug = !interactiveDebug;
        Logger.Log("Logger::Update press J interactiveDebug="+interactiveDebug, Logger.Level.WARN);
        _timeAtLastFrame = _timeAtCurrentFrame;
      }
      if (Input.GetKey(KeyCode.H)) {
		if(_level == Level.TRACE) {
		  _level = _previousLevel;
		} else {
		  _previousLevel = _level;
		  _level = Level.TRACE;
		}        
		Logger.Log("Logger::Update press H _level="+_level, Logger.Level.WARN);
		_timeAtLastFrame = _timeAtCurrentFrame;
      }
    }
  }
  */
}
