using UnityEngine;
using System.Collections;

public class ScaleLabelLengthMatcher : MonoBehaviour {

    private int _padding = 10;
    private int _maxBGAccesses = 10;
  
    private UILabel _label;
    private UISprite _bg;
    private BoxCollider _collider;
    
    public UILabel label {
        get{
            if(null == _label) {
                _label = gameObject.GetComponentInChildren<UILabel>();
            }
            return _label;
        }
    }
    public UISprite bg {
        get {
            if(_maxBGAccesses > 0 && null == _bg) {
                _maxBGAccesses--;
                _bg = gameObject.GetComponentInChildren<UISprite>();
            }
            return _bg;
        }
    }
    public BoxCollider boxCollider {
        get {
            if(null == _collider) {
                _collider = gameObject.GetComponent<BoxCollider>();  
            }
            return _collider;
        }
    }
        
  public float factor;
  public float offset;
    
    // Use this for initialization
  void Start () {        
	}
	
	// Update is called once per frame
	void Update () {

        boxCollider.size = new Vector3(
            label.relativeSize.x*label.transform.localScale.x+_padding, 
            boxCollider.size.y, 
            boxCollider.size.z);

        boxCollider.center = new Vector3(
            boxCollider.size.x*factor+offset+Mathf.Sign(offset)*label.transform.localScale.x, 
            boxCollider.center.y,
            boxCollider.center.z);

        if(null != bg) {
            bg.transform.localScale =  boxCollider.size;  
        }
    }
}
