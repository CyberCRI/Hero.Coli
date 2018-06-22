using UIProto.Scriptable.Enums;
using UnityEngine;

namespace UIProto.Scriptable
{
    [System.Serializable]
    public class DisplayDevice
    {
        #region Events
        public delegate void OnBrickChangeEventHandler(BricksData brick);
        public OnBrickChangeEventHandler OnBrickChange;

        public delegate void OnDeviceCompletedEventHandler();
        public OnDeviceCompletedEventHandler OnDeviceCompleted;

        public delegate DeviceDisplayData CompareWithStoredDeviceEventHandler(DisplayDevice deviceToCompare);
        public CompareWithStoredDeviceEventHandler OnCompareWithStoredDevice;

        public delegate void OnStoreNewDeviceEventHandler(DeviceDisplayData creationData);
        public OnStoreNewDeviceEventHandler OnStoreNewDevice;
        #endregion

        #region Variables
        [Header("Main Properties")]
        public Sprite symbol;
        public string name;
        public string deviceDescription;

        public int craftManagerIndex;

        private DeviceState _state = DeviceState.Empty;
        public DeviceState State
        {
            get { return _state; }
        }
        #endregion

        #region Bricks
        private PromoterData _promoter;
        public PromoterData Promoter
        {
            get { return _promoter; }
            set
            {
                _promoter = value;

                CheckDeviceState();

                if (OnBrickChange != null)
                    OnBrickChange(_promoter);   
            }
        }

        private RBSData _RBS;
        public RBSData RBS
        {
            get { return _RBS; }
            set
            {
                _RBS = value;

                CheckDeviceState();

                if (OnBrickChange != null)
                    OnBrickChange(_RBS);
            }
        }

        private CodingSequenceData _codingSequence;
        public CodingSequenceData CodingSequence
        {
            get { return _codingSequence; }
            set
            {
                _codingSequence = value;
                

                CheckDeviceState();

                if (OnBrickChange != null)
                    OnBrickChange(_codingSequence);
            }
        }

        private TerminatorData _terminator;
        public TerminatorData Terminator
        {
            get { return _terminator; }
            set
            {
                _terminator = value;

                CheckDeviceState();

                if (OnBrickChange != null)
                    OnBrickChange(_terminator);
            }
        }
        #endregion

        #region Device Methods
        private void CheckDeviceState ()
        {
            bool promoterEmpty = _promoter == null;
            bool RBSEmpty = _RBS == null;
            bool codingSequenceEmpty = _codingSequence == null;
            bool terminatorEmpty = _terminator == null;

            if (promoterEmpty && RBSEmpty && codingSequenceEmpty && terminatorEmpty)
                _state = DeviceState.Empty;
            else
            {
                if (!promoterEmpty && !RBSEmpty && !codingSequenceEmpty && !terminatorEmpty)
                {
                    DeviceDisplayData comparisonResult = null;
                    DeviceDisplayData data = null;

                    if (OnCompareWithStoredDevice != null)
                        comparisonResult = OnCompareWithStoredDevice(this);

                    _state = DeviceState.Completed;

                    if (comparisonResult)
                    {
                        // FEEDBACK : device already in datebase

                        data = comparisonResult;
                    }
                    else
                    {
                        Finalyze();

                        data = GetData();

                        if (OnStoreNewDevice != null)
                            OnStoreNewDevice(data);
                    }

                    symbol = data.symbol;
                    name = data.name;
                    deviceDescription = data.deviceDescription;

                    if (OnDeviceCompleted != null)
                        OnDeviceCompleted();
                } 
                else
                    _state = DeviceState.InCompletion;
            }
        }

        public void Initialyze ()
        {
            _promoter = ScriptableObject.CreateInstance<PromoterData>();
            _promoter.type = BricksType.Promoter;
            _promoter.State = DataState.Empty;

            _RBS = ScriptableObject.CreateInstance<RBSData>();
            _RBS.type = BricksType.RBS;
            _RBS.State = DataState.Empty;

            _codingSequence = ScriptableObject.CreateInstance<CodingSequenceData>();
            _codingSequence.type = BricksType.CodingSequence;
            _codingSequence.State = DataState.Empty;

            _terminator = ScriptableObject.CreateInstance<TerminatorData>();
            _terminator.type = BricksType.Terminator;
            _terminator.State = DataState.Empty;
        }

        private void Finalyze ()
        {
            name = _promoter.name + " : " + _RBS.name + " : " + _codingSequence.name + " : " + _terminator.name;

            deviceDescription = "Allow your bacterium to " + _codingSequence.DeviceDescriptionPart + " " + _promoter.DeviceDescriptionPart;

            deviceDescription = deviceDescription.Replace("%RBS%", _RBS.GetDescriptionPart(_codingSequence.ActionVerb));

            //create symbol
        }
        #endregion

        #region Bricks Methods
        public BricksData GetBrick (BricksType type)
        {
            switch (type)
            {
                case BricksType.Promoter:
                    return Promoter;
                case BricksType.RBS:
                    return RBS; 
                case BricksType.CodingSequence:
                    return CodingSequence;
                case BricksType.Terminator:
                    return Terminator;
                default:
                    throw new System.Exception("No Brick of type : " + type);
            }
        }

        public void SetBrick (BricksType type, BricksData brickToSet)
        {
            switch (type)
            {
                case BricksType.Promoter:
                    Promoter = (PromoterData)brickToSet;
                    break;
                case BricksType.RBS:
                    RBS = (RBSData)brickToSet;
                    break;
                case BricksType.CodingSequence:
                    CodingSequence = (CodingSequenceData)brickToSet;
                    break;
                case BricksType.Terminator:
                    Terminator = (TerminatorData)brickToSet;
                    break;
            }
        }
        #endregion

        #region Data Methods
        public DeviceDisplayData GetData ()
        {
            DeviceDisplayData obj = ScriptableObject.CreateInstance<DeviceDisplayData>();

            obj.symbol = symbol;
            obj.name = name;
            obj.deviceDescription = deviceDescription;
            obj.promoter = _promoter;
            obj.RBS = _RBS;
            obj.codingSequence = _codingSequence;
            obj.terminator = _terminator;

            return obj;
        }

        public void SetFromData (DeviceDisplayData data)
        {
            symbol = data.symbol;
            name = data.name;
            deviceDescription = data.deviceDescription;

            if (data.promoter != null)
                Promoter = data.promoter;

            if (data.RBS != null)
                RBS = data.RBS;

            if (data.codingSequence != null)
                CodingSequence = data.codingSequence;

            if (data.terminator != null)
                Terminator = data.terminator;
        }
        #endregion
    }
}
