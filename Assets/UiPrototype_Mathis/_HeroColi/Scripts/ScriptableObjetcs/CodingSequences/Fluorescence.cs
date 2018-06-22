///-----------------------------------------------------------------
///   Author : #DEVELOPER_NAME#
///   Date   : #DATE#
///-----------------------------------------------------------------

using UnityEngine;

namespace UIProto.Scriptable.CodingSequences
{
    public enum FluorescenceColors
    {
        RED,
        GREEN
    }

    public enum LightColors
    {
        ORANGE,
        BLUE
    }

    [CreateAssetMenu(fileName = "new coding sequence", menuName = "HeroColi/Bricks/CodingSequence/Fluorescence", order = 1)]
    public class Fluorescence : CodingSequenceData
    {
        [SerializeField] public FluorescenceColors fluorescenceColorName;
        [SerializeField] public Color fluorescenceColor;

        [SerializeField] public LightColors lightColorName;
        [SerializeField] public Color lightColor;

        public override void GenerateDescriptionElements()
        {
            _brickDescription = "A CodingSequence which allow your bacterium to " + actionVerb.ToString().ToLower() + " " + fluorescenceColorName.ToString().ToLower() + " fluorescence while being exposed to " + lightColorName.ToString().ToLower() + " light.";

            _deviceDescriptionPart = actionVerb.ToString().ToLower() + "%RBS% " + fluorescenceColorName.ToString().ToLower() + " fluorescence while being exposed to " + lightColorName.ToString().ToLower() + " light";
        }

    }
}
