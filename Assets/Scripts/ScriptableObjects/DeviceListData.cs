using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "DeviceList", menuName = "Data/DeviceList", order = 24)]
public class DeviceListData: ScriptableObject {
	public DeviceData[] deviceDataList;
}

