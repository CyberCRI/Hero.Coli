using UnityEngine;
using System.Collections;

public class PickableItem : MonoBehaviour {
  private DNABit _dnaBit;
  public AvailableBioBricksManager _availableBioBricksManager;

  void Awake()
  {
    Logger.Log("PickableItem::Start", Logger.Level.TEMP);
    //_dnaBit = new GeneBrick("TEST", "TEST");
    _dnaBit = new TerminatorBrick("TEST", 12.12f);
  }

  public DNABit getDNABit()
  {
    return _dnaBit;
  }

  public void pickUp()
  {
    _availableBioBricksManager.addAvailableBioBrick((BioBrick) _dnaBit, false);
    //AvailableBioBricksManager.get().addAvailableBioBrick((BioBrick) _dnaBit, false);
    Destroy(gameObject);
  }
}
