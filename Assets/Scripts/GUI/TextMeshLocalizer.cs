using UnityEngine;

public class TextMeshLocalizer : MonoBehaviour
{
    [SerializeField]
    private TextMesh testMesh;
    [SerializeField]
    private UILabel fakeLabel;
    [SerializeField]
    private UILocalize localizer;

    void Start()
    {
        localize();
    }

    void OnEnable()
    {
        localize();
    }

    public virtual void localize()
    {
        localizer.Localize();
        testMesh.text = fakeLabel.text.Replace("\\n", "\n");
    }
}
