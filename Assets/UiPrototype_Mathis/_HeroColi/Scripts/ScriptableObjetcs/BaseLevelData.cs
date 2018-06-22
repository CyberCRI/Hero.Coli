using System.Collections.Generic;
using UIProto.Data.Crafting;
using UnityEngine;

namespace UIProto.Scriptable.Level
{
    [CreateAssetMenu(fileName = "new level", menuName = "HeroColi/Level", order = 1)]
    public class BaseLevelData : ScriptableObject
    {
        public List<DeviceDisplayData> baseDevices;

        public List<CraftingBricks> baseBricks;

        public int plasmidesNumber;
    }
}
