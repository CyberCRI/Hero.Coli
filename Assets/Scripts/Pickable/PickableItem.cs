using UnityEngine;
using System.Collections;

public abstract class PickableItem : MonoBehaviour {
  protected DNABit _dnaBit;

  public DNABit getDNABit()
  {
    return _dnaBit;
  }

  protected abstract void addTo();

  public void pickUp()
  {
    addTo ();
    Destroy(gameObject);
  }
}
