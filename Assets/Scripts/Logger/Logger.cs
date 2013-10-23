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
  private static Level _level = Level.INFO;

  //TODO "inline" this
  public static void Log(string debugMsg, Level level = Level.DEBUG) {
    if(level >= _level) {
      string timedMsg = DateTime.Now.ToString("hh:mm:ss:ffffff") +" "+debugMsg;
      if (level == Level.WARN) {
        Debug.LogWarning(timedMsg);
      } else if (level == Level.ERROR) {
        Debug.LogError(timedMsg);
      } else {
        Debug.Log(level.ToString()+" "+ timedMsg);
      }
    }
  }

  public static string ToString<T>(ICollection<T> objects, string separator = defaultSeparator) {
    T[] array = new T[objects.Count];
    objects.CopyTo(array, 0);
    return string.Join(separator, Array.ConvertAll(array, o => o.ToString()));
  }
}
