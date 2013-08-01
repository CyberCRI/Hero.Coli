using UnityEngine;
using System.Collections;
using System;

public class PhysicalMedium : MonoBehaviour
{
	public bool triggerFick = false;
	public int MediumId;
	public float Size;
	
	private ReactionEngine _RE;
	
	void Start(){
		GameObject obj = GameObject.Find("ReactionEngine");
		if(obj){
			_RE = obj.GetComponent<ReactionEngine>();
			if(!_RE)
				Debug.LogError("Could not find Reaction Engine");
		}
		else
			Debug.LogError("Could not find Reaction Engine");
	}
	
	void OnTriggerEnter (Collider collider)
	{
		if(triggerFick){
			PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium> ();
			if (PMext == null)
				return;
			int colliderMediumIdExt = PMext.MediumId;
			Medium colliderMediumExt = ReactionEngine.getMediumFromId (colliderMediumIdExt, _RE.getMediumList ());
			if (colliderMediumExt == null) {
				Debug.Log ("The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	
			if (PMext == null)
				return;
			Medium medium = ReactionEngine.getMediumFromId (MediumId, _RE.getMediumList ());
			if (medium == null) {
				Debug.Log ("The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	    
			float surface = Math.Min (Size, PMext.Size);
			Fick fick = _RE.getFick ();
			FickReaction reaction = Fick.getFickReactionFromIds (colliderMediumIdExt, MediumId, fick.getFickReactions ());
			if (reaction == null) {
				Debug.Log ("This FickReaction does not exist.");
	        
			}
			reaction.setSurface (surface);
		}
	}

	public void OnTriggerExit (Collider collider)
	{
		if(triggerFick){
			PhysicalMedium PMext = collider.gameObject.GetComponent<PhysicalMedium> ();
			if (PMext == null)
				return;
			int colliderMediumIdExt = PMext.MediumId;
			Medium colliderMediumExt = ReactionEngine.getMediumFromId (colliderMediumIdExt, _RE.getMediumList ());
			if (colliderMediumExt == null) {
				Debug.Log ("The collided medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	
			if (PMext == null)
				return;
			Medium medium = ReactionEngine.getMediumFromId (MediumId, _RE.getMediumList ());
			if (medium == null) {
				Debug.Log ("The medium does not exist in the reaction Engine. Load it or change the MediumId number in the PhysicalMedium script.");
				return ;
			}
	    
			Fick fick = _RE.getFick ();
			FickReaction reaction = Fick.getFickReactionFromIds (colliderMediumIdExt, MediumId, fick.getFickReactions ());
			if (reaction == null) {
				Debug.Log ("This FickReaction does not exist.");
	        
			}
			reaction.setSurface (0);
		}
	}
	
}
