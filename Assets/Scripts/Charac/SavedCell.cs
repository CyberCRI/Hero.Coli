using UnityEngine;
using System.Collections;

public class SavedCell : MonoBehaviour {

  private Rigidbody _rigidbody;
  public Hero playableCell;
  private CapsuleCollider cellCollider;
  public Vector3 lastCheckpointPosition;

  private Hashtable _optionsDuplicatedAlpha = iTween.Hash(
      "alpha", 0.7f,
      "time", 1.0f,
      "easetype", iTween.EaseType.easeInQuint
      );

  void Awake ()
    {
        //TODO find systematic way of doing this

        Destroy(GetComponent<Hero>());
        Destroy(GetComponent<CellControl>());
        Destroy(GetComponent<PhysicalMedium>());
        Destroy(GetComponent<PhenoSpeed>());
        Destroy(GetComponent<PhenoLight>());
        Destroy(GetComponent<PhenoToxic>());
        Destroy(GetComponent<PhenoFickContact>());
        Destroy(GetComponent<BlackLight>());
        
        Destroy(GetComponent<Rigidbody>());
        //rigidbody.isKinematic = true;

        
        SwimAnimator newCellSwimAnimator = (SwimAnimator)GetComponent<SwimAnimator>();
        newCellSwimAnimator.setSpeed(0);


        cellCollider.enabled = false;
        
        //transform.position = playableCell.transform.position;
        transform.position = lastCheckpointPosition;
        transform.localScale = playableCell.transform.localScale;

        //TODO set slow animation
        //TODO change appearance to make it different from playable bacterium: maybe remove eyes?
        //TODO put animation when bacterium becomes playable, then divide cell
        iTween.FadeTo(gameObject, _optionsDuplicatedAlpha);

  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerExit(Collider col) {
    if(!cellCollider.enabled && (null != col.GetComponent<Hero>()))
    {
      cellCollider.enabled = true;
    }
  }
}
