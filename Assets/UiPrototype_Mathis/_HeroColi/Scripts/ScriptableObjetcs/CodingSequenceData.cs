using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Scriptable
{
    
    
    public class CodingSequenceData : BricksData
    {
        [Header("Coding Sequence Properties")]
        [SerializeField] private DeviceAction _actionVerb;
        public DeviceAction ActionVerb
        {
            get { return _actionVerb; }
            set
            {
                _actionVerb = value;

                _state = CheckState() ? DataState.Filled : DataState.Empty;
            }
        }

        public override void GenerateDescriptionElements()
        {
            
        }

        protected override void CleanBrickProperties()
        {
            base.CleanBrickProperties();
            _actionVerb = DeviceAction.NONE;
        }

        protected override bool CheckState()
        {
            if (base.CheckState() && _actionVerb != DeviceAction.NONE)
                return true;
            else
                return false;
        }

    }
}
