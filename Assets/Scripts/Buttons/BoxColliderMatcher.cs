using UnityEngine;

public class BoxColliderMatcher : MonoBehaviour
{
    public int padding = 10;
    public float factor;
    public float offset;

    private int _maxBGAccesses = 10;
    [SerializeField]
    private UILabel _label;
    private UISprite _bg;
    private BoxCollider _collider;

    public UILabel label
    {
        get
        {
            if (null == _label)
            {
                _label = gameObject.GetComponentInChildren<UILabel>();
            }
            return _label;
        }
    }
    public UISprite bg
    {
        get
        {
            if (null == _bg && _maxBGAccesses > 0)
            {
                _maxBGAccesses--;
                _bg = gameObject.GetComponentInChildren<UISprite>();
            }
            return _bg;
        }
    }
    public BoxCollider boxCollider
    {
        get
        {
            if (null == _collider)
            {
                _collider = gameObject.GetComponent<BoxCollider>();
            }
            return _collider;
        }
    }

    protected virtual void specificUpdate()
    {
    }


    protected void goLocalCenterSpecificUpdate()
    {
        boxCollider.center = gameObject.transform.localPosition;
    }

    protected void bgLocalCenterSpecificUpdate()
    {
        if (null != bg) boxCollider.center = bg.transform.localPosition;
    }


    protected void boxColliderComputedSpecificUpdate()
    {
        boxCollider.center = new Vector3(
            boxCollider.size.x * factor + offset + Mathf.Sign(offset) * label.transform.localScale.x,
            boxCollider.center.y,
            boxCollider.center.z);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        boxCollider.size = new Vector3(
            label.relativeSize.x * label.transform.localScale.x + padding,
            boxCollider.size.y,
            boxCollider.size.z);

        specificUpdate();

        if (null != bg)
        {
            bg.transform.localScale = boxCollider.size;
        }
    }
}
