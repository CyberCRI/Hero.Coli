﻿using UnityEngine;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour, ILocalizable
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "TooltipManager";
    private static TooltipManager _instance;
    public static TooltipManager get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("TooltipManager get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<TooltipManager>();
        }
        return _instance;
    }

    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            loadDataIntoDico(inputFiles, _loadedInfoWindows);
            I18n.register(this);
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
        _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;

    private bool initialize()
    {
        if (!_initialized)
        {
            if ((null != bioBrickTooltipPanel) && (null != deviceTooltipPanel))
            {
                bioBrickTooltipPanel.gameObject.SetActive(false);
                deviceTooltipPanel.gameObject.SetActive(false);
                _initialized = true;
            }
            setExplanationSuffixes();
        }
        return _initialized;
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
        initialize();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public string[] inputFiles;
    public TooltipPanel bioBrickTooltipPanel;
    public TooltipPanel deviceTooltipPanel;
    public GameStateController gameStateController;
    public Camera uiCamera;

    private UIPanel _tooltipPanel;
    private UISprite _backgroundSprite;
    private UILocalize _titleLabel;
    private UILocalize _typeLabel;
    private UILocalize _subtitleLabel;
    private UISprite _illustrationSprite;
    private UILocalize _customFieldLabel;
    private UILocalize _customValueLabel;
    private UILocalize _lengthValueLabel;
    private UILocalize _referenceValueLabel;
    private UILocalize _energyConsumptionValueLabel;
    private UILocalize _explanationLabel;
    private CraftZoneDisplayedBioBrick _promoter;
    private CraftZoneDisplayedBioBrick _rbs;
    private CraftZoneDisplayedBioBrick _gene;
    private CraftZoneDisplayedBioBrick _terminator;
    private static LinkedList<BioBrick> _bricks;
    private Dictionary<string, TooltipInfo> _loadedInfoWindows = new Dictionary<string, TooltipInfo>();

    [SerializeField]
    private string deviceBackground;
    [SerializeField]
    private string bioBrickBackground;

    private const string _bioBrickPrefix = "b_";
    private const string _devicePrefix = "d_";
    private const string _basePairsUnitString = "bp";
    private const string _genericExplanationSuffix = "TOOLTIP.D_GENERICEXPLANATION."; 
    private const string _beginningSuffixKey = _genericExplanationSuffix + "BEGINNING";
    private const string _quantityBASESuffixKey = _genericExplanationSuffix + "QUANTITYBASE";
    private const string _quantityLOWSuffixKey = _genericExplanationSuffix + "QUANTITYLOW"; 
    private const string _quantityMEDSuffixKey = _genericExplanationSuffix + "QUANTITYMED";
    private const string _quantityHighSuffixKey = _genericExplanationSuffix + "QUANTITYHIGH";
    private const string _quantitySuffixKey = _genericExplanationSuffix + "QUANTITY";
    private const string _repressedSuffixKey = _genericExplanationSuffix + "REPRESSED";
    private const string _activatedSuffixKey = _genericExplanationSuffix + "ACTIVATED";
    private const string _andSuffixKey = _genericExplanationSuffix + "AND";
    
    private static string _beginningSuffix = _beginningSuffixKey;
    private static string _quantityBaseSuffix = _quantityBASESuffixKey;
    private static string _quantityLowSuffix = _quantityLOWSuffixKey; 
    private static string _quantityMedSuffix = _quantityMEDSuffixKey; 
    private static string _quantityHighSuffix = _quantityHighSuffixKey;
    private static string _quantitySuffix = _quantitySuffixKey;
    private static string _repressedSuffix = _repressedSuffixKey;
    private static string _activatedSuffix = _activatedSuffixKey;
    private static string _andSuffix = _andSuffixKey;
    private static string _endSuffix = ".";

    public enum Quadrant
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }

    public enum TooltipType
    {
        BIOBRICK,
        DEVICE
    }

    public static string getDeviceBackground()
    {
        return _instance.deviceBackground;
    }
    public static string getBioBrickBackground()
    {
        return _instance.bioBrickBackground;
    }

    private static void setVarsFromTooltipPanel(TooltipType type)
    {
        TooltipPanel panel;

        switch (type)
        {
            case TooltipType.BIOBRICK:
                panel = _instance.bioBrickTooltipPanel;
                break;
            case TooltipType.DEVICE:
                panel = _instance.deviceTooltipPanel;
                break;
            default:
                panel = _instance.bioBrickTooltipPanel;
                break;
        }

        _instance._tooltipPanel = panel.gameObject.GetComponent<UIPanel>();

        _instance._titleLabel = panel.titleLabel;
        _instance._typeLabel = panel.typeLabel;
        _instance._subtitleLabel = panel.subtitleLabel;
        _instance._illustrationSprite = panel.illustrationSprite;
        _instance._customFieldLabel = panel.customFieldLabel;
        _instance._customValueLabel = panel.customValueLabel;
        _instance._lengthValueLabel = panel.lengthValueLabel;
        _instance._referenceValueLabel = panel.referenceValueLabel;
        _instance._energyConsumptionValueLabel = panel.energyConsumptionValueLabel;
        _instance._explanationLabel = panel.explanationLabel;
        _instance._backgroundSprite = panel.backgroundSprite;
        // Debug.Log("setVarsFromTooltipPanel _instance._promoter = " + panel.promoter);
        _instance._promoter = panel.promoter;
        _instance._rbs = panel.rbs;
        _instance._gene = panel.gene;
        _instance._terminator = panel.terminator;
    }

    public static bool displayTooltip()
    {
        if (_instance && _instance._tooltipPanel && _instance._tooltipPanel.gameObject)
        {
            _instance._tooltipPanel.gameObject.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool displayTooltip(bool isOver, Device device, Vector3 pos)
    {
        string code = (null == device) ? null : _devicePrefix + device.getInternalName();
        bool result = displayTooltip(isOver, code, pos, device);
        // set bricks
        _bricks = device.getBioBricks();
        int bricksCount = _bricks == null ? 0 : _bricks.Count;
        if (bricksCount == 4)
        {
            _instance._promoter.Initialize(_bricks.First.Value);
            _bricks.RemoveFirst();
            _instance._rbs.Initialize(_bricks.First.Value);
            _bricks.RemoveFirst();
            _instance._gene.Initialize(_bricks.First.Value);
            _bricks.RemoveFirst();
            _instance._terminator.Initialize(_bricks.First.Value);
            _bricks.RemoveFirst();
        }
        else
        {
            Debug.LogWarning("displayTooltip: incorrect bricks count (" + bricksCount + ")");
        }
        return result;
    }

    public static bool displayTooltip(bool isOver, BioBrick brick, Vector3 pos)
    {
        // Debug.Log("TooltipManager displayTooltip(" + isOver + ", brick=" + brick.getInternalName() + ", " + pos + ")");
        string code = (null == brick) ? null : _bioBrickPrefix + brick.getName();
        return displayTooltip(isOver, code, pos, brick);
    }

    private static bool displayTooltip(bool isOver, string code, Vector3 pos, DNABit dnabit)
    {
        // Debug.Log("TooltipManager displayTooltip(" + isOver + ", code=" + code + ", " + pos + ", " + dnabit + ")");

        if (!isOver || (null == code))
        {
            // Debug.Log("TooltipManager hides tooltip");
            return displayTooltip();
        }
        else
        {
            if (fillInFields(code, dnabit))
            {
                // Debug.Log("TooltipManager fillInFieldsFromCode succeeded");

                _instance._tooltipPanel.gameObject.SetActive(true);

                setPosition(pos);
                return true;
            }
            else
            {
                Debug.LogWarning("TooltipManager displayTooltip(" + code + ") failed");
                return false;
            }
        }
    }

    private static bool fillInFields(string code, DNABit dnabit)
    {
        // Debug.Log("TooltipManager fillInFields(" + code + ", " + dnabit + ")");
        TooltipInfo info = retrieveFromDico(code);
        if (null == info)
        {
            // Debug.Log("null == info");
            if (null != dnabit)
            {
                info = getInfoFromDevice(code, dnabit);
                if (null == info)
                {
                    Debug.LogWarning("TooltipManager null != dnabit && null == info");
                    return false;
                }
                // Debug.Log("added " + info);
                _instance._loadedInfoWindows.Add(code, info);
            }
            else
            {
                if (null == info)
                {
                    Debug.LogWarning("TooltipManager null == dnabit && null == info");
                    return false;
                }
            }
        }
        // else
        // {
        //     // Debug.Log("found " + info);
        // }
        return fillInFieldsFromInfo(info, dnabit);
    }

    private static TooltipInfo getInfoFromDevice(string code, DNABit dnabit)
    {
        // Debug.Log("TooltipManager getInfoFromDevice(" + code + ", " + dnabit + ")");

        // build keys
        // unloaded fields - localization keys constructed from dnabit name are used
        Device device = dnabit as Device;

        if (null == device)
        {
            Debug.LogWarning("TooltipManager getInfoFromDevice: not a Device: " + dnabit);
            return null;
        }

        string stem = TooltipLoader.getKeyRoot(code);

        bool localized1, localized2;

        TooltipInfo info = new TooltipInfo(
            code,
            getTitle(dnabit, stem, out localized1),
            TooltipType.DEVICE,
            TooltipLoader._emptyField,
            TooltipLoader._emptyField,
            TooltipLoader.getCustomField(stem),
            TooltipLoader.getCustomValue(stem),
            getLength(dnabit),
            TooltipLoader.getReferenceKey(stem),
            TooltipLoader.getEnergyConsumptionKey(stem),
            getExplanation(dnabit, stem, out localized2)
        );

        info._localized = localized1 || localized2;

        // Debug.Log("TooltipManager getInfoFromDevice returns " + info);

        return info;
    }

    private static string getTitle(DNABit dnabit, string stem, out bool localized)
    {
        string title = TooltipLoader.getTitle(stem);
        localized = title == TooltipLoader._emptyField; 
        return localized ? dnabit.getTooltipTitleKey() : title;
    }

    private static string getExplanation(DNABit dnabit, string stem, out bool localized)
    {
        string explanation = TooltipLoader.getExplanationKey(stem);
        localized = explanation == TooltipLoader._emptyField;
        return localized ? dnabit.getTooltipExplanation() : explanation;
    }

    private static string getLength(DNABit bit)
    {
        if (null == bit)
        {
            return TooltipLoader._emptyField;
        }
        return bit.getLength().ToString() + _basePairsUnitString;
    }

    public static bool isLocalizedString(string field)
    {
        return (Localization.Localize(field) == field);
    }

    private static bool fillInFieldsFromInfo(TooltipInfo info, DNABit dnabit)
    {
        // Debug.Log("TooltipManager fillInFieldsFromInfo(" + info + ")");
        if (null != info)
        {
            setVarsFromTooltipPanel(info._tooltipType);

            _instance._backgroundSprite.spriteName = info._background;

            // runtime creation of tooltip title when necessary
            if (dnabit != null && TooltipLoader._emptyField == info._title)
            {
                // Debug.Log("TooltipManager fillInFieldsFromInfo dnabit != null && TooltipLoader._emptyField == info._title");
                info._title = dnabit.getTooltipTitleKey();
                info._localized = info._localized || (isLocalizedString(info._title));
                // Debug.Log("TooltipManager " + info._localized + " " + info._title);
            }
            // otherwise rely on translation system
            _instance._titleLabel.key = info._title;

            _instance._typeLabel.key = info._type;

            if (null != _instance._subtitleLabel)
            {
                _instance._subtitleLabel.key = info._subtitle;
            }

            if (null != _instance._illustrationSprite)
            {
                _instance._illustrationSprite.spriteName = info._illustration;
            }

            if ((null != _instance._customFieldLabel) && (null != _instance._customValueLabel))
            {
                _instance._customFieldLabel.key = info._customField;
                _instance._customValueLabel.key = info._customValue;
            }

            // runtime computation of length when necessary
            if (TooltipLoader._emptyField == info._length)
            {
                info._length = getLength(dnabit);
            }
            // otherwise rely on translation system
            _instance._lengthValueLabel.key = info._length;

            // TODO runtime computation of energy consumption
            if (null != _instance._energyConsumptionValueLabel)
            {
                _instance._energyConsumptionValueLabel.key = info._energyConsumption;
            }

            _instance._referenceValueLabel.key = info._reference;

            // TODO real runtime creation of explanations
            if (dnabit != null && TooltipLoader._emptyField == info._explanation)
            {
                // temporary solution: directly re-use biobrick explanation
                // Debug.Log("TooltipManager fillInFieldsFromInfo(" + info + ") dnabit != null && TooltipLoader._emptyField == info._explanation");
                info._explanation = dnabit.getTooltipExplanation();
            }
            // otherwise rely on translation system
            _instance._explanationLabel.key = info._explanation;
            // Debug.Log("TooltipManager fillInFieldsFromInfo(" + info + ") finished successfully");
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private void setExplanationSuffixes()
    {
        _beginningSuffix = Localization.Localize(_beginningSuffixKey);
        _quantityBaseSuffix = Localization.Localize(_quantityBASESuffixKey);
        _quantityLowSuffix = Localization.Localize(_quantityLOWSuffixKey);
        _quantityMedSuffix = Localization.Localize(_quantityMEDSuffixKey);
        _quantitySuffix = Localization.Localize(_quantitySuffixKey);
        _repressedSuffix = Localization.Localize(_repressedSuffixKey);
        _activatedSuffix = Localization.Localize(_activatedSuffixKey);
        _andSuffix = Localization.Localize(_andSuffixKey);
    }

    public static string produceExplanation(Device device)
    {
        // Debug.Log("TooltipManager produceExplanation " + device);
        // "Produces"
        string result = _beginningSuffix + " ";
        
        // " a little"
        string quantitySuffix;
        switch (device.levelIndex)
        {
            case 0:
                quantitySuffix = _quantityBaseSuffix;
                break;
            case 1:
                quantitySuffix = _quantityLowSuffix;
                break;
            case 2:
                quantitySuffix = _quantityMedSuffix;
                break;
            case 3:
                quantitySuffix = _quantityHighSuffix;
                break;
            default:
                quantitySuffix = _quantityBaseSuffix;
                break;
        }
        result += quantitySuffix + " ";

        // " amount of"
        result += _quantitySuffix + " ";

        // " 'X'"
        result += Localization.Localize(GameplayNames.getShortName(device.getFirstGeneProteinName())) + " ";

        // " in presence of 'Y' and absence of 'Z'"
        result += getPromoterRegulationSuffix(device);

        // "."
        result += _endSuffix;

        // Debug.Log("TooltipManager produceExplanation("+device+")=" + result);
        return result;
    }

    private static string getPromoterRegulationSuffix(Device device)
    {
        string result = "";
        LinkedList<BioBrick> bricks = device.getBioBricks();
        if (null != bricks)
        {
            PromoterBrick promoter = bricks.First.Value as PromoterBrick;
            if (promoter != null)
            {
                PromoterBrick.Regulation regulation = promoter.getRegulation();
                switch (regulation)
                {
                    case PromoterBrick.Regulation.CONSTANT:
                        break;
                    case PromoterBrick.Regulation.ACTIVATED:
                        result = getActivatorsString(promoter);
                        break;
                    case PromoterBrick.Regulation.REPRESSED:
                        result = getRepressorsString(promoter);
                        break;
                    case PromoterBrick.Regulation.BOTH:
                        result = getActivatorsString(promoter) + " " + _andSuffix + " " + getRepressorsString(promoter);
                        break;
                    default:
                        break;
                }
            }
        }
        return result;                
    }

    private static string getActivatorsString(PromoterBrick promoter)
    {
        return _activatedSuffix + " " + getLocalizedListOfMolecules(promoter.activators);
    }

    private static string getRepressorsString(PromoterBrick promoter)
    {
        return _repressedSuffix + " " + getLocalizedListOfMolecules(promoter.repressors);
    }

    private static string getLocalizedListOfMolecules(List<string> molecules)
    {
        List<string> localized = new List<string>();
        foreach(string molecule in molecules)
        {
            localized.Add(Localization.Localize(GameplayNames.getBrickNameKey(molecule)));
        }
        return Logger.ToString<string>(localized);
    }

    public static string buildExplanationKeyFromStem(string stem)
    {
        return TooltipLoader.getExplanationKey((TooltipLoader._tooltipPrefix + TooltipManager._bioBrickPrefix + stem).ToUpperInvariant());
    }

    private static TooltipInfo retrieveFromDico(string code)
    {
        // Debug.Log("TooltipManager retrieveFromDico(" + code + ")");
        TooltipInfo info;
        if (!_instance._loadedInfoWindows.TryGetValue(code, out info))
        {
            // Debug.LogWarning("TooltipManager retrieveFromDico(" + code + ") failed");
            info = null;
        }
        return info;
    }

    private void loadDataIntoDico(string[] inputFiles, Dictionary<string, TooltipInfo> dico)
    {
        TooltipLoader tLoader = new TooltipLoader();

        // string loadedFiles = "";

        foreach (string file in inputFiles)
        {
            foreach (TooltipInfo info in tLoader.loadInfoFromFile(file))
            {
                dico.Add(info._code, info);
            }
            // if (!string.IsNullOrEmpty(loadedFiles))
            // {
                // loadedFiles += ", ";
            // }
            // loadedFiles += file;
        }

        // Debug.Log(this.GetType() + " loadDataIntoDico loaded " + loadedFiles);
    }

    private static void setPosition(Vector3 pos)
    {
        Quadrant quadrant = getQuadrant(pos);
        float xShift, yShift;

        float xScale = _instance._backgroundSprite.transform.localScale.x
          , yScale = _instance._backgroundSprite.transform.localScale.y;

        xShift = xScale / 2;
        yShift = yScale / 2;

        switch (quadrant)
        {
            case Quadrant.TOP_LEFT:
                yShift = -yShift;
                break;
            case Quadrant.TOP_RIGHT:
                xShift = -xShift;
                yShift = -yShift;
                break;
            case Quadrant.BOTTOM_LEFT:
                break;
            case Quadrant.BOTTOM_RIGHT:
                xShift = -xShift;
                break;
            default:
                Debug.LogWarning("TooltipManager setPosition default case");
                break;
        }
        _instance._tooltipPanel.transform.position = new Vector3(pos.x, pos.y, _instance._tooltipPanel.transform.position.z);
        _instance._tooltipPanel.transform.localPosition += new Vector3(xShift, yShift, 0f);
    }

    public static Quadrant getQuadrant(Vector3 pos)
    {
        Vector3 screenPos = _instance.uiCamera.WorldToScreenPoint(pos);
        bool top = screenPos.y > _instance.uiCamera.pixelHeight / 2;
        bool left = screenPos.x < _instance.uiCamera.pixelWidth / 2;

        if (top)
        {
            if (left)
                return Quadrant.TOP_LEFT;
            else
                return Quadrant.TOP_RIGHT;
        }
        else
        {
            if (left)
                return Quadrant.BOTTOM_LEFT;
            else
                return Quadrant.BOTTOM_RIGHT;
        }
    }

    public void onLanguageChanged()
    {
        // Debug.Log(this.GetType() + " onLanguageChanged");
        foreach (KeyValuePair<string, TooltipInfo> element in _loadedInfoWindows)
        {
            element.Value.onLanguageChanged();
        }
        setExplanationSuffixes();
    }
}