using UnityEngine;

public class SuccessCheckerManager : MonoBehaviour
{
  SuccessChecker _currentSuccessChecker;

  void Start()
  {
    _currentSuccessChecker = gameObject.AddComponent<SuccessChecker_001>();
  }

  void Update()
  {
    if(null != _currentSuccessChecker && _currentSuccessChecker.isSuccess())
    {
      InfoWindowManager.displayInfoWindow(_currentSuccessChecker.getInfoWindowCode());
      _currentSuccessChecker = null;
    }
  }
}
