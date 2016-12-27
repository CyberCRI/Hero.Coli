using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
  public UISprite backgroundSprite;
  public UILocalize titleLabel;
  public UILocalize typeLabel;
  public UILocalize subtitleLabel;
  public UISprite illustrationSprite;
  public UILocalize customFieldLabel;
  public UILocalize customValueLabel;
  public UILocalize lengthValueLabel;
  public UILocalize energyConsumptionValueLabel;
  public UILocalize referenceValueLabel;
  public UILocalize explanationLabel;

  public override string ToString ()
  {
    return "[TooltipPanel "
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
      +"]";
  }
}

