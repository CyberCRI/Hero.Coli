using UnityEngine;
using System.Collections;
using System;

public class PhysicalMedium : MonoBehaviour
{
	public bool triggerFick = false;
	public int MediumId;
	public float Size;
	
	private ReactionEngine _reactionEngine;
	
	void Start(){
		Logger.Log("PhysicalMedium::Start "+MediumId, Logger.Level.TRACE);
    _reactionEngine = ReactionEngine.get ();
	}
	
	void OnTriggerEnter (Collider collider)
	{
		Logger.Log("PhysicalMedium::OnTriggerEnter "+MediumId+" collider="+collider, Logger.Level.TRACE);
		if(triggerFick){
			PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium> ();
			if (PMext == null)
				return;
			int colliderMediumIdExt = PMext.MediumId;
			Medium colliderMediumExt = ReactionEngine.getMediumFromId (colliderMediumIdExt, _reactionEngine.getMediumList ());
			if (colliderMediumExt == null) {
				Debug.Log ("The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	
			if (PMext == null)
				return;
			Medium medium = ReactionEngine.getMediumFromId (MediumId, _reactionEngine.getMediumList ());
			if (medium == null) {
				Debug.Log ("The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	    
			float surface = Math.Min (Size, PMext.Size);
			Fick fick = _reactionEngine.getFick ();
			FickReaction reaction = Fick.getFickReactionFromIds (colliderMediumIdExt, MediumId, fick.getFickReactions ());
			if (reaction == null) {
				Debug.Log ("This FickReaction does not exist.");
	        
			}
			reaction.setSurface (surface);
		}
	}

	public void OnTriggerExit (Collider collider)
	{
		Logger.Log("PhysicalMedium::OnTriggerExit "+MediumId+" collider="+collider, Logger.Level.TRACE);
		if(triggerFick){
			PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium> ();
			if (PMext == null)
				return;
			int colliderMediumIdExt = PMext.MediumId;
			Medium colliderMediumExt = ReactionEngine.getMediumFromId (colliderMediumIdExt, _reactionEngine.getMediumList ());
			if (colliderMediumExt == null) {
				Debug.Log ("The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	
			if (PMext == null)
				return;
			Medium medium = ReactionEngine.getMediumFromId (MediumId, _reactionEngine.getMediumList ());
			if (medium == null) {
				Debug.Log ("The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	    
			Fick fick = _reactionEngine.getFick ();
			FickReaction reaction = Fick.getFickReactionFromIds (colliderMediumIdExt, MediumId, fick.getFickReactions ());
			if (reaction == null) {
				Debug.Log ("This FickReaction does not exist.");
	        
			}
			reaction.setSurface (0);
		}
	}
	
}
