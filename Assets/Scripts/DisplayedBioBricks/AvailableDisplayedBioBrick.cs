using UnityEngine;
using System.Collections;

public class AvailableDisplayedBioBrick : DisplayedBioBrick {

  public static new string              prefabURI = "GUI/screen3/BioBricks/AvailableDisplayedBioBrickPrefab";
  public static new UnityEngine.Object  prefab    = null;
  public static CraftZoneManager        craftZoneManager = null;

  /*TODO
   *automatically choose name:
   *    1 if Device already exists: name it like real one
   *    2 else name it with methodology
   *    3 else name it with fun name or device0/1/2/3/4...
   *if player creates already existing device
   *    select this already existing device
   */


  public static AvailableDisplayedBioBrick Create(
   Transform parentTransform,
   Vector3 localPosition,
   string spriteName,
   BioBrick biobrick
   )
  {
    string nullSpriteName = (spriteName!=null)?"":"(null)";
    if(prefab == null) prefab = Resources.Load(prefabURI);
    if(craftZoneManager == null) {
      craftZoneManager = (CraftZoneManager)GameObject.Find("AssemblyZonePanel").GetComponent<CraftZoneManager>();
    }

    Logger.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
      + ", localPosition="+localPosition
      + ", spriteName="+spriteName+nullSpriteName
      + ", biobrick="+biobrick
      , Logger.Level.DEBUG
      );

    AvailableDisplayedBioBrick result = (AvailableDisplayedBioBrick)DisplayedBioBrick.Create(
      parentTransform
      ,localPosition
      ,spriteName
      ,biobrick
      ,prefab
      );

    return result;
 }

  public void display(bool enabled) {
    gameObject.SetActive(enabled);
  }

  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableDisplayedBioBrick::OnPress _id="+_id, Logger.Level.INFO);
      craftZoneManager.replaceWithBioBrick(_biobrick);
    }
  }
}
