using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Logger : MonoBehaviour {
  public const string defaultSeparator = ", ";
  public enum Level {
    ALL,
    TRACE,
    DEBUG,
    INFO,
    WARN,
    ERROR
  }
  private static Level _level = Level.ALL;

  public static void Log(string debugMsg, Level level = Level.DEBUG) {
    if(level >= _level) Debug.Log(level.ToString()+" "+ DateTime.Now.ToString("hh:mm:ss") +" "+debugMsg);
  }

  public static string ToString<T>(ICollection<T> objects, string separator = defaultSeparator) {
    T[] array = new T[objects.Count];
    objects.CopyTo(array, 0);
    return string.Join(separator, Array.ConvertAll(array, o => o.ToString()));
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
