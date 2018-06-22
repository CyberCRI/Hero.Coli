using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Scriptable
{
    
    
    public class CodingSequenceData : BricksData
    {
        [Header("Coding Sequence Properties")]
        [SerializeField] public DeviceAction actionVerb;

        public override void GenerateDescriptionElements()
        {
            
        }

        protected override void CleanBrickProperties()
        {
            actionVerb = DeviceAction.NONE;
        }

    }
}
