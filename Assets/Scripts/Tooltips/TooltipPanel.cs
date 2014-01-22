using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
  public UISprite backgroundSprite;
  public UILabel titleLabel;
  public UILabel typeLabel;
  public UILabel subtitleLabel;
  public UISprite illustrationSprite;
  public UILabel lengthValue;
  public UILabel energyConsumptionValue;
  public UILabel referenceValue;
  public UILabel explanationLabel;

  public override string ToString ()
  {
    return string.Format ("[TooltipPanel "
      +"backgroundSprite="+backgroundSprite
      +", titleLabel="+titleLabel
      +", typeLabel="+typeLabel
      +", subtitleLabel="+subtitleLabel
      +", illustrationSprite="+illustrationSprite
      +", lengthValue="+lengthValue
      +", energyConsumptionValue="+energyConsumptionValue
      +", referenceValue="+referenceValue
      +", explanationLabel="+explanationLabel
      +"]");
  }
}

