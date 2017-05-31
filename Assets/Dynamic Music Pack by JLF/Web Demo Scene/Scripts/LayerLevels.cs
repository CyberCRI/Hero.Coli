using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class LayerLevels : MonoBehaviour {
	

	public AudioMixer layersMixer;
	
	public void Setl1Lvl (float l1Lvl)
	{
		layersMixer.SetFloat ("l1Vol", l1Lvl);
	}
	
	public void Setl2Lvl (float l2Lvl)
	{
		layersMixer.SetFloat ("l2Vol", l2Lvl);
	}
	public void Setl3Lvl (float l3Lvl)
	{
		layersMixer.SetFloat ("l3Vol", l3Lvl);
	}
}