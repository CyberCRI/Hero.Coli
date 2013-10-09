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

  private LinkedList<DisplayedBioBrick> _currentCraftBricks = new LinkedList<DisplayedBioBrick>();
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
    Logger.Log("CraftZoneManager::Start starting...", Logger.Level.TRACE);
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
    LinkedList<BioBrick> bioBrickList = new LinkedList<BioBrick>();
    bioBrickList.AddLast(new PromoterBrick(notNullName+"_promoter", beta, formula));
    bioBrickList.AddLast(new RBSBrick(notNullName+"_rbs", rbsFactor));
    bioBrickList.AddLast(new GeneBrick(notNullName+"_gene", proteinName));
    bioBrickList.AddLast(new TerminatorBrick(notNullName+"_terminator", terminatorFactor));
    displayBioBricks(bioBrickList);

    Logger.Log("CraftZoneManager::Start ...ending!", Logger.Level.TRACE);
	}

  public void displayBioBricks(LinkedList<BioBrick> bricks) {
    Debug.Log("CraftZoneManager::displayBioBricks("+Logger.ToString<BioBrick>(bricks)+")");
    //remove all previous biobricks
    foreach (DisplayedBioBrick brick in _currentCraftBricks) {
      Destroy(brick.gameObject);
    }
    _currentCraftBricks.Clear();

    //add new biobricks
    LinkedList<BioBrick>.Enumerator enumerator = bricks.GetEnumerator();
    enumerator.MoveNext();
    int index = 0;
    foreach (BioBrick brick in bricks) {
      Debug.Log("CraftZoneManager::displayBioBricks brick="+brick);
      _currentCraftBricks.AddLast(DisplayedBioBrick.Create(transform, getNewPosition(index), null, brick));
      index++;
    }

    //to initialize the "last hovered biobrick" info window
    lastHoveredInfoManager.setHoveredBioBrick(bricks.First.Value);
  }

  public void displayDevice(Device device) {
    Debug.Log("CraftZoneManager::displayDevice("+device+")");
    LinkedList<ExpressionModule> modules = device.getExpressionModules();
    ExpressionModule firstModule = modules.First.Value;
    LinkedList<BioBrick> bricks = firstModule.getBioBricks();
    displayBioBricks(bricks);
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
