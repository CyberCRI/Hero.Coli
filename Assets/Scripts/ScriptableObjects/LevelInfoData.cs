using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "LevelInfo", menuName = "Data/LevelInfo", order = 22)]
public class LevelInfoData : ScriptableObject
{
	public string code;
	public bool inventoryBioBricks;
	public bool inventoryDevices;
	public bool areAllBioBricksAvailable;
	public BiobrickListData availableBioBricks;
	public bool areAllDevicesAvailable;
	public DeviceListData availableDevices;

	public override string ToString ()
	{
		return string.Format ("[LevelInfo code: {0}, areAllBricksAvailable: {1}, areAllDevicesAvailable: {2}]", code, areAllBioBricksAvailable, areAllDevicesAvailable);
	}
}