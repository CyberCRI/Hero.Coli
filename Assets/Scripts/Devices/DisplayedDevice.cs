using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedDevice : DisplayedElement
{

    public UISprite levelSprite;
    public UISprite deviceBackgroundSprite;
    public UILocalize moleculeOverlay;
    public ParticleSystem feedbackParticleSystem;
    protected bool _isFeedbackParticleSystemActive = false;

    // prefab URIs
    private const string equipedPrefabURI = "GUI/screen1/Devices/DisplayedDevicePrefab";
    public const string equipmentPrefabURI = "GUI/screen1/Devices/EquipmentDevicePrefab";
    public const string equipedWithMoleculesPrefabURI = "GUI/screen1/Devices/EquipedDisplayedDeviceWithMoleculesPrefab";
    //private const string inventoriedPrefabURI = "GUI/screen1/Devices/InventoriedDeviceButtonPrefab";
    private const string inventoriedPrefabURI = "GUI/screen1/Devices/InventoryDevicePrefab";
    private const string listedPrefabURI = "GUI/screen3/Devices/ListedDevicePrefab";

    // device icon //

    // common
    private const string _separator = "_";
    private const string _baseDeviceTextureString = "device_";
    private const string _quality256 = "256x256_";
    private const string _quality80 = "80x80_";
    private const string _quality64 = "64x64_";
    private const string _qualityDefault = _quality64;

    // _level
    private const float _level0 = .06f;
    private const float _level1 = .126f;
    private const float _level2 = .23f;
    private const float _level3 = .4f;
    private const float _level01Threshold = (_level0 + _level1) / 2;
    private const float _level12Threshold = (_level1 + _level2) / 2;
    private const float _level23Threshold = (_level2 + _level3) / 2;
    private const string _levelBaseSuffix = "_base";
    private const string _levelLowSuffix = "_low";
    private const string _levelMedSuffix = "_med";
    private const string _level0Suffix = "level0";
    private const string _level1Suffix = "level1";
    private const string _level2Suffix = "level2";
    private const string _level3Suffix = "level3";
    private const string _textSuffix = "_text";
    private const string _pictureSuffix = "_picture";
    private const string _level0TextSpriteName = _baseDeviceTextureString + _qualityDefault + _level0Suffix + _textSuffix;
    private const string _level1TextSpriteName = _baseDeviceTextureString + _qualityDefault + _level1Suffix + _textSuffix;
    private const string _level2TextSpriteName = _baseDeviceTextureString + _qualityDefault + _level2Suffix + _textSuffix;
    private const string _level3TextSpriteName = _baseDeviceTextureString + _qualityDefault + _level3Suffix + _textSuffix;
    private const string _level0PictureSpriteName = _baseDeviceTextureString + _qualityDefault + _level0Suffix + _pictureSuffix;
    private const string _level1PictureSpriteName = _baseDeviceTextureString + _qualityDefault + _level1Suffix + _pictureSuffix;
    private const string _level2PictureSpriteName = _baseDeviceTextureString + _qualityDefault + _level2Suffix + _pictureSuffix;
    private const string _level3PictureSpriteName = _baseDeviceTextureString + _qualityDefault + _level3Suffix + _pictureSuffix;

    // default texture
    private const string _defaultTexture            = "default";
    private const string _defaultTextureWithText    = _defaultTexture + _textSuffix;

    // molecule name overlay
    private const string _moleculeOverlayPrefix = "BRICK.ICONLABEL.";

    // background
    private const string backgroundSuffix               = "background_";
    private const string squareBackgroundSuffix         = "square";
    private const string roundedSquareBackgroundSuffix  = "rounded_square";
    private const string circleBackgroundSuffix         = "circle";
    private const string squareBackgroundSpriteName         = _baseDeviceTextureString + _qualityDefault + backgroundSuffix + squareBackgroundSuffix;
    private const string roundedSquareBackgroundSpriteName  = _baseDeviceTextureString + _qualityDefault + backgroundSuffix + roundedSquareBackgroundSuffix;
    private const string circleBackgroundSpriteName         = _baseDeviceTextureString + _qualityDefault + backgroundSuffix + circleBackgroundSuffix;

    private const string _equipedPrefix = "e_";
    private const string _inventoriedPrefix = "i_";
    private const string _listedPrefix = "l_";
    private const string _craftSlotPrefix = "c_"; 
    private string[] _deviceNamePrefixes = new string[4]{_equipedPrefix,_inventoriedPrefix,_listedPrefix,_craftSlotPrefix};

    public Device _device;
    protected static DevicesDisplayer _devicesDisplayer;
    public DevicesDisplayer.DeviceType _deviceType;

    private static Dictionary<string, string> geneSpecialTextureDico = new Dictionary<string, string>()
  {
        {"ARAC", _defaultTextureWithText},
        {"ATC", _defaultTextureWithText},
        {"IPTG", _defaultTextureWithText},
        {"LARA", _defaultTextureWithText},
        {"REPR1", _defaultTextureWithText},
        {"REPR2", _defaultTextureWithText},
        {"REPR3", _defaultTextureWithText},
        {"REPR4", _defaultTextureWithText}
  };

  public static void clear()
  {
      _devicesDisplayer = null;
  }
  
    public void playFeedback()
    {
        _isFeedbackParticleSystemActive = true;
        
        feedbackParticleSystem.gameObject.SetActive(true);
        feedbackParticleSystem.Emit(50);

        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(terminateParticleSystem());
        }
    }
    
    IEnumerator terminateParticleSystem()
    {
        yield return new WaitForSeconds(1.5f);
        
        _isFeedbackParticleSystemActive = false;
        
        feedbackParticleSystem.Stop();
        feedbackParticleSystem.gameObject.SetActive(false);
        
        yield return null;
    }

    private static string getTextureName(string proteinName)
    {
        string texture = _defaultTextureWithText;
        if (!geneSpecialTextureDico.TryGetValue(proteinName, out texture))
        {
            texture = proteinName.ToLowerInvariant();
        }
        return texture;
    }

    private void setBackgroundSprite(string backgroundSpriteName = circleBackgroundSpriteName)
    {
        if (null != deviceBackgroundSprite)
        {
            deviceBackgroundSprite.spriteName = backgroundSpriteName;
            deviceBackgroundSprite.gameObject.SetActive(true);
        }
    }
    
    private void setMoleculeOverlay(string proteinName)
    {
        setMoleculeOverlay(proteinName, moleculeOverlay);
    }

    public static void setMoleculeOverlay(string proteinName, UILocalize moleculeOverlayLocalize, bool dontFilter = false)
    {
        if (null != moleculeOverlayLocalize)
        {
            string texture = _defaultTextureWithText;
            if (dontFilter || geneSpecialTextureDico.TryGetValue(proteinName, out texture))
            {
                moleculeOverlayLocalize.gameObject.SetActive(true);
                moleculeOverlayLocalize.key = _moleculeOverlayPrefix + proteinName.ToUpperInvariant();
            }
            else
            {
                moleculeOverlayLocalize.gameObject.SetActive(false);
            }
        }
    }

    private void setLevelSprite(int levelIndex = -1, bool isPictureMode = true)
    {
        if (levelIndex == -1)
        {
            levelIndex = getLevelIndex();
        }
        if (null != levelSprite)
        {
            string spriteName = getLevelSpriteName(levelIndex, isPictureMode);
            if (!string.IsNullOrEmpty(spriteName))
            {
                levelSprite.spriteName = spriteName;
                levelSprite.gameObject.SetActive(true);
            }
            else
            {
                levelSprite.gameObject.SetActive(false);
            }
        }
    }

    private void setDeviceIcon(int levelIndex = -1)
    {
        if (levelIndex == -1)
        {
            levelIndex = getLevelIndex();
        }

        string withLevelSuffix = getTextureName() + _separator + getLevelSuffix(levelIndex);

        if (_atlas.GetListOfSprites().Contains(withLevelSuffix))
        {
            setSprite(withLevelSuffix);
        }
        else
        {
            setSprite(getTextureName());
        }
    }

    private int getLevelIndex()
    {
        int index = 0;
        if (_device == null)
        {
            index = 0;
        }
        else
        {
            float expressionLevel = _device.getExpressionLevel();

            if (expressionLevel < _level01Threshold)
            {
                index = 0;
            }
            else if (expressionLevel < _level12Threshold)
            {
                index = 1;
            }
            else if (expressionLevel < _level23Threshold)
            {
                index = 2;
            }
            else
            {
                index = 3;
            }
        }
        return index;
    }

    private string getLevelSuffix(int levelIndex = -1)
    {
        string levelSuffix;
        if (levelIndex == -1)
        {
            levelIndex = getLevelIndex();
        }
        switch (levelIndex)
        {
            case 0:
                levelSuffix = _level0Suffix;
                break;
            case 1:
                levelSuffix = _level1Suffix;
                break;
            case 2:
                levelSuffix = _level2Suffix;
                break;
            case 3:
                levelSuffix = _level3Suffix;
                break;
            default:
                levelSuffix = _level0Suffix;
                break;
        }
        return levelSuffix;
    }

    private string getLevelSpriteName(int levelIndex = -1, bool isPictureMode = true)
    {
        string spriteName;

        if (levelIndex == -1)
        {
            levelIndex = getLevelIndex();
        }

        if (_device == null)
        {
            spriteName = "";
        }
        else
        {
            switch (levelIndex)
            {
                case 0:
                    spriteName = isPictureMode ? _level0PictureSpriteName : _level0TextSpriteName;
                    break;
                case 1:
                    spriteName = isPictureMode ? _level1PictureSpriteName : _level1TextSpriteName;
                    break;
                case 2:
                    spriteName = isPictureMode ? _level2PictureSpriteName : _level2TextSpriteName;
                    break;
                case 3:
                    spriteName = isPictureMode ? _level3PictureSpriteName : _level3TextSpriteName;
                    break;
                default:
                    spriteName = isPictureMode ? _level0PictureSpriteName : _level0TextSpriteName;
                    break;
            }
        }
        return spriteName;
    }

    public static DisplayedDevice Create(
            Transform parentTransform
            , Vector3 localPosition
            , string spriteName
            , Device device
            , DevicesDisplayer devicesDisplayer
            , DevicesDisplayer.DeviceType deviceType
        )
    {

        if (device == null)
        {
            Debug.LogWarning("DisplayedDevice Create device==null");
        }

        string nullSpriteName = (spriteName != null) ? "" : "(null)";
        Object prefab;
        if (deviceType == DevicesDisplayer.DeviceType.Equiped)
        {
            // Debug.Log("DisplayedDevice: will create Equiped " + equipedPrefabURI);
            prefab = Resources.Load(equipedPrefabURI);
        }
        else if (deviceType == DevicesDisplayer.DeviceType.Inventoried)
        {
            // Debug.Log("DisplayedDevice: will create Inventoried " + inventoriedPrefabURI);
            prefab = Resources.Load(inventoriedPrefabURI);
        }
        else if (deviceType == DevicesDisplayer.DeviceType.Listed)
        {
            // Debug.Log("DisplayedDevice: will create Listed " + listedPrefabURI);
            prefab = Resources.Load(listedPrefabURI);
        }
        else
        {
            Debug.LogWarning("DisplayedDevice Create : unmanaged device type " + deviceType);
            return null;
        }

        // Debug.Log("DisplayedDevice Create(type=" + deviceType
        // + ", parentTransform=" + parentTransform
        // + ", localPosition=" + localPosition
        // + ", device=" + device
        // + ", devicesDisplayer=" + devicesDisplayer
        // + ", spriteName=" + spriteName + nullSpriteName
        // );

        DisplayedDevice result = (DisplayedDevice)DisplayedElement.Create(
          parentTransform
          , localPosition
          , ""
          , prefab
          );
          
        result.Initialize(device, devicesDisplayer);

        return result;
    }

    public string getTextureName()
    {
        string usedSpriteName = _baseDeviceTextureString;

        DevicesDisplayer.TextureQuality quality = DevicesDisplayer.getTextureQuality();

        switch (quality)
        {
            case DevicesDisplayer.TextureQuality.HIGH:
                usedSpriteName += _quality256;
                break;
            case DevicesDisplayer.TextureQuality.NORMAL:
                usedSpriteName += _quality80;
                break;
            case DevicesDisplayer.TextureQuality.LOW:
                usedSpriteName += _quality64;
                break;
            default:
                usedSpriteName += _qualityDefault;
                break;
        }

        if (null == _device)
        {
            usedSpriteName += getTextureName("");
        }
        else
        {
            usedSpriteName += getTextureName(_device.getFirstGeneProteinName());
        }


        /*
            if(quality == DevicesDisplayer.TextureQuality.LOW)
              usedSpriteName += getLevelSuffix(device);
        */


        // Debug.Log(this.GetType() + " getTextureName usedSpriteName=" + usedSpriteName);
        return usedSpriteName;
    }

    public void Initialize(
        Device device
      , DevicesDisplayer devicesDisplayer = null
      )
    {

        if (device == null)
        {
            Debug.LogWarning(this.GetType() + " Initialize device==null");
        }

        _device = Device.buildDevice(device);
        if (_device == null)
        {
            Debug.LogWarning(this.GetType() + " Initialize _device==null");
        }
        // Debug.Log(this.GetType() + " Create built device " + _device + " from " + device);
        if (_devicesDisplayer == null)
        {
            _devicesDisplayer = DevicesDisplayer.get();
        }
        
        // Debug.Log(this.GetType() + " Initialize ends");

        setName();
        setBackgroundSprite();
        setMoleculeOverlay(device.getFirstGeneProteinName());
        int levelIndex = getLevelIndex();
        setLevelSprite(levelIndex);
        setDeviceIcon(levelIndex);
    }

    private void setName()
    {
        gameObject.name = _deviceNamePrefixes[(int)_deviceType] + _device.getInternalName();
    }

    protected string getDebugInfos()
    {
        return "device id=" + _id + ", inner device=" + _device + ", device type=" + _deviceType + ", time=" + Time.realtimeSinceStartup;
    }

    public void toggleEquiped()
    {
        if (_device == null)
        {
            Debug.LogWarning(this.GetType() + " toggleEquiped _device==null");
            return;
        }

        DeviceContainer.AddingResult addingResult = _devicesDisplayer.askAddEquipedDevice(_device);
        // Debug.Log(this.GetType() + " toggleEquiped added device result=" + addingResult + ", " + getDebugInfos());
        if (DeviceContainer.AddingResult.FAILURE_SAME_NAME == addingResult
           || DeviceContainer.AddingResult.FAILURE_SAME_DEVICE == addingResult)
        {
            if (_devicesDisplayer.askRemoveEquipedDevice(_device))
            {
                RedMetricsManager.get().sendEvent(TrackingEvent.UNEQUIP, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
            }
        }
        else
        {
            RedMetricsManager.get().sendEvent(TrackingEvent.EQUIP, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
        }
    }
    
    void Update ()
    {
        if((_isFeedbackParticleSystemActive) && (GameStateController.isPause()) && (null != feedbackParticleSystem))
        {
             feedbackParticleSystem.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }

    public override void OnPress(bool isPressed)
    {
        Debug.Log(this.GetType() + " OnPress() of " + _device.getInternalName());
    }

    protected virtual void OnHover(bool isOver)
    {
        // Debug.Log(this.GetType() + " OnHover() of " + _device.getInternalName());
        TooltipManager.displayTooltip(isOver, _device, transform.position);
    }

    //TODO remove tooltip only if tooltip was about this displayed device
    protected virtual void OnDestroy()
    {
        TooltipManager.displayTooltip();
    }
}
