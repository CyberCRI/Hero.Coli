using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "Device", menuName = "Data/Device", order = 24)]
public class DeviceData : ScriptableObject {
	public string id;
	public BiobrickData[] bricks;

	public override string ToString()
	{
		return this.GetType() + "[bricks=" + Logger.ToString<BiobrickData>(bricks)+ "]";
	}
}
