using UnityEngine;

public class Advisor : MonoBehaviour
{
    [SerializeField]
    private UIAnchor _nanoBotAnchor;
    [SerializeField]
    private GameObject _nextButton;
    [SerializeField]
    private UILocalize _localize;
    [SerializeField]
    private float _topAnchorValue;
    [SerializeField]
    private float _bottomAnchorValue;
    [SerializeField]
    private float _highlightedAreaRatio;
    [SerializeField]
    private float _nanobotAreaRatio;
    [SerializeField]
    private float minX = -0.31f, minY = -0.23f, maxX = 0.37f, maxY = 0.2f;

    public void setUpNanoBot(Vector2 position, string text, float scaleFactor, bool showButton = false)
    {
        float offset = scaleFactor * _highlightedAreaRatio + _nanobotAreaRatio;
        _nanoBotAnchor.relativeOffset.x = position.x - Mathf.Sign(position.x) * offset;
        _nanoBotAnchor.relativeOffset.x = _nanoBotAnchor.relativeOffset.x > 0 ? Mathf.Min(_nanoBotAnchor.relativeOffset.x, maxX) : Mathf.Max(_nanoBotAnchor.relativeOffset.x, minX);
        _nanoBotAnchor.relativeOffset.y = position.y - Mathf.Sign(position.y) * offset;
        _nanoBotAnchor.relativeOffset.y = _nanoBotAnchor.relativeOffset.y > 0 ? Mathf.Min(_nanoBotAnchor.relativeOffset.y, maxY) : Mathf.Max(_nanoBotAnchor.relativeOffset.y, minY);
        _localize.key = text;
        _localize.Localize();
        _nextButton.SetActive(showButton);
    }
}
