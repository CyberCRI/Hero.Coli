using UnityEngine;

public abstract class SuccessChecker : MonoBehaviour
{
  abstract public bool isSuccess();

  abstract public string getInfoWindowCode();
}
