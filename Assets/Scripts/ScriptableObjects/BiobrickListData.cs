using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BiobrickDataCount
{
	public BiobrickData biobrickData;
	public int count = 1;

	public override string ToString()
	{
		return this.GetType() + "[biobrickData=" + biobrickData + ", count=" + count + "]";
	}
}
	
[CreateAssetMenu (fileName = "BiobrickList", menuName = "Data/BiobrickList/Default", order = 22)]
public class BiobrickListData : ScriptableObject {
	public BiobrickDataCount[] biobrickDataList;
}
