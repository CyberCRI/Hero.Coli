using UnityEngine;
using System.Collections;

public class AvailableDisplayedBioBrick : DisplayedBioBrick {

  public static new string              prefabURI = "GUI/screen3/BioBricks/AvailableDisplayedBioBrickPrefab";
  public static new UnityEngine.Object  prefab    = null;
  public static CraftZoneManager        craftZoneManager = null;

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

    Debug.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
      + ", localPosition="+localPosition
      + ", spriteName="+spriteName+nullSpriteName
      + ", biobrick="+biobrick
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
    Logger.Log("AvailableDisplayedBioBrick::OnPress _id="+_id);
    craftZoneManager.replaceWithBioBrick(_biobrick);
  }
}
