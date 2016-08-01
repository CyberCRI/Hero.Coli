using UnityEngine;

public class AvailableDisplayedBioBrick : DisplayedBioBrick {
    
    [SerializeField]
    private UISprite _jigsawSprite;
    [SerializeField]
    private UILocalize _biobrickOverlay;
    
    private const string _jigsawSpriteNamePrefix     = "jigsaw_";
    private const string _promoterJigsawSpriteName   = _jigsawSpriteNamePrefix + "promoter";
    private const string _rbsJigsawSpriteName        = _jigsawSpriteNamePrefix + "rbs";
    private const string _geneJigsawSpriteName       = _jigsawSpriteNamePrefix + "gene";
    private const string _terminatorJigsawSpriteName = _jigsawSpriteNamePrefix + "terminator";

  private static CraftZoneManager       _craftZoneManager;

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
  public GameObject noneLeftSprite, noneLeftLabel;

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
      
      result.name = "AvailableDisplayed"+biobrick.getName();
      result.Initialize();

    return result;
 }
    
    public void Initialize()
    {
        setJigsawSprite();
        setBioBrickOverlay();
        
        Debug.Log("this.transform="+this.transform);
        Debug.Log("this.transform.localScale="+this.transform.localScale);
        
        this.transform.localScale = Vector3.one;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
    }
    
    private void setJigsawSprite()
    {
        if (null != _jigsawSprite)
        {
            string jigsawSpriteName = "";
            switch (_biobrick.getType())
            {
                case BioBrick.Type.PROMOTER:
                    jigsawSpriteName = _promoterJigsawSpriteName;
                    break;
                case BioBrick.Type.RBS:
                    jigsawSpriteName = _rbsJigsawSpriteName;
                    break;
                case BioBrick.Type.GENE:
                    jigsawSpriteName = _geneJigsawSpriteName;
                    break;
                case BioBrick.Type.TERMINATOR:
                    jigsawSpriteName = _terminatorJigsawSpriteName;
                    break;
                default:
                    jigsawSpriteName = _promoterJigsawSpriteName;
                    break;
            }
            _jigsawSprite.spriteName = jigsawSpriteName;
        }
    }
    
    private void setBioBrickOverlay()
    {
        DisplayedDevice.setMoleculeOverlay(this._biobrick.getInternalName(), _biobrickOverlay, true);
    }
 
 public void Update()
 {
     amount.text = _biobrick.amount.ToString();
     setNoneLeftIndicators(0 >= _biobrick.amount);
 }
 
 private void setNoneLeftIndicators(bool isActive)
 {
     noneLeftSprite.SetActive(isActive);
     noneLeftLabel.SetActive(isActive);
 }

  public void display(bool enabled) {
    gameObject.SetActive(enabled);
  }

    public override void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            Logger.Log("AvailableDisplayedBioBrick::OnPress _id=" + _id, Logger.Level.INFO);
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
}
