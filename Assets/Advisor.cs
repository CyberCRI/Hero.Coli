using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Advisor : MonoBehaviour {

    [SerializeField]
    private Text _dynamicText;
    [SerializeField]
    private Transform _positionTop;
    [SerializeField]
    private Transform _positionBottom;
    [SerializeField]
    private GameObject _nanoBot;

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

        _dynamicText.text = text;
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
        _dynamicText.text = text;
    }
}
