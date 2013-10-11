using UnityEngine;
using System.Collections;

public class RandomRename : MonoBehaviour {
  public CraftFinalizer craftFinalizer;

	void OnPress(bool isPressed) {
    Logger.Log ("RandomRename::OnPress("+isPressed+")", Logger.Level.WARN);
    craftFinalizer.randomRename();
  }
}
