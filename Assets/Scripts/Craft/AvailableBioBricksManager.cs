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

  //biobrick arrays
  private static PromoterBrick[]    promoters   = new PromoterBrick[]{};//{new PromoterBrick("testProm", 0.5f, "promoter1")};
  private static RBSBrick[]         rbs         = new RBSBrick[]{new RBSBrick("testRBS", 1.0f)};
  private static GeneBrick[]        genes       = new GeneBrick[]{new GeneBrick("testGene", "gene1"), new GeneBrick("testGene2", "gene2")};
  private static TerminatorBrick[]  terminators = new TerminatorBrick[]{new TerminatorBrick("testTerminator", 1.0f), new TerminatorBrick("testTerminator2", 1.0f), new TerminatorBrick("testTerminator3", 1.0f)};

  //biobrick data catalog
  private static LinkedList<PromoterBrick>       _availablePromoters   = new LinkedList<PromoterBrick>(promoters);
  private static LinkedList<RBSBrick>            _availableRBS         = new LinkedList<RBSBrick>(rbs);
  private static LinkedList<GeneBrick>           _availableGenes       = new LinkedList<GeneBrick>(genes);
  private static LinkedList<TerminatorBrick>     _availableTerminators = new LinkedList<TerminatorBrick>(terminators);

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
    string listToString = "list=[";
    foreach(AvailableDisplayedBioBrick brick in list) {
      listToString += brick.ToString()+", ";
    }
    listToString += "]";
    Logger.Log("AvailableBioBricksManager::switchTo("+listToString+")");
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
	
	// Update is called once per frame
	void Update () {
	
	}


  string[] _bioBrickFiles = new string[]{ "Assets/Data/raph/biobricks.xml" };
  void Awake () {
    //load biobricks from xml
    BioBrickLoader loader = new BioBrickLoader();
    
    _availablePromoters   = new LinkedList<PromoterBrick>();
    _availableRBS         = new LinkedList<RBSBrick>();
    _availableGenes       = new LinkedList<GeneBrick>();
    _availableTerminators = new LinkedList<TerminatorBrick>();

    foreach (string file in _bioBrickFiles) {
      //LinkedListExtensions.AppendRange<ReactionsSet>(_reactionsSets, fileLoader.loadReactionsFromFile(file));
      loader.loadBioBricksFromFile(file);
      Logger.Log("AvailableBioBricksManager::Awake loads "+file, Logger.Level.WARN);
    }
    Logger.Log("AvailableBioBricksManager::Awake DONE", Logger.Level.WARN);
  }

  /*
  private void sortInto(LinkedList<T> bricks) where T:BioBrick {

  }
  */

}
