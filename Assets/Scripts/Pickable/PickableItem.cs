using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PickableItem : MonoBehaviour {
  protected DNABit _dnaBit;
  public GameObject toDestroy;

  public DNABit getDNABit()
  {
    return _dnaBit;
  }

  protected abstract void addTo();

  public void pickUp()
  {
    List<IPickable> pickables;
    Tools.GetInterfaces<IPickable>(out pickables, gameObject);
    foreach(IPickable pickable in pickables)
    {
      pickable.OnPickedUp();
    }
		
    addTo();
    if(toDestroy)
    {
      Destroy(toDestroy);
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
