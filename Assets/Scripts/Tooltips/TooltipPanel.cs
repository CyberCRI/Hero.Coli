using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
    // common tooltip fields
    public UISprite backgroundSprite;
    public UILocalize titleLabel;
    public UILocalize typeLabel;
    public UILocalize subtitleLabel; // absent from new device tooltip
    public UISprite illustrationSprite; // absent from new device tooltip, replaced by 4 bricks
    public UILocalize customFieldLabel;
    public UILocalize customValueLabel;
    public UILocalize lengthValueLabel;
    public UILocalize energyConsumptionValueLabel;
    public UILocalize referenceValueLabel;
    public UILocalize explanationLabel;
    // new generic device tooltip fields
    public CraftZoneDisplayedBioBrick promoter;
    public CraftZoneDisplayedBioBrick rbs;
    public CraftZoneDisplayedBioBrick gene;
    public CraftZoneDisplayedBioBrick terminator;

    public override string ToString()
    {
        return "[TooltipPanel "
          + "backgroundSprite=" + backgroundSprite
          + ", titleLabel=" + titleLabel
          + ", typeLabel=" + typeLabel
          + ", subtitleLabel=" + subtitleLabel
          + ", illustrationSprite=" + illustrationSprite
          + ", customFieldLabel=" + customFieldLabel
          + ", customValueLabel=" + customValueLabel
          + ", lengthValueLabel=" + lengthValueLabel
          + ", energyConsumptionValueLabel=" + energyConsumptionValueLabel
          + ", referenceValueLabel=" + referenceValueLabel
          + ", explanationLabel=" + explanationLabel
          + ", promoter=" + promoter
          + ", rbs=" + rbs
          + ", gene=" + gene
          + ", terminator=" + terminator
          + "]";
    }
}

