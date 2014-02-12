using UnityEngine;

public class SuccessChecker_001 : SuccessChecker
{
  private float _timeSuccess = 1000;
  private float _timeAtLastFrame = 0f;
  private float _timeAtCurrentFrame = 0f;
  private float _deltaTime = 0f;

  private bool _succeeded = false;

  override public bool isSuccess()
  {
    if(!_succeeded)
    {
      _timeAtCurrentFrame = Time.realtimeSinceStartup;
      _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
      if(_deltaTime > _timeSuccess) {
        _succeeded = true;
        return true;
      }
      else
      {
        return false;
      }
    }
    else
    {
      return true;
    }
  }

  override public string getInfoWindowCode()
  {
    return "PickUpDevice";
  }
}