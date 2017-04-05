using UnityEngine;
using System.Collections;

public class IconAnimation : MonoBehaviour
{
    public bool _isPlaying = false;
    [SerializeField]
    private UISprite _icon;
    [SerializeField]
    private UILabel _label;
    [SerializeField]
    private Color _baseColor;
    [SerializeField]
    private Color _animationColor;
    private Color _diffColor;
    private float _animationTime = 10f;
    public float _timeFactor = 5;
    public float _amplitude = 0.1f;
    public float _duration = 3f;
    public float _currentlyPlayed = -1f;
    private Vector3 _originalScale;

    // always between 0 and 1
    private float _ratio;

    // Use this for initialization
    void Start()
    {
        _originalScale = transform.localScale;

        _diffColor = _animationColor - _baseColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlaying || _currentlyPlayed >= 0)
        {
            _ratio = (1 + Mathf.Sin(_timeFactor * Time.unscaledTime)) / 2;

            _icon.color = _diffColor * _ratio + _baseColor;
            _label.color = _diffColor * _ratio + _baseColor;

            transform.localScale = _originalScale * (1 + _amplitude * _ratio);

            if (_currentlyPlayed >= 0)
            {
                _currentlyPlayed += Time.unscaledDeltaTime;

                if (_currentlyPlayed > _duration)
                {
                    stop();
                }
            }
        }
    }

    public void playForAWhile(float duration = 0f)
    {
        // Debug.Log(this.GetType() + " playForAWhile");
        if (duration != 0)
        {
            _duration = duration;
        }

        _currentlyPlayed = 0f;
    }

    public void play()
    {
        // Debug.Log(this.GetType() + " play");
        _isPlaying = true;
    }

    public void stop()
    {
        // Debug.Log(this.GetType() + " stop");
        _isPlaying = false;
        _currentlyPlayed = -1f;

        _icon.color = _baseColor;
        _label.color = _baseColor;
        transform.localScale = _originalScale;
    }
}
