using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Scriptable
{
    [CreateAssetMenu(fileName = "new promoter", menuName = "HeroColi/Bricks/Promoter", order = 1)]
    public class PromoterData : BricksData
    {
        [Header("Promoter Properties")]

        [SerializeField] private PromoterMedium _activationMedium;
        public PromoterMedium ActivationMedium
        {
            get { return _activationMedium; }
        }
        [SerializeField][Range(0f, 1f)] private float _efficacity;
        public float Efficacity
        {
            get { return _efficacity; }
            private set
            {
                if (value >= 0 && value <= 1)
                    _efficacity = value;
                else
                    throw new System.Exception("Trying to set Promoter Efficacity with a wrong value");

                _state = CheckState() ? DataState.Filled : DataState.Empty;
            }
        }

        public override void GenerateDescriptionElements()
        {
            _brickDescription = "A promoter which work in " + _activationMedium.ToString().ToLower() + " environment with an efficacity of " + _efficacity + ".";

            _deviceDescriptionPart = "in " + _activationMedium.ToString().ToLower() + " environment";
        }

        protected override void CleanBrickProperties()
        {
            base.CleanBrickProperties();

            _efficacity = 0;
            _activationMedium = PromoterMedium.NONE;
        }

        protected override bool CheckState()
        {
            if (base.CheckState() && _efficacity != 0)
                return true;
            else
                return false;
        }
    }
}
