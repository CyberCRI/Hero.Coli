using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BiobrickDataCount
{
	public BiobrickData biobrickData;
	public int count = 1;
}

[CreateAssetMenu (fileName = "BiobrickList", menuName = "Data/BiobrickList", order = 22)]
public class BiobrickListData : ScriptableObject {
	public BiobrickDataCount[] biobrickDataList;
}
