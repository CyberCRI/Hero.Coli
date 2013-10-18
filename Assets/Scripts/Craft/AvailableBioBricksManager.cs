using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class AvailableBioBricksManager : MonoBehaviour {

  //width of a displayed BioBrick
  public int _width = 12;

  //prefab for available biobricks
  public GameObject availableBioBrick;

  //visual, clickable biobricks currently displayed
  LinkedList<AvailableDisplayedBioBrick>  _displayedBioBricks   = new LinkedList<AvailableDisplayedBioBrick>();

  //biobrick data catalog
  private static LinkedList<PromoterBrick>       _availablePromoters   = new LinkedList<PromoterBrick>();
  private static LinkedList<RBSBrick>            _availableRBS         = new LinkedList<RBSBrick>();
  private static LinkedList<GeneBrick>           _availableGenes       = new LinkedList<GeneBrick>();
  private static LinkedList<TerminatorBrick>     _availableTerminators = new LinkedList<TerminatorBrick>();

  //visual, clickable biobrick catalog
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailablePromoters   = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailableRBS         = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailableGenes       = new LinkedList<AvailableDisplayedBioBrick>();
  LinkedList<AvailableDisplayedBioBrick>  _displayableAvailableTerminators = new LinkedList<AvailableDisplayedBioBrick>();

  private void updateDisplayedBioBricks() {
    _displayableAvailablePromoters   = getDisplayableAvailableBioBricks<PromoterBrick>(
      _availablePromoters
      , getDisplayableAvailableBioBrick<PromoterBrick>
      );
    _displayableAvailableRBS         = getDisplayableAvailableBioBricks<RBSBrick>(
      _availableRBS
      , getDisplayableAvailableBioBrick<RBSBrick>
      );
    _displayableAvailableGenes       = getDisplayableAvailableBioBricks<GeneBrick>(
      _availableGenes
      , getDisplayableAvailableBioBrick<GeneBrick>
      );
    _displayableAvailableTerminators = getDisplayableAvailableBioBricks<TerminatorBrick>(
      _availableTerminators
      , getDisplayableAvailableBioBrick<TerminatorBrick>
      );
  }

  public Vector3 getNewPosition(int index ) {
    //TODO manage rows and columns
      return availableBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
  }

  private delegate AvailableDisplayedBioBrick DisplayableAvailableBioBrickCreator<T>(T brick, int index) where T:BioBrick;

  private LinkedList<AvailableDisplayedBioBrick> getDisplayableAvailableBioBricks<T>(
    LinkedList<T> bioBricks
    , DisplayableAvailableBioBrickCreator<T> creator
  ) where T:BioBrick {
    LinkedList<AvailableDisplayedBioBrick> result = new LinkedList<AvailableDisplayedBioBrick>();
    foreach (T brick in bioBricks) {
      AvailableDisplayedBioBrick availableBrick = creator(brick, result.Count);
      availableBrick.display(false);
      result.AddLast(availableBrick);
    }
    return result;
  }

  private AvailableDisplayedBioBrick getDisplayableAvailableBioBrick<T>(T brick, int index) where T:BioBrick {

    Transform parentTransformParam = transform;
    Vector3 localPositionParam = getNewPosition(index);
    string spriteNameParam = AvailableDisplayedBioBrick.getSpriteName(brick);
    T biobrickParam = brick;

    Logger.Log("AvailableBioBricksManager::getDisplayableAvailableBioBrick(brick="+brick+", index="+index+"),"
      +", parentTransformParam="+parentTransformParam
      +", localPositionParam="+localPositionParam
      +" (width="+_width+")"
      +", spriteNameParam="+spriteNameParam
      +", biobrickParam="+biobrickParam
      , Logger.Level.TRACE
      );

    AvailableDisplayedBioBrick resultBrick = AvailableDisplayedBioBrick.Create(
      parentTransformParam
      ,localPositionParam
      ,spriteNameParam
      ,biobrickParam
    );
    return resultBrick;
  }

  public void displayPromoters() {
    Logger.Log("AvailableBioBricksManager::displayPromoters", Logger.Level.TRACE);
    switchTo(_displayableAvailablePromoters);
  }
  public void displayRBS() {
    Logger.Log("AvailableBioBricksManager::displayRBS", Logger.Level.TRACE);
    switchTo(_displayableAvailableRBS);
  }
  public void displayGenes() {
    Logger.Log("AvailableBioBricksManager::displayGenes", Logger.Level.TRACE);
    switchTo(_displayableAvailableGenes);
  }
  public void displayTerminators() {
    Logger.Log("AvailableBioBricksManager::displayTerminators", Logger.Level.TRACE);
    switchTo(_displayableAvailableTerminators);
  }
  private void switchTo(LinkedList<AvailableDisplayedBioBrick> list) {
    string listToString = "list=[";
    foreach(AvailableDisplayedBioBrick brick in list) {
      listToString += brick.ToString()+", ";
    }
    listToString += "]";
    Logger.Log("AvailableBioBricksManager::switchTo("+listToString+")", Logger.Level.TRACE);
    display(_displayedBioBricks, false);
    _displayedBioBricks.Clear();
    _displayedBioBricks.AppendRange(list);
    display(_displayedBioBricks, true);
  }

  private void display(LinkedList<AvailableDisplayedBioBrick> bricks, bool enabled) {
    foreach (AvailableDisplayedBioBrick brick in bricks) {
      brick.display(enabled);
    }
  }

	// Use this for initialization
	void Start () {
	  updateDisplayedBioBricks();
    displayPromoters();
	}

  string[] _bioBrickFiles = new string[]{ "Assets/Data/raph/biobricks.xml" };
  void Awake () {
    Logger.Log("AvailableBioBricksManager::Awake", Logger.Level.INFO);
    //load biobricks from xml
    BioBrickLoader loader = new BioBrickLoader();
    
    _availablePromoters   = new LinkedList<PromoterBrick>();
    _availableRBS         = new LinkedList<RBSBrick>();
    _availableGenes       = new LinkedList<GeneBrick>();
    _availableTerminators = new LinkedList<TerminatorBrick>();

    foreach (string file in _bioBrickFiles) {
      Logger.Log("AvailableBioBricksManager::Awake loads "+file, Logger.Level.TRACE);
      sortInto(loader.loadBioBricksFromFile(file));
    }
    Logger.Log("AvailableBioBricksManager::Awake loaded "+_bioBrickFiles, Logger.Level.INFO);
  }

  private void sortInto(LinkedList<BioBrick> bricks) {
    Logger.Log("AvailableBioBricksManager::sortInto START with "+Logger.ToString<BioBrick>(bricks), Logger.Level.TRACE);
    foreach (BioBrick brick in bricks) {
      switch(brick.getType()) {
        case BioBrick.Type.PROMOTER:
          _availablePromoters.AddLast((PromoterBrick)brick);
          break;
        case BioBrick.Type.RBS:
          _availableRBS.AddLast((RBSBrick)brick);
          break;
        case BioBrick.Type.GENE:
          _availableGenes.AddLast((GeneBrick)brick);
          break;
        case BioBrick.Type.TERMINATOR:
          _availableTerminators.AddLast((TerminatorBrick)brick);
          break;
        default:
          Logger.Log("AvailableBioBricksManager::sortInto unknown type "+brick.getType(), Logger.Level.WARN);
          break;
      }
    }
    Logger.Log("AvailableBioBricksManager::sortInto DONE", Logger.Level.TRACE);
  }

}
