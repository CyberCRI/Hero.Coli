using UnityEngine;

public class AvailableDisplayedBioBrick : DisplayedBioBrick {
    
    [SerializeField]
    protected UISprite _noneLeftSprite;
    [SerializeField]
    private UILocalize _noneLeftLabel;
    [SerializeField]
    private UILabel _amountLabel;
    [SerializeField]
    private GameObject _amountBackground;

    private static CraftZoneManager       _craftZoneManager;

  /*TODO
   *automatically choose name:
   *    1 if Device already exists: name it like real one
   *    2 else name it with methodology
   *    3 else name it with fun name or device0/1/2/3/4...
   *if player creates already existing device
   *    select this already existing device
   */

  protected const string _prefabURIAvailable = "GUI/screen3/BioBricks/AvailableDisplayedBioBrickPrefab";

  public static AvailableDisplayedBioBrick Create(
   Transform parentTransform,
   Vector3 localPosition,
   string spriteName,
   BioBrick biobrick
   )
  {
    string nullSpriteName = (spriteName!=null)?"":"(null)";
    Object prefab = Resources.Load(_prefabURIAvailable);
    if(_craftZoneManager == null) {
      _craftZoneManager = CraftZoneManager.get();
    }

    // Debug.Log("AvailableDisplayedBioBrick::Create(parentTransform="+parentTransform
    //   + ", localPosition="+localPosition
    //   + ", spriteName="+spriteName+nullSpriteName
    //   + ", biobrick="+biobrick
    //   );

    AvailableDisplayedBioBrick result = (AvailableDisplayedBioBrick)DisplayedBioBrick.Create(
      parentTransform
      ,localPosition
      ,spriteName
      ,biobrick
      ,prefab
      );
      
      result.name = "AvailableDisplayed"+biobrick.getName();

      result.Initialize();

    return result;
 }
    
public void Initialize()
{
      if(BioBrick.isUnlimited)
      {
          Destroy(_amountLabel.gameObject);
          Destroy(_amountBackground);
      }
}
 
 public void Update()
 {
    if(_amountLabel != null)
        _amountLabel.text = _biobrick.amount.ToString();
     setNoneLeftIndicators(0 >= _biobrick.amount);
 }
 
 private void setNoneLeftIndicators(bool isActive)
 {
     _noneLeftSprite.gameObject.SetActive(isActive);
     _noneLeftLabel.gameObject.SetActive(isActive);
 }

  public void display(bool enabled) {
    gameObject.SetActive(enabled);
  }

  protected override void setJigsawSprite()
  {
      base.setJigsawSprite();
      _noneLeftSprite.spriteName = _jigsawSprite.spriteName;
  }

    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            // Debug.Log(this.GetType() + " OnPress of " + _biobrick.getInternalName());
            //Debug.LogError("pressed");
            if (CraftZoneManager.isDeviceEditionOn())
            {
                //Debug.LogError("isDeviceEditionOn");
                if (_craftZoneManager == null)
                {
                    _craftZoneManager = CraftZoneManager.get();
                }
                if (_biobrick.amount > 0)
                {
                    //Debug.LogError("_biobrick.amount > 0");
                    _craftZoneManager.replaceWithBioBrick(_biobrick);
                    RedMetricsManager.get ().sendEvent(TrackingEvent.ADD, new CustomData(CustomDataTag.BIOBRICK, _biobrick.getInternalName()));
                }
                else
                {
                    //Debug.LogError("!_biobrick.amount > 0");
                }
            }
            else
            {
                //Debug.LogError("!isDeviceEditionOn");
            }
        }
    }

    public static void clear()
    {
        _craftZoneManager = null;
    }
}
