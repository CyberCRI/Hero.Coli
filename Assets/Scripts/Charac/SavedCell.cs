using UnityEngine;
using System.Collections;

public class SavedCell : MonoBehaviour {

  [SerializeField]
  private FlagellaSetter _flagellaSetter;

  private Rigidbody _rigidbody;
  private CapsuleCollider _cellCollider;
  private float _waitAnimationSpeed = 0.1f;

  private Hashtable _optionsDuplicatedAlpha = iTween.Hash(
      "alpha", 0.4f,
      "time", 1.0f,
      "easetype", iTween.EaseType.easeInQuint
      );

    public void initialize (Hero playableCell)
    {
        resetCollisionState();
        
        SwimAnimator newCellSwimAnimator = (SwimAnimator)GetComponent<SwimAnimator>();
        newCellSwimAnimator.setSpeed(_waitAnimationSpeed);
        
        // transform.position = playableCell.transform.position;
        transform.localScale = playableCell.transform.localScale;

        _flagellaSetter.setFlagellaCount(playableCell.gameObject.GetComponent<PhenoSpeed>().getFlagellaCount());

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
