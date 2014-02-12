using UnityEngine;

public class MotionRestrainer : MonoBehaviour
{
  static private float radius = 50f;
  static private float sqrRadius = radius*radius;
  private Vector3 _initialPosition;

  void Start()
  {
    _initialPosition = transform.position;
  }

  void Update()
  {
    Vector3 deltaPosition = transform.position - _initialPosition;
    if(deltaPosition.sqrMagnitude > sqrRadius)
    {
      Vector3 newPosition = deltaPosition.normalized*radius + _initialPosition;
      transform.position = newPosition;
    }
  }
}

