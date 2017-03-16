using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "RBSBiobrick", menuName = "Data/Biobrick/RBS", order = 23)]
public class RBSBiobrickData : BiobrickData
{
	public float rbsFactor;

	#region implemented abstract members of BiobrickData

	protected override BioBrick.Type BiobrickType ()
	{
		return BioBrick.Type.RBS;
	}

	#endregion
}