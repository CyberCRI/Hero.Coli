using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represent a fick activation
 \details This Phenotype enable the contact surface between two medium when they collide together
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 */
public class PhenoFickContact : Phenotype {

  //! Called at the beginning
  public override void StartPhenotype()
  {
  }

  public override void UpdatePhenotype()
  {
    Logger.Log("PhenoFickContact", Logger.Level.ONSCREEN);
  }

  public VectrosityPanel vectroPanel;
  private int _vectroPanelInitMediumId = 2;
  private LinkedList<int> _collidedMediumIds = new LinkedList<int>();

  void OnTriggerEnter(Collider collider)
  {
  	Logger.Log("PhenoFickContact::OnTriggerEnter collider="+collider, Logger.Level.DEBUG);
      PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium>();
      if (PMext == null) {
  	  Logger.Log("PhenoFickContact::OnTriggerEnter collider.PMext == null", Logger.Level.TRACE);
        return;
  	}
    int colliderMediumIdExt = PMext.MediumId;
    Medium colliderMediumExt = ReactionEngine.getMediumFromId(colliderMediumIdExt, _reactionEngine.getMediumList());
    if (colliderMediumExt == null)
      {
        Logger.Log("PhenoFickContact::OnTriggerEnter The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.", Logger.Level.WARN);
        return ;
      }

    PhysicalMedium PM = GetComponent<PhysicalMedium>();
    if (PMext == null) {
	  Logger.Log("PhenoFickContact::OnTriggerEnter this.PMext == null", Logger.Level.TRACE);
      return;
    }
    int mediumId = PM.MediumId;
    Medium medium = ReactionEngine.getMediumFromId(mediumId, _reactionEngine.getMediumList());
    if (medium == null)
      {
        Logger.Log("PhenoFickContact::OnTriggerEnter The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.", Logger.Level.WARN);
        return ;
      }
    
    float surface = Math.Min(PM.Size, PMext.Size);
    Fick fick = _reactionEngine.getFick();
    FickReaction reaction = Fick.getFickReactionFromIds(colliderMediumIdExt, mediumId, fick.getFickReactions());
    if (reaction == null)
    {
      Logger.Log("PhenoFickContact::OnTriggerEnter This FickReaction does not exist.", Logger.Level.WARN);
      return;
    }
    Logger.Log("PhenoFickContact::OnTriggerEnter reaction.setSurface("+surface+");", Logger.Level.TEMP);
    reaction.setSurface(surface);

    // set medium as medium of collider
    vectroPanel.setMedium(colliderMediumIdExt);
    _collidedMediumIds.AddLast(colliderMediumIdExt);
    Logger.Log("PhenoFickContact::OnTriggerEnter"
      +" reaction.setSurface("+surface+")"
      +" _collidedMediumIds.Count="+_collidedMediumIds.Count
      +" _collidedMediumIds.Last.Value="+_collidedMediumIds.Last.Value
      ,Logger.Level.TEMP);
  }

  public void OnTriggerExit(Collider collider)
  {
  	Logger.Log("PhenoFickContact::OnTriggerExit collider="+collider, Logger.Level.DEBUG);
    PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium>();
    if (PMext == null)
    {
      Logger.Log("PhenoFickContact::OnTriggerExit collider.PMext == null", Logger.Level.TRACE);
      return;
  	}
    int colliderMediumIdExt = PMext.MediumId;
    Medium colliderMediumExt = ReactionEngine.getMediumFromId(colliderMediumIdExt, _reactionEngine.getMediumList());
    if (colliderMediumExt == null)
    {
      Logger.Log("PhenoFickContact::OnTriggerExit The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.", Logger.Level.WARN);
      return ;
    }

    PhysicalMedium PM = GetComponent<PhysicalMedium>();
    if (PMext == null)
    {
      Logger.Log("PhenoFickContact::OnTriggerExit this.PMext == null", Logger.Level.TRACE);
      return;
    }
    int mediumId = PM.MediumId;
    Medium medium = ReactionEngine.getMediumFromId(mediumId, _reactionEngine.getMediumList());
    if (medium == null)
    {
      Logger.Log("PhenoFickContact::OnTriggerExit The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.", Logger.Level.WARN);
      return ;
    }

    // un-set medium as medium of collider
    _collidedMediumIds.Remove(colliderMediumIdExt);
    string nullLast = (null != _collidedMediumIds.Last)?_collidedMediumIds.Last.Value.ToString():"null";
    Logger.Log("PhenoFickContact::OnTriggerExit"
      +" _collidedMediumIds.Count="+_collidedMediumIds.Count
      +" _collidedMediumIds.Last.Value="+nullLast
      ,Logger.Level.TEMP);

    //Logger.Log("PhenoFickContact::OnTriggerExit _collidedMediumIds.Last.Value="+nullLast, Logger.Level.TEMP);

    if(null != _collidedMediumIds.Last)
    {
      // TODO consider the current medium as superposition of mediums the ids of which are _collidedMediumIds
      vectroPanel.setMedium(_collidedMediumIds.Last.Value);
      Logger.Log("PhenoFickContact::OnTriggerExit setting medium to previous value "+vectroPanel._mediumId, Logger.Level.WARN);
    }
    else
    {
      //not in any Fick contact anymore
      vectroPanel.setMedium(_vectroPanelInitMediumId);
    
      Fick fick = _reactionEngine.getFick();
      FickReaction reaction = Fick.getFickReactionFromIds(colliderMediumIdExt, mediumId, fick.getFickReactions());
      if (reaction == null)
      {
        Logger.Log("PhenoFickContact::OnTriggerExit This FickReaction does not exist.", Logger.Level.WARN);
      }
      Logger.Log("PhenoFickContact::OnTriggerExit reaction.setSurface(0)", Logger.Level.WARN);
      reaction.setSurface(0);
    }

  }

}