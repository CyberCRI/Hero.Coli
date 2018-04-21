using UnityEngine;
using System.Collections;

public class SavedCell : CellAnimator {
  private Rigidbody _rigidbody;
  private CapsuleCollider _cellCollider;
  private float _waitAnimationSpeed = 0.1f;

  private Hashtable _optionsDuplicatedAlpha = iTween.Hash(
      "alpha", 0.4f,
      "time", 1.0f,
      "easetype", iTween.EaseType.easeInQuint
      );

    public void initialize (Character playableCell)
    {
        resetCollisionState();        
        SwimAnimator newCellSwimAnimator = (SwimAnimator)GetComponent<SwimAnimator>();
        newCellSwimAnimator.setSpeed(_waitAnimationSpeed);        
        transform.localScale = playableCell.transform.localScale;
        setFlagellaCount(playableCell.gameObject.GetComponent<PhenoSpeed>().getFlagellaCount());
        safeFadeTo(_optionsDuplicatedAlpha);
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

  public void setEnabledCollider(bool enabled)
  {
    safeInitCollider();
    _cellCollider.enabled = enabled;
  }

  void OnTriggerExit(Collider col) {
    if(_cellCollider.isTrigger && (null != col.GetComponent<Character>()))
    {
      _cellCollider.isTrigger = false;
    }
  }
}
