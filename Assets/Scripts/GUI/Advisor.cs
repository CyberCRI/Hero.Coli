using UnityEngine;

public class Advisor : MonoBehaviour {
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
    [SerializeField]
    private UILocalize _localize;

    void Start()
    {
        _originalScale = _nanoBot.transform.localScale;
    }

    public void setUpNanoBot(bool top, string text, bool showButton = false)
    {
        if (top)
        {
            _nanoBot.transform.position = _positionTop.position;
        }
        else
        {
            _nanoBot.transform.position = _positionBottom.position;
        }

        _localize.key = text;
        _localize.Localize();
        _nextButton.SetActive(showButton);
    }
}
