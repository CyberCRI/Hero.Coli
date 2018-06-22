///-----------------------------------------------------------------
///   Author : #DEVELOPER_NAME#
///   Date   : #DATE#
///-----------------------------------------------------------------

using UnityEngine;

namespace UIProto.Scriptable
{
    [CreateAssetMenu(fileName = "new device", menuName = "HeroColi/Devices", order = 1)]
    public class DeviceDisplayData : ScriptableObject
    {
        [SerializeField] public Sprite symbol;
        [SerializeField] public new string name;
        [SerializeField] public string deviceDescription;

        [SerializeField] public PromoterData promoter;
        [SerializeField] public RBSData RBS;
        [SerializeField] public CodingSequenceData codingSequence;
        [SerializeField] public TerminatorData terminator;

        public DisplayDevice GetDeviceFromData ()
        {
            return 
                new DisplayDevice
                {
                    symbol = symbol,
                    name = name,
                    deviceDescription = deviceDescription,
                    Promoter = promoter,
                    RBS = RBS,
                    CodingSequence = codingSequence,
                    Terminator = terminator
                };
        }
    }
}
