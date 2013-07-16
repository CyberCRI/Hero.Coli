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



  //! Called at the begening
  public override void StartPhenotype()
  {
  }

  public override void UpdatePhenotype()
  {
  }

  private void ModifyColliderFickSurface(float surface, Collider collider)
  {
    
  }

  void OnTriggerEnter(Collider collider)
  {
    PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium>();
    if (PMext == null)
      return;
    int colliderMediumIdExt = PMext.MediumId;
    Medium colliderMediumExt = ReactionEngine.getMediumFromId(colliderMediumIdExt, _RE.getMediumList());
    if (colliderMediumExt == null)
      {
        Debug.Log("The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
        return ;
      }

    PhysicalMedium PM = GetComponent<PhysicalMedium>();
    if (PMext == null)
      return;
    int mediumId = PM.MediumId;
    Medium medium = ReactionEngine.getMediumFromId(mediumId, _RE.getMediumList());
    if (medium == null)
      {
        Debug.Log("The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
        return ;
      }
    
    float surface = Math.Min(PM.Size, PMext.Size);
    Fick fick = _RE.getFick();
    FickReaction reaction = Fick.getFickReactionFromIds(colliderMediumIdExt, mediumId, fick.getFickReactions());
    if (reaction == null)
      {
        Debug.Log("This FickReaction does not exist.");
        
      }
    reaction.setSurface(surface);
  }

  public void OnTriggerExit(Collider collider)
  {
    PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium>();
    if (PMext == null)
      return;
    int colliderMediumIdExt = PMext.MediumId;
    Medium colliderMediumExt = ReactionEngine.getMediumFromId(colliderMediumIdExt, _RE.getMediumList());
    if (colliderMediumExt == null)
      {
        Debug.Log("The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
        return ;
      }

    PhysicalMedium PM = GetComponent<PhysicalMedium>();
    if (PMext == null)
      return;
    int mediumId = PM.MediumId;
    Medium medium = ReactionEngine.getMediumFromId(mediumId, _RE.getMediumList());
    if (medium == null)
      {
        Debug.Log("The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
        return ;
      }
    
    Fick fick = _RE.getFick();
    FickReaction reaction = Fick.getFickReactionFromIds(colliderMediumIdExt, mediumId, fick.getFickReactions());
    if (reaction == null)
      {
        Debug.Log("This FickReaction does not exist.");
        
      }
    reaction.setSurface(0);
  }

  public void OnTriggerStay(Collider collisionInfo)
  {
    
//     foreach (ContactPoint contact in collisionInfo.contacts)
//       {
//         Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
//       }
  }
}