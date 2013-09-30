using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO:
 * Replace LinkedList by an array or fields
 * OnHover for DisplayedBioBrick
 * OnPress for AvailableDisplayedBioBrick + Update of _currentCraftBricks in CraftZone
 * OnPress for CraftZoneDisplayedBioBrick + Update of _currentCraftBricks in CraftZone
 * Update of state of CraftFinalizationButton
 */

//TODO refactor CraftZoneManager and AvailableBioBricksManager?
public class CraftZoneManager : MonoBehaviour {

  private LinkedList<DisplayedBioBrick> _currentCraftBricks;
  public GameObject displayedBioBrick;
  public LastHoveredInfoManager lastHoveredInfoManager;

  //width of a displayed BioBrick
  public int _width = 200;


  public LinkedList<DisplayedBioBrick> getCurrentBioBricks() {
    return new LinkedList<DisplayedBioBrick>(_currentCraftBricks);
  }

  private Vector3 getNewPosition(int index ) {
      return displayedBioBrick.transform.localPosition + new Vector3(index*_width, 0.0f, -0.1f);
  }

	// Use this for initialization
	void Start () {

    Logger.Log("CraftZoneManager::Start starting...");
    //promoter
    float beta = 10.0f;
    string formula = "![0.8,2]LacI";
    //rbs
    float rbsFactor = 1.0f;
    //gene
    string proteinName = DevicesDisplayer.getRandomProteinName();
    //terminator
    float terminatorFactor = 1.0f;

    string notNullName = "craftTest";
    BioBrick[] bioBrickArray = {
      new PromoterBrick(notNullName+"_promoter", beta, formula),
      new RBSBrick(notNullName+"_rbs", rbsFactor),
      new GeneBrick(notNullName+"_gene", proteinName),
      new TerminatorBrick(notNullName+"_terminator", terminatorFactor)
    };

    DisplayedBioBrick[] dBioBrickArray = {
      DisplayedBioBrick.Create(transform, getNewPosition(0), "promoter", bioBrickArray[0])
      ,DisplayedBioBrick.Create(transform, getNewPosition(1), "RBS", bioBrickArray[1])
      ,DisplayedBioBrick.Create(transform, getNewPosition(2), "gene", bioBrickArray[2])
      ,DisplayedBioBrick.Create(transform, getNewPosition(3), "terminator", bioBrickArray[3])
    };

    _currentCraftBricks = new LinkedList<DisplayedBioBrick>( dBioBrickArray );

    //to initialize the "last hovered biobrick" info window
    lastHoveredInfoManager.setHoveredBioBrick(dBioBrickArray[0]._biobrick);

    Logger.Log("CraftZoneManager::Start ...ending!");
	}

  private DisplayedBioBrick findFirstBioBrick(BioBrick.Type type) {
    foreach(DisplayedBioBrick brick in _currentCraftBricks) {
      if(brick._biobrick.getType() == type) return brick;
    }
    Logger.Log("CraftZoneManager::findFirstBioBrick("+type+") failed with current bricks="+_currentCraftBricks);
    return null;
  }

  public void replaceWithBrick(DisplayedBioBrick dBioBrick) {

    DisplayedBioBrick toReplace = findFirstBioBrick(dBioBrick._biobrick.getType());
    LinkedListNode<DisplayedBioBrick> toReplaceNode = _currentCraftBricks.Find(toReplace);

    DisplayedBioBrick newBrick = DisplayedBioBrick.Create(
      transform,
      toReplace.transform.localPosition,
      dBioBrick._sprite.spriteName,
      dBioBrick._biobrick
      );

    _currentCraftBricks.AddAfter(toReplaceNode, newBrick);
    _currentCraftBricks.Remove(toReplace);
    Destroy(toReplace.gameObject);
  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
