using UnityEngine;
using System.Collections;

public class SavedCell : MonoBehaviour {

  private Rigidbody _rigidbody;
  private Hero _playableCell;
  private CapsuleCollider _cellCollider;
  private Vector3 _lastCheckpointPosition;

  private Hashtable _optionsDuplicatedAlpha = iTween.Hash(
      "alpha", 0.7f,
      "time", 1.0f,
      "easetype", iTween.EaseType.easeInQuint
      );

    public void initialize (Hero playableCell, Vector3 checkpointPosition)
    {
        _playableCell = playableCell;
        _lastCheckpointPosition = checkpointPosition;

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

        resetCollisionState();
        
        SwimAnimator newCellSwimAnimator = (SwimAnimator)GetComponent<SwimAnimator>();
        newCellSwimAnimator.setSpeed(0);
        
        transform.position = playableCell.transform.position;
        //transform.position = _lastCheckpointPosition;
        transform.localScale = playableCell.transform.localScale;

        //TODO set slow animation
        //TODO change appearance to make it different from playable bacterium: maybe remove eyes?
        //TODO put animation when bacterium becomes playable, then divide cell
        iTween.FadeTo(gameObject, _optionsDuplicatedAlpha);
  }

	public void resetCollisionState()
  {
    if (null == _cellCollider)
    {
      _cellCollider = (CapsuleCollider)GetComponent<CapsuleCollider>();
    }
    _cellCollider.isTrigger = true;
  }

  void OnTriggerExit(Collider col) {
    if(_cellCollider.isTrigger && (null != col.GetComponent<Hero>()))
    {
      _cellCollider.isTrigger = false;
    }
  }
}
