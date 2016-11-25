using UnityEngine;

public class ResettableMine : MonoBehaviour
{
    private TriggeredMineRevealer[] _revealers;
    private bool[] _revelations;
    [SerializeField]
    private Renderer _renderer;

    void Start()
    {
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
            _revelations = new bool[_revealers.Length + 1];
            TriggeredMineRevealer[] newRevealers = new TriggeredMineRevealer[_revealers.Length + 1];
            for (int index = 0; index < _revealers.Length; index++)
            {
                newRevealers[index] = _revealers[index];
            }
            newRevealers[_revealers.Length] = revealer;
            _revealers = newRevealers;
        }
    }

    public void replaceInRevealers(ResettableMine mine)
    {
        // Debug.Log(this.GetType() + " replaceInRevealers of " + name);
        if (null != _revealers && null != mine)
        {
            foreach (TriggeredMineRevealer revealer in _revealers)
            {
                revealer.replace(this, mine);
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

            if(!found)
            {
                Debug.LogWarning(this.GetType() + " could not find revealer " + tRevealer.gameObject.name);
            }
        }
        updateVisibility();
    }

    public void updateVisibility()
    {
        // Debug.Log(this.GetType() + " updateVisibility of " + name);

        if(null != _revelations)
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
            _renderer.enabled = visible;
        }
    }
}