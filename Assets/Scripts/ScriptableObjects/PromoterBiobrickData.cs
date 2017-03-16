using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "PromoterBiobrick", menuName = "Data/Biobrick/Promoter", order = 22)]
public class PromoterBiobrickData : BiobrickData
{
	public int beta;
	public string formula;

	#region implemented abstract members of BiobrickData

	protected override BioBrick.Type BiobrickType ()
	{
		return BioBrick.Type.PROMOTER;
	}

	#endregion
}
