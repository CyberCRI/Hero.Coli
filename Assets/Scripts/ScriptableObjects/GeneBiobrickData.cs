using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "GeneBiobrick", menuName = "Data/Biobrick/Gene", order = 24)]
public class GeneBiobrickData : BiobrickData
{
	public string protein;

	#region implemented abstract members of BiobrickData

	protected override BioBrick.Type BiobrickType ()
	{
		return BioBrick.Type.GENE;
	}

	#endregion
}

