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
    ALL,          // gets everything logged
	ONSCREEN,     // gets printed in the Logger window
	INTERACTIVE,  // gets logged when Interactive mode is on - press J
    TRACE,        // for step by step follow up on computation
    DEBUG,        // for functions calls
    INFO,         // for events that help finding out the sequence of events leading to a bug
    WARN,         // for unhandled, unexpected events that don't stop the program
    ERROR         // only for crashes or events threatening the program flow
  }
  private static Level _level = Level.DEBUG;
  private static Level _previousLevel = Level.DEBUG;
	
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
      if (level == Level.WARN) {
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
			return null;	
		} else {
			string left = ToString<T>(tree.getLeftNode()), right = ToString<T>(tree.getRightNode());
			string resultString = tree.getData().ToString();
			resultString = !string.IsNullOrEmpty(left)?resultString+", "+left:resultString;
			resultString = !string.IsNullOrEmpty(right)?resultString+", "+right:resultString;
			return resultString;
		}
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
	}
  }
	
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
}
