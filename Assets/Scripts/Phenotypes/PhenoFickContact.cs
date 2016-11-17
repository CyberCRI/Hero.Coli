// #define DEV

using UnityEngine;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represents a fick activation
 \details This Phenotype enables the contact surface between two mediums when they collide together
 */
public class PhenoFickContact : Phenotype
{

    public VectrosityPanel vectroPanel;
    public GraphMoleculeList graphMoleculeList;
    private int _vectroPanelInitMediumId = 2;
    private LinkedList<int> _collidedMediumIds = new LinkedList<int>();
    private LinkedList<int> _collidedHashCodes = new LinkedList<int>();

#if DEV
    [SerializeField]
    private int[] _collidedMediumIdsTab;
    [SerializeField]
    private int[] _collidedHashCodesTab;
#endif

    private Fick _fick;

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
        // Debug.Log(this.GetType() + " OnTriggerEnter collider=" + collider.gameObject.name);

        int hashCode = collider.gameObject.GetHashCode();
        if (_collidedHashCodes.Contains(hashCode))
        {
            // Debug.Log(this.GetType() + " OnTriggerEnter already in collision with " + collider.gameObject.name);
            return;
        }

        PhysicalMedium extPM = collider.gameObject.GetComponent<PhysicalMedium>();
        if (extPM == null)
        {
            // Debug.Log(this.GetType() + " OnTriggerEnter collider.extPM == null");
            return;
        }
        int extColliderMediumId = extPM.MediumId;
        Medium extColliderMedium = ReactionEngine.getMediumFromId(extColliderMediumId, _reactionEngine.getMediumList());
        if (null == extColliderMedium)
        {
            Debug.LogWarning(this.GetType() + " OnTriggerEnter The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
            return;
        }

        PhysicalMedium pm = GetComponent<PhysicalMedium>();
        if (null == pm)
        {
            Debug.LogWarning(this.GetType() + " OnTriggerEnter PM == null");
            return;
        }

        float surface = Math.Min(pm.Size, extPM.Size);
        if (null == _fick)
        {
            _fick = _reactionEngine.getFick();
        }
        FickReaction reaction = Fick.getFickReactionFromIds(extColliderMediumId, Hero.mediumId, _fick.getFickReactions());
        if (null == reaction)
        {
            Debug.LogWarning(this.GetType() + " OnTriggerEnter This FickReaction does not exist.");
            return;
        }
        reaction.setSurface(surface);

        // set medium as medium of collider
        // Debug.Log(this.GetType() + " colliderMediumIdExt : " + extColliderMediumId);
        configureExternalDisplays(extColliderMediumId);
        _collidedMediumIds.AddLast(extColliderMediumId);
        _collidedHashCodes.AddLast(hashCode);
#if DEV
        updateTabs(" entered " + collider.gameObject.name);
#endif

        // Debug.Log(this.GetType() + " OnTriggerEnter"
        //   +" reaction.setSurface("+surface+")"
        //   +" _collidedMediumIds.Count="+_collidedMediumIds.Count
        //   +" _collidedMediumIds.Last.Value="+_collidedMediumIds.Last.Value
        //   );
    }

    public void onDied()
    {
        reset();
    }

    public void OnTriggerExit(Collider collider)
    {
        // Debug.Log(this.GetType() + " OnTriggerExit collider=" + collider.gameObject.name);

        int hashCode = collider.gameObject.GetHashCode();
        if (!_collidedHashCodes.Contains(hashCode))
        {
            // Debug.Log(this.GetType() + " OnTriggerEnter was not in collision with " + collider.gameObject.name);
            return;
        }

        PhysicalMedium extPM = collider.gameObject.GetComponent<PhysicalMedium>();
        if (extPM != null)
        {
#if DEV
            pop(extPM.MediumId, hashCode, collider);
#else
            pop(extPM.MediumId, hashCode);
#endif
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
        _collidedHashCodes.Clear();
#if DEV
        updateTabs(" reset");
#endif
    }

#if DEV
    private void pop(int mediumId, int hashCode, Collider collider, bool setDisplay = true)
#else    
    private void pop(int mediumId, int hashCode, bool setDisplay = true)
#endif
    {
        _collidedMediumIds.Remove(mediumId);
        _collidedHashCodes.Remove(hashCode);
        if (!_collidedMediumIds.Contains(mediumId))
        {
            removeFick(mediumId);
        }
        if (setDisplay)
        {
            setExternalDisplay();
        }
#if DEV
        updateTabs(" left " + collider.gameObject.name);
#endif
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

#if DEV
    private void updateTabs(string debugString)
    {
        // Debug.Log(this.GetType() + debugString);
        _collidedMediumIdsTab = new int[_collidedMediumIds.Count];
        int idx = 0;
        foreach (int medId in _collidedMediumIds)
        {
            _collidedMediumIdsTab[idx] = medId;
            idx++;
        }

        _collidedHashCodesTab = new int[_collidedHashCodes.Count];
        idx = 0;
        foreach (int hashCode in _collidedHashCodes)
        {
            _collidedMediumIdsTab[idx] = hashCode;
            idx++;
        }
    }
#endif
}