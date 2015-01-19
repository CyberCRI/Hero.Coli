using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwimAnimator : MonoBehaviour {

  public List<Animation> anims = new List<Animation>();

  public void setSpeed(float speed)
  {
    safeInitAnims();
    foreach(Animation anim in anims) {
      foreach (AnimationState state in anim) {
        state.speed = speed;
      }
    }
  }

	public void safeInitAnims() {
    if(0 == anims.Count)
    {
      Logger.Log("SwimAnimator::safeInitAnims initializing anims", Logger.Level.INFO);
      anims = new List<Animation>(GetComponentsInChildren<Animation>());
    }
	}
}
