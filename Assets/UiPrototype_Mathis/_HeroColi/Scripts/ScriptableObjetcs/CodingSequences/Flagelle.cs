///-----------------------------------------------------------------
///   Author : #DEVELOPER_NAME#
///   Date   : #DATE#
///-----------------------------------------------------------------

using UnityEngine;

namespace UIProto.Scriptable.Flagelle
{
    [CreateAssetMenu(fileName = "new coding sequence", menuName = "HeroColi/Bricks/CodingSequence/Flagelle", order = 2)]
    public class Flagelle : CodingSequenceData
    {
        public override void GenerateDescriptionElements()
        {
            _brickDescription = "A CodingSequence which allow your bacterium to " + ActionVerb.ToString().ToLower() + "%RBS%";

            _deviceDescriptionPart = ActionVerb.ToString().ToLower() + " %RBS%";
        }

    }
}
