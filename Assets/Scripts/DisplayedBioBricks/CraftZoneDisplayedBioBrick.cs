using UnityEngine;


public class CraftZoneDisplayedBioBrick : DisplayedBioBrick
{

    protected const string _prefabURICraftZone = uriPrefix + "CraftZoneDisplayedBioBrickPrefab";
    private CraftDeviceSlot _slot;
    public CraftDeviceSlot slot
    {
        set { _slot = value; }
    }

    public static CraftZoneDisplayedBioBrick Create(
     Transform parentTransform,
     Vector3 localPosition,
     BioBrick biobrick
     )
    {
        Object prefab = Resources.Load(_prefabURICraftZone);

        // Debug.Log("CraftZoneDisplayedBioBrick Create(parentTransform="+parentTransform
        //   + ", localPosition="+localPosition
        //   + ", biobrick="+biobrick
        //   );

        CraftZoneDisplayedBioBrick result = (CraftZoneDisplayedBioBrick)DisplayedBioBrick.Create(
          parentTransform
          , localPosition
          , biobrick
          , prefab
          );

        return result;
    }

    public override void OnPress(bool isPressed)
    {
        // Debug.Log(this.GetType() + " OnPress of " + _biobrick.getInternalName());
        //Debug.LogError(this.GetType() + " OnPress with CraftZoneManager.isDeviceEditionOn()="+CraftZoneManager.isDeviceEditionOn());
        if (CraftZoneManager.isDeviceEditionOn())
        {
            CraftZoneManager czm = CraftZoneManager.get();
            if (null != czm)
            {
                if (null != _slot)
                {
                    _slot.removeBrick(this);
                }
                else
                {
                    czm.removeBioBrick(this);
                }
                RedMetricsManager.get().sendRichEvent(TrackingEvent.REMOVE, new CustomData(CustomDataTag.BIOBRICK, _biobrick.getInternalName()));
            }
            TooltipManager.displayTooltip();
        }
    }

}
