using UnityEngine;
using System.Collections;

public class RandomRename : MonoBehaviour {
  public CraftFinalizer craftFinalizer;

	void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("RandomRename::OnPress()", Logger.Level.INFO);
      craftFinalizer.randomRename();
    }
  }
}
