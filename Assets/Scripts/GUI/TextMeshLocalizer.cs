using UnityEngine;

public class TextMeshLocalizer : MonoBehaviour {

	[SerializeField]
	private TextMesh testMesh;
	[SerializeField]
	private UILabel fakeLabel;
	[SerializeField]
	private UILocalize localizer;

	void Start () {
		localizer.Localize();
		testMesh.text = fakeLabel.text;
	}
	
	public void localize () {
		localizer.Localize();
		testMesh.text = fakeLabel.text;
	}
}
