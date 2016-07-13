using UnityEngine;

public class AvailableDisplayedBioBrick : DisplayedBioBrick {

  private static CraftZoneManager       craftZoneManager;

  /*TODO
   *automatically choose name:
   *    1 if Device already exists: name it like real one
   *    2 else name it with methodology
   *    3 else name it with fun name or device0/1/2/3/4...
   *if player creates already existing device
   *    select this already existing device
   */

  protected static string _prefabURIAvailable = "GUI/screen3/BioBricks/AvailableDisplayedBioBrickPrefab";
  public UILabel amount;
  public GameObject noneLeftMask;

  public static AvailableDisplayedBioBrick Create(
   Transform parentTransform,
   Vector3 localPosition,
   string spriteName,
   BioBrick biobrick
   )
  {
    string nullSpriteName = (spriteName!=null)?"":"(null)";
    Object prefab = Resources.Load(_prefabURIAvailable);
    if(craftZoneManager == null) {
      craftZoneManager = CraftZoneManager.get();
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
 
 public void Update()
 {
     amount.text = _biobrick.amount.ToString();
     noneLeftMask.SetActive(0 >= _biobrick.amount);
 }

  public void display(bool enabled) {
    gameObject.SetActive(enabled);
  }

  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableDisplayedBioBrick::OnPress _id="+_id, Logger.Level.INFO);
      if(CraftZoneManager.isDeviceEditionOn())
      {
        if(craftZoneManager == null) {
            craftZoneManager = CraftZoneManager.get();
        }
        if(_biobrick.amount > 0)
        {
            craftZoneManager.replaceWithBioBrick(_biobrick);
        }
      }
    }
  }
}
