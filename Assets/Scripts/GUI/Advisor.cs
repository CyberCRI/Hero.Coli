using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    void Start()
    {
        _originalScale = _nanoBot.transform.localScale;
    }

    public void setUpNanoBot(bool top, string text)
    {
        if (top == true)
        {
            _nanoBot.transform.position = _positionTop.position;
        }
        if (top == false)
        {
            _nanoBot.transform.position = _positionBottom.position;
        }

        GetDynamicText().key = text;
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
        GetDynamicText().key = text;
    }

    public void setUpNanoBot(Vector3 position)
    {
        _nanoBot.transform.position = position;
    }

    public void setUpNanoBot(float scale)
    {
        _nanoBot.transform.localScale = _originalScale * scale;
    }

    public UILocalize GetDynamicText()
    {
        return _dynamicText.GetComponent<UILocalize>();
    }
}
