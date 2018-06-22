using UnityEngine;

namespace UIProto.Scriptable
{
    [CreateAssetMenu(fileName = "new terminator", menuName = "HeroColi/Bricks/Terminator", order = 4)]
    public class TerminatorData : BricksData
    {
        public override void GenerateDescriptionElements()
        {
            _brickDescription = "Needed to complete a device";

            _deviceDescriptionPart = "";
        }
    }
}
