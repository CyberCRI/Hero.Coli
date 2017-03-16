using UnityEngine;
using System.Collections;

[CreateAssetMenu (fileName = "TerminatorBiobrick", menuName = "Data/Biobrick/Terminator", order = 24)]
public class TerminatorBiobrickData : BiobrickData
{
	public float terminatorFactor;

	#region implemented abstract members of BiobrickData
	protected override BioBrick.Type BiobrickType ()
	{
		return BioBrick.Type.TERMINATOR;
	}
	#endregion
}