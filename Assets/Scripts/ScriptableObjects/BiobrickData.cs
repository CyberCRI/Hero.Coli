using UnityEngine;
using System.Collections;

public abstract class BiobrickData : ScriptableObject {
	public string id;
	public int size;
	public BioBrick.Type type {
		get { return BiobrickType(); }
	}

	protected abstract BioBrick.Type BiobrickType();
}