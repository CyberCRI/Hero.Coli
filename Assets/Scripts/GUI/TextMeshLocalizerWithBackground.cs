using UnityEngine;

public class TextMeshLocalizerWithBackground : TextMeshLocalizer
{

    public int padding = 10;
    public float factor;
    public float offset;

    private int _maxBGAccesses = 10;
    [SerializeField]
    private UILabel _label;
    [SerializeField]
    private UISprite _bg;
    [SerializeField]
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

    public override void localize()
    {
        base.localize();
    }
}