using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PickableBioBrickRef : PickableBioBrick
{
	[System.Obsolete("use bioBrickDataCount")]
	[SerializeField]
	private string bioBrickName;
	[System.Obsolete("use bioBrickDataCount")]
	[SerializeField]
	private int amount = 1;

	[SerializeField]
	private BiobrickDataCount bioBrickDataCount;

	protected override DNABit produceDNABit ()
	{
		return BiobrickBuilder.createBioBrickFromData (bioBrickDataCount);
	}
}



