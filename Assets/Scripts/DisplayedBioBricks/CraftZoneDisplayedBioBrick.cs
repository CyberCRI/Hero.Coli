using UnityEngine;


public class CraftZoneDisplayedBioBrick : DisplayedBioBrick {

  protected static string _prefabURICraftZone = "GUI/screen3/BioBricks/CraftZoneDisplayedBioBrickPrefab";
  private CraftDeviceSlot _slot;
  public CraftDeviceSlot slot
  {
      set { _slot = value; }
  }

  public static CraftZoneDisplayedBioBrick Create(
   Transform parentTransform,
   Vector3 localPosition,
   string spriteName,
   BioBrick biobrick
   )
  {
    string nullSpriteName = (spriteName!=null)?"":"(null)";
    Object prefab = Resources.Load(_prefabURICraftZone);

    Logger.Log("CraftZoneDisplayedBioBrick::Create(parentTransform="+parentTransform
      + ", localPosition="+localPosition
      + ", spriteName="+spriteName+nullSpriteName
      + ", biobrick="+biobrick
      , Logger.Level.DEBUG
      );

    CraftZoneDisplayedBioBrick result = (CraftZoneDisplayedBioBrick)DisplayedBioBrick.Create(
      parentTransform
      ,localPosition
      ,spriteName
      ,biobrick
      ,prefab
      );

    return result;
 }

  protected override void OnPress(bool isPressed) {
    Logger.Log("CraftZoneDisplayedBioBrick::OnPress _id="+_id+", isPressed="+isPressed, Logger.Level.INFO);
    //Debug.LogError("CraftZoneDisplayedBioBrick::OnPress with CraftZoneManager.isDeviceEditionOn()="+CraftZoneManager.isDeviceEditionOn());
    if(CraftZoneManager.isDeviceEditionOn())
    {
        LimitedBiobricksCraftZoneManager lbczm = (LimitedBiobricksCraftZoneManager) CraftZoneManager.get ();
        if(null != lbczm)
        {
            if(null != _slot)
            {
                _slot.removeBrick(this);
            }
            else
            {
                lbczm.removeBioBrick(this);
            }
        }
        TooltipManager.displayTooltip();
    }
  }

}
