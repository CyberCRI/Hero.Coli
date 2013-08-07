using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftZoneManager : MonoBehaviour {
  LinkedList<DisplayedBioBrick> bricks;
  public GameObject displayedBioBrick;
  //width of a displayed BioBrick
  public int _width = 52;


  public Vector3 getNewPosition(int index ) {
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
      DisplayedBioBrick.Create(transform, getNewPosition(0), bioBrickArray[0], this, "promoter"),
      DisplayedBioBrick.Create(transform, getNewPosition(1), bioBrickArray[1], this, "RBS"),
      DisplayedBioBrick.Create(transform, getNewPosition(2), bioBrickArray[2], this, "gene"),
      DisplayedBioBrick.Create(transform, getNewPosition(3), bioBrickArray[3], this, "terminator")
    };

    bricks = new LinkedList<DisplayedBioBrick>( dBioBrickArray );

    Logger.Log("CraftZoneManager::Start ...ending!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
