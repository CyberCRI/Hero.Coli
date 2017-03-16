using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu (fileName = "CheckPointList", menuName = "Data/BiobrickList/CheckPoint", order = 22)]
public class BiobrickCheckPointListData : BiobrickListData
{
	public BiobrickCheckPointListData previous = null;

	bool isListCircular()
	{
		if (previous == null)
			return false;
		var first = this;
		var second = previous;
		while (first != null && second != null && second.previous != null) {
			if (Object.ReferenceEquals (first, second)) {
				return true;
			}
			first = first.previous;
			second = second.previous.previous;
		}
		return false;
	}

	void OnValidate()
	{
		// Prevents the creation of a circular list
		if (isListCircular ()) {
			Debug.LogError ("Circular list !");
			previous = null;
		}
	}
}

