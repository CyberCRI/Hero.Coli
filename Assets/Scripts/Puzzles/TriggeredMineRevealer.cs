using UnityEngine;

public class TriggeredMineRevealer : TriggeredBehaviour
{
    [SerializeField]
    ResettableMine[] _mines;

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        if (null != _mines)
        {
            foreach(ResettableMine mine in _mines)
            {
                // Debug.Log(this.GetType() + " Start calls addRevealer on " + mine.name);
                mine.addRevealer(this);
            }
        }
    }

    void enableAll(bool enable)
    {
        // Debug.Log(this.GetType() + " enableAll(" + enable + ")");
        if (null != _mines)
        {
            foreach (ResettableMine mine in _mines)
            {
                // Debug.Log(this.GetType() + " enableAll reveals " + mine.name);
                mine.reveal(this, enable);
            }
        }
    }

    public override void triggerStart()
    {
        // Debug.Log(this.GetType() + " triggerStart");
        enableAll(true);
    }

    public override void triggerStay()
    {
    }

    public override void triggerExit()
    {
        // Debug.Log(this.GetType() + " triggerExit");
        enableAll(false);
    }

    public void replace(ResettableMine oldMine, ResettableMine newMine)
    {
        // Debug.Log(this.GetType() + " replace(" + oldMine.name + ", " + oldMine.name + ")");

        bool replaced = false;
        if (null != _mines)
        {
            for (int index = 0; index < _mines.Length; index++)
            {
                if (oldMine == _mines[index])
                {
                    // Debug.Log(this.GetType() + " replace replaces " + oldMine.name + " by " + newMine.name);
                    _mines[index] = newMine;
                    replaced = true;
                    break;
                }
            }
        }
        if(!replaced)
        {
            Debug.LogWarning(this.GetType() + " could not find " + oldMine.name + " in mines");
        }
    }
}