
public class TooltipInfo
{
    public string _code;
    public string _background;
    public string _title;
    public string _type;
    public TooltipManager.TooltipType _tooltipType;
    public string _subtitle;
    public string _illustration;
    public string _customField;
    public string _customValue;
    public string _length;
    public string _reference;
    public string _energyConsumption;
    public string _explanation;

    //TODO replace "type" of type string by type enum
    public TooltipInfo(
      string code,
      string title,
      TooltipManager.TooltipType tooltipType,
      string subtitle,
      string illustration,
      string customField,
      string customValue,
      string length,
      string reference,
      string energyConsumption,
      string explanation
      )
    {
        _code = code;
        _title = title;
        _tooltipType = tooltipType;
        _subtitle = subtitle;
        _illustration = illustration;
        _customField = customField;
        _customValue = customValue;
        _length = length;
        _reference = reference;
        _energyConsumption = energyConsumption;
        _explanation = explanation;

        switch (tooltipType)
        {
            case TooltipManager.TooltipType.BIOBRICK:
                _type = TooltipLoader.biobrickKey;
                _background = TooltipManager.getBioBrickBackground();
                break;
            case TooltipManager.TooltipType.DEVICE:
                _type = TooltipLoader.deviceKey;
                _background = TooltipManager.getDeviceBackground();
                break;
            default:
                _type = TooltipLoader.biobrickKey;
                _background = TooltipManager.getBioBrickBackground();
                break;
        }
    }

    public override string ToString()
    {
        return "[TooltipInfo " +
          "_code:" + _code +
          ", _background:" + _background +
          ", _title:" + _title +
          ", _type:" + _type +
          ", _subtitle:" + _subtitle +
          ", _illustration:" + _illustration +
          ", _customField:" + _customField +
          ", _customValue:" + _customValue +
          ", _length:" + _length +
          ", _reference:" + _reference +
          ", _energyConsumption:" + _energyConsumption +
          ", _explanation:" + _explanation +
          "]";
    }
}

