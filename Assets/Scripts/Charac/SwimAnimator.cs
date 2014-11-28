using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwimAnimator : MonoBehaviour {

  private List<Animation> anims = new List<Animation>();

  public void setSpeed(float speed)
  {
    foreach(Animation anim in anims) {
      foreach (AnimationState state in anim) {
        state.speed = speed;
      }
    }
  }

  void Awake()
  {
    anims = new List<Animation>();
  }

	// Use this for initialization
	void Start () {
    anims = new List<Animation>(GetComponentsInChildren<Animation>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
