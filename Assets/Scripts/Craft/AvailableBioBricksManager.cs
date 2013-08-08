using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvailableBioBricksManager : MonoBehaviour {

  LinkedList<DisplayedBioBrick> _displayedBioBricks = new LinkedList<DisplayedBioBrick>();
  LinkedList<PromoterBrick> _availablePromoters = new LinkedList<PromoterBrick>();
  LinkedList<RBSBrick> _availableRBS = new LinkedList<RBSBrick>();
  LinkedList<GeneBrick> _availableGenes = new LinkedList<GeneBrick>();
  LinkedList<TerminatorBrick> _availableTerminators = new LinkedList<TerminatorBrick>();

  public void displayPromoters() {
    Logger.Log("AvailableBioBricksManager::displayPromoters");
  }
  public void displayRBS() {
    Logger.Log("AvailableBioBricksManager::displayRBS");
  }
  public void displayGenes() {
    Logger.Log("AvailableBioBricksManager::displayGenes");
  }
  public void displayTerminators() {
    Logger.Log("AvailableBioBricksManager::displayTerminators");
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
