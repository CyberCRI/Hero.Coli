using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class AvailableBioBricksManager : MonoBehaviour {
  public GameObject availableBioBrick;
  //width of a displayed BioBrick
  public int _width = 200;

  LinkedList<AvailableDisplayedBioBrick>  _displayedBioBricks   = new LinkedList<AvailableDisplayedBioBrick>();

  private LinkedList<PromoterBrick>       _availablePromoters   = new LinkedList<PromoterBrick>();
  private LinkedList<RBSBrick>            _availableRBS         = new LinkedList<RBSBrick>();
  private LinkedList<GeneBrick>           _availableGenes       = new LinkedList<GeneBrick>();
  private LinkedList<TerminatorBrick>     _availableTerminators = new LinkedList<TerminatorBrick>();

  LinkedList<AvailableDisplayedBioBrick>       _displayableAvailablePromoters   = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>       _displayableAvailableRBS         = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>       _displayableAvailableGenes       = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>       _displayableAvailableTerminators = new LinkedList<AvailableDisplayedBioBrick>();

  private void updateDisplayedBioBricks() {
    _displayableAvailablePromoters   = getDisplayableAvailableBioBricks(_availablePromoters);
    _displayableAvailableRBS         = getDisplayableAvailableBioBricks(_availableRBS);
    _displayableAvailableGenes       = getDisplayableAvailableBioBricks(_availableGenes);
    _displayableAvailableTerminators = getDisplayableAvailableBioBricks(_availableTerminators);
  }

  public Vector3 getNewPosition(int index ) {
    //TODO manage rows and columns
      return availableBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
  }

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(LinkedList<BioBrick> bioBricks) {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (BioBrick brick in bioBricks) {
      result.AddLast(getDisplayableAvailableBioBrick(brick, result.Count));
    }
    return result;
  }

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(LinkedList<PromoterBrick> bioBricks) {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (PromoterBrick brick in bioBricks) {
      result.AddLast(getDisplayableAvailableBioBrick(brick, result.Count));
    }
    return result;
  }

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(LinkedList<GeneBrick> bioBricks) {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (GeneBrick brick in bioBricks) {
      result.AddLast(getDisplayableAvailableBioBrick(brick, result.Count));
    }
    return result;
  }

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(LinkedList<RBSBrick> bioBricks) {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (RBSBrick brick in bioBricks) {
      result.AddLast(getDisplayableAvailableBioBrick(brick, result.Count));
    }
    return result;
  }

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks(LinkedList<TerminatorBrick> bioBricks) {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (TerminatorBrick brick in bioBricks) {
      result.AddLast(getDisplayableAvailableBioBrick(brick, result.Count));
    }
    return result;
  }

  private AvailableDisplayedBioBrick getDisplayableAvailableBioBrick(BioBrick brick, int index) {
    AvailableDisplayedBioBrick resultBrick = (AvailableDisplayedBioBrick)AvailableDisplayedBioBrick.Create(
          transform
          ,getNewPosition(index)
          ,AvailableDisplayedBioBrick.getSpriteName(brick)
          ,availableBioBrick
        );
    return resultBrick;
  }

  public void displayPromoters() {
    Logger.Log("AvailableBioBricksManager::displayPromoters");
    switchTo(_displayableAvailablePromoters);
  }
  public void displayRBS() {
    Logger.Log("AvailableBioBricksManager::displayRBS");
    switchTo(_displayableAvailableRBS);
  }
  public void displayGenes() {
    Logger.Log("AvailableBioBricksManager::displayGenes");
    switchTo(_displayableAvailableGenes);
  }
  public void displayTerminators() {
    Logger.Log("AvailableBioBricksManager::displayTerminators");
    switchTo(_displayableAvailableTerminators);
  }
  private void switchTo(LinkedList<AvailableDisplayedBioBrick> list) {
    Logger.Log("AvailableBioBricksManager::switchTo("+list+")");
    _displayedBioBricks.Clear();
    _displayedBioBricks.AppendRange(list);
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
