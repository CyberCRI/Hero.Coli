using UnityEngine;
using System.Collections;

public class SavedCell : MonoBehaviour {

  private Rigidbody _rigidbody;
  private CapsuleCollider _cellCollider;
  private float _waitAnimationSpeed = 0.1f;

  private Hashtable _optionsDuplicatedAlpha = iTween.Hash(
      "alpha", 0.4f,
      "time", 1.0f,
      "easetype", iTween.EaseType.easeInQuint
      );

    public void initialize (Hero playableCell, Vector3 checkpointPosition)
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

        resetCollisionState();
        
        SwimAnimator newCellSwimAnimator = (SwimAnimator)GetComponent<SwimAnimator>();
        newCellSwimAnimator.setSpeed(_waitAnimationSpeed);
        
        transform.position = playableCell.transform.position;
        transform.localScale = playableCell.transform.localScale;

        //TODO set slow animation
        //TODO change appearance to make it different from playable bacterium: maybe remove eyes?
        //TODO put animation when bacterium becomes playable, then divide cell
        Hero.safeFadeTo(gameObject, _optionsDuplicatedAlpha);
  }

    private void safeInitCollider()
    {
        if (null == _cellCollider)
        {
            _cellCollider = (CapsuleCollider)GetComponent<CapsuleCollider>();
        }
    }

	public void resetCollisionState()
  {
    safeInitCollider();
    setCollidable(false);
  }

  public void setCollidable(bool collidable)
  {
    safeInitCollider();
    _cellCollider.isTrigger = !collidable;
  }

  void OnTriggerExit(Collider col) {
    if(_cellCollider.isTrigger && (null != col.GetComponent<Hero>()))
    {
      _cellCollider.isTrigger = false;
    }
  }
}
