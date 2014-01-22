using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
  public UISprite backgroundSprite;
  public UILabel titleLabel;
  public UILabel typeLabel;
  public UILabel subtitleLabel;
  public UISprite illustrationSprite;
  public UILabel customFieldLabel;
  public UILabel customValueLabel;
  public UILabel lengthValueLabel;
  public UILabel energyConsumptionValueLabel;
  public UILabel referenceValueLabel;
  public UILabel explanationLabel;

  public override string ToString ()
  {
    return string.Format ("[TooltipPanel "
      +"backgroundSprite="+backgroundSprite
      +", titleLabel="+titleLabel
      +", typeLabel="+typeLabel
      +", subtitleLabel="+subtitleLabel
      +", illustrationSprite="+illustrationSprite
      +", customFieldLabel="+customFieldLabel
      +", customValueLabel="+customValueLabel
      +", lengthValueLabel="+lengthValueLabel
      +", energyConsumptionValueLabel="+energyConsumptionValueLabel
      +", referenceValueLabel="+referenceValueLabel
      +", explanationLabel="+explanationLabel
      +"]");
  }
}

