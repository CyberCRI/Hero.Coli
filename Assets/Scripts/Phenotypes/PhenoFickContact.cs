using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represents a fick activation
 \details This Phenotype enable the contact surface between two medium when they collide together
 
 
 */
public class PhenoFickContact : Phenotype
{

    public VectrosityPanel vectroPanel;
    public GraphMoleculeList graphMoleculeList;
    private int _vectroPanelInitMediumId = 2;
    private LinkedList<int> _collidedMediumIds = new LinkedList<int>();

    //! Called at the beginning
    public override void StartPhenotype()
    {
    }

    public override void UpdatePhenotype()
    {
    }

    private void configureExternalDisplays(int mediumId)
    {
        vectroPanel.setMedium(mediumId);
        graphMoleculeList.setMediumId(mediumId);
    }

    void OnTriggerEnter(Collider collider)
    {
        // Debug.Log(this.GetType() + " OnTriggerEnter collider=" + collider);
        PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium>();
        if (PMext == null)
        {
            // Debug.Log(this.GetType() + " OnTriggerEnter collider.PMext == null");
            return;
        }
        int colliderMediumIdExt = PMext.MediumId;
        Medium colliderMediumExt = ReactionEngine.getMediumFromId(colliderMediumIdExt, _reactionEngine.getMediumList());
        if (colliderMediumExt == null)
        {
            Debug.LogWarning(this.GetType() + " OnTriggerEnter The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
            return;
        }

        PhysicalMedium PM = GetComponent<PhysicalMedium>();
        if (PM == null)
        {
            Debug.LogWarning(this.GetType() + " OnTriggerEnter PM == null");
            return;
        }

        float surface = Math.Min(PM.Size, PMext.Size);
        Fick fick = _reactionEngine.getFick();
        FickReaction reaction = Fick.getFickReactionFromIds(colliderMediumIdExt, Hero.mediumId, fick.getFickReactions());
        if (reaction == null)
        {
            Debug.LogWarning(this.GetType() + " OnTriggerEnter This FickReaction does not exist.");
            return;
        }
        reaction.setSurface(surface);

        // set medium as medium of collider
        // Debug.Log(this.GetType() + " colliderMediumIdExt : " + colliderMediumIdExt);
        configureExternalDisplays(colliderMediumIdExt);
        _collidedMediumIds.AddLast(colliderMediumIdExt);

        /*
        // Debug.Log(this.GetType() + " OnTriggerEnter"
          +" reaction.setSurface("+surface+")"
          +" _collidedMediumIds.Count="+_collidedMediumIds.Count
          +" _collidedMediumIds.Last.Value="+_collidedMediumIds.Last.Value
          );
        */
    }

    public void onDied()
    {
        reset();
    }

    public void OnTriggerExit(Collider collider)
    {
        // Debug.Log(this.GetType() + " OnTriggerExit collider=" + collider);
        PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium>();
        if (PMext != null)
        {
            pop(PMext.MediumId);
        }
    }

    private void reset()
    {
        List<int> treatedMediums = new List<int>();
        foreach (int mediumId in _collidedMediumIds)
        {
            if (!treatedMediums.Contains(mediumId))
            {
                removeFick(mediumId);
                treatedMediums.Add(mediumId);
            }
        }
        _collidedMediumIds.Clear();
    }

    private void pop(int mediumId, bool setDisplay = true)
    {
        _collidedMediumIds.Remove(mediumId);
        if (!_collidedMediumIds.Contains(mediumId))
        {
            removeFick(mediumId);
        }
        if (setDisplay)
        {
            setExternalDisplay();
        }
    }

    private void setExternalDisplay()
    {
        if (null != _collidedMediumIds.Last)
        {
            // TODO consider the current medium as superposition of mediums the ids of which are _collidedMediumIds
            configureExternalDisplays(_collidedMediumIds.Last.Value);
        }
        else
        {
            //not in any Fick contact anymore
            configureExternalDisplays(_vectroPanelInitMediumId);
        }
    }

    private void removeFick(int mediumId)
    {
        Fick fick = _reactionEngine.getFick();
        FickReaction reaction = Fick.getFickReactionFromIds(mediumId, Hero.mediumId, fick.getFickReactions());
        if (reaction == null)
        {
            Debug.LogWarning("FickReaction(" + mediumId + ", " + Hero.mediumId + ") not found");
        }
        else
        {
            reaction.setSurface(0);
        }
    }
}