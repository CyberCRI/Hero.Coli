using UnityEngine;
using System.Collections.Generic;

public abstract class PickableItem : MonoBehaviour
{
    protected DNABit _dnaBit;

    // true => parent; false => self
    [SerializeField]
    private bool _destroyParent;
    
    protected abstract DNABit produceDNABit();
    protected abstract void addTo();

    void Awake()
    {
        _dnaBit = produceDNABit();
    }

    public DNABit getDNABit()
    {
        if (null == _dnaBit)
        {
            // Debug.Log(this.GetType() + " getDNABit() - null == _dnaBit => produceDNABit");
            _dnaBit = produceDNABit();
        }
        return _dnaBit;
    }

    public void pickUp()
    {
        // Debug.Log(this.GetType() + " pickUp ()");
        RedMetricsManager.get().sendEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.DNABIT, getDNABit().getInternalName()));
        List<IPickable> pickables;
        Tools.GetInterfaces<IPickable>(out pickables, gameObject);
        foreach (IPickable pickable in pickables)
        {
            pickable.OnPickedUp();
        }

        addTo();
        if (_destroyParent)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
