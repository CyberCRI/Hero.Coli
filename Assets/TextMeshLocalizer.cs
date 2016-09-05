using UnityEngine;

public class TextMeshLocalizer : MonoBehaviour {

	[SerializeField]
	private TextMesh testMesh;
	[SerializeField]
	private UILabel fakeLabel;

	void Start () {
		testMesh.text = fakeLabel.text;
	}
	
	public void localize () {
		testMesh.text = fakeLabel.text;
	}
}
