using UnityEngine;

public class Advisor : MonoBehaviour {

    [SerializeField]
    private UILabel _dynamicText;
    [SerializeField]
    private Transform _positionTop;
    [SerializeField]
    private Transform _positionBottom;
    [SerializeField]
    private GameObject _nanoBot;
    [SerializeField]
    private Vector3 _originalScale;
    [SerializeField]
    private GameObject _nextButton;
    
    private UILocalize _localize;

    void Start()
    {
        _originalScale = _nanoBot.transform.localScale;
    }

    public void setUpNanoBot(bool top, string text, bool showButton = false)
    {
        if (top == true)
        {
            _nanoBot.transform.position = _positionTop.position;
        }
        if (top == false)
        {
            _nanoBot.transform.position = _positionBottom.position;
        }

        getDynamicText().key = text;
        _nextButton.SetActive(showButton);
    }

    public void setUpNanoBot(bool top)
    {
        if (top == true)
        {
            _nanoBot.transform.position = _positionTop.position;
        }
        if (top == false)
        {
            _nanoBot.transform.position = _positionBottom.position;
        }
    }

    public void setUpNanoBot(string text)
    {
        getDynamicText().key = text;
    }

    public void setUpNanoBot(Vector3 position)
    {
        _nanoBot.transform.position = position;
    }

    public void setUpNanoBot(float scale)
    {
        _nanoBot.transform.localScale = _originalScale * scale;
    }

    public UILocalize getDynamicText()
    {
        if(null == _localize)
        {
            _localize = _dynamicText.GetComponent<UILocalize>();
        }
        return _localize;
    }
}
