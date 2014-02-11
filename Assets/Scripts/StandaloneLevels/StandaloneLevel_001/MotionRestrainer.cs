using UnityEngine;

public class MotionRestrainer : MonoBehaviour
{
  private float ratio = 0.95f;
  static private float radius = 10f;
  static private float sqrRadius = radius*radius;
  private Vector3 _initialPosition;
  //private CellControl _cellControl;

  void Start()
  {
    _initialPosition = transform.position;
    //_cellControl = GameObject.Find("Perso").GetComponent<CellControl>();
  }

  void Update()
  {
    Vector3 deltaPosition = _initialPosition - transform.position;
    if((_initialPosition - transform.position).sqrMagnitude >= sqrRadius)
    {
      Vector3 newPosition = deltaPosition.normalized*ratio*radius + _initialPosition;
      transform.position = newPosition;
    }
  }
}

