using UnityEngine;
using System.Collections;

public class ResettableMine : MonoBehaviour
{
    [SerializeField]
    private TriggeredMineRevealer[] _revealers;
    [SerializeField]
    private bool[] _revelations;
    [SerializeField]
    private Renderer _renderer;
    private const float _fadeTimeS = 0.5f;

    private Hashtable _optionsInAlpha = iTween.Hash(
        "alpha", 1.0f,
        "time", _fadeTimeS,
        "easetype", iTween.EaseType.easeOutExpo
        );

    private Hashtable _optionsOutAlpha = iTween.Hash(
        "alpha", 0.0f,
        "time", _fadeTimeS,
        "easetype", iTween.EaseType.easeInExpo,
        "oncomplete", "onOutComplete"
        );

    void Start()
    {
        // Debug.Log(this.GetType() + " Start " + name);
        updateVisibility();
    }

    void onOutComplete()
    {
        // Debug.Log(this.GetType() + " onOutComplete " + name);
        _renderer.enabled = false;
    }

    public void addRevealer(TriggeredMineRevealer revealer)
    {
        // Debug.Log(this.GetType() + " addRevealer " + revealer.name + " to " + name);
        if (null == _revealers)
        {
            _revelations = new bool[1];
            _revealers = new TriggeredMineRevealer[] { revealer };
        }
        else
        {
            bool[] newRevelations = new bool[_revealers.Length + 1];
            TriggeredMineRevealer[] newRevealers = new TriggeredMineRevealer[_revealers.Length + 1];
            for (int index = 0; index < _revealers.Length; index++)
            {
                newRevealers[index] = _revealers[index];
                newRevelations[index] = _revelations[index];
            }
            newRevealers[_revealers.Length] = revealer;
            _revealers = newRevealers;
            _revelations = newRevelations;
        }
    }

    // called from the old mine to set the new mine
    public void transferParameters(ResettableMine mine)
    {
        // Debug.Log(this.GetType() + " transferParameters of " + name);
        if (null != _revealers && null != mine)
        {
            TriggeredMineRevealer revealer;
            for(int index = 0; index < _revealers.Length; index++)
            {
                revealer = _revealers[index];
                revealer.replace(this, mine);
                mine.addRevealer(revealer);
                if (_revelations[index])
                {
                    // Debug.Log(this.GetType() + " transferParameters revelation true of " + revealer.name + " on " + name);
                    mine.reveal(revealer, true);
                }
            }
        }
    }

    public void reveal(TriggeredMineRevealer tRevealer, bool enable)
    {
        // Debug.Log(this.GetType() + " reveal " + name + " " + enable);
        bool found = false;
        if (null != _revealers && null != _revelations)
        {
            for (int index = 0; index < _revealers.Length; index++)
            {
                if (_revealers[index] == tRevealer)
                {
                    _revelations[index] = enable;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning(this.GetType() + " could not find revealer " + tRevealer.gameObject.name);
            }
        }
        updateVisibility();
    }

    public void updateVisibility()
    {
        // Debug.Log(this.GetType() + " updateVisibility of " + name);

        if (null != _revelations)
        {
            bool visible = false;
            foreach (bool revelation in _revelations)
            {
                if (revelation)
                {
                    // Debug.Log(this.GetType() + " updateVisibility found true");
                    visible = true;
                    break;
                }
            }

            if (visible)
            {
                _renderer.enabled = true;
            }
            Hashtable fadeOptions = visible ? _optionsInAlpha : _optionsOutAlpha;
            iTween.FadeTo(gameObject, fadeOptions);
        }
    }
}