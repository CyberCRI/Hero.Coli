using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PickableItem : MonoBehaviour
{
    protected DNABit _dnaBit;
    public GameObject toDestroy;
    
    protected abstract DNABit produceDNABit ();
    protected abstract void addTo ();
    
    void Awake ()
    {
        _dnaBit = produceDNABit ();
    }

    public DNABit getDNABit ()
    {
        if (null == _dnaBit) {
            Logger.Log ("PickableItem::getDNABit() - null == _dnaBit => produceDNABit", Logger.Level.DEBUG);
            _dnaBit = produceDNABit ();
        }
        return _dnaBit;
    }

    public void pickUp ()
    {
        Logger.Log ("PickableItem::pickUp ()", Logger.Level.DEBUG);
        List<IPickable> pickables;
        Tools.GetInterfaces<IPickable> (out pickables, gameObject);
        foreach (IPickable pickable in pickables) {
            pickable.OnPickedUp ();
        }
    
        addTo ();
        if (toDestroy) {
            Destroy (toDestroy);
        } else {
            Destroy (gameObject);
        }
    }
}
