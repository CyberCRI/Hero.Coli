using UnityEngine;
using System.Collections;

public class CraftZoneDisplayedBioBrick : DisplayedBioBrick {

  protected static string _prefabURICraftZone = "GUI/screen3/BioBricks/CraftZoneDisplayedBioBrickPrefab";

  public static CraftZoneDisplayedBioBrick Create(
   Transform parentTransform,
   Vector3 localPosition,
   string spriteName,
   BioBrick biobrick
   )
  {
    string nullSpriteName = (spriteName!=null)?"":"(null)";
    Object prefab = Resources.Load(_prefabURICraftZone);

    Logger.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
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
    CraftZoneManager.get ().removeBioBrick(_biobrick);
    TooltipManager.tooltip();
  }

}
