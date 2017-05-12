using UnityEngine;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    [SerializeField]
    private Vector2 resolution;
    [SerializeField]
    private float aspectRatio;

    void Start()
    {
        if (0 == aspectRatio && (0 == resolution.y || 0 == resolution.x))
        {
            resolution.x = 1280f;
            resolution.y = 800f;
            aspectRatio = 1.6f;
        }
        else if (0 == resolution.x)
        {
            resolution.x = Mathf.Round(resolution.y * aspectRatio);
        }
        else if (0 == resolution.y)
        {
            resolution.y = Mathf.Round(resolution.x / aspectRatio);
        }
        else if (0 == aspectRatio)
        {
            aspectRatio = resolution.x / resolution.y;
        }
        name = resolution.x.ToString("N0") + "x" + resolution.y.ToString("N0");
        Text buttonText = this.gameObject.GetComponentInChildren<Text>();
        if (null != buttonText)
        {
            buttonText.text = name;
        }
    }

    public void validateResolution()
    {
        Debug.Log(this.GetType() + " " + name + " pressed");
        Screen.SetResolution(Mathf.RoundToInt(resolution.x), Mathf.RoundToInt(resolution.y), true);
        ResolutionScript[] scripts = GameObject.FindObjectsOfType<ResolutionScript>();
        foreach(ResolutionScript script in scripts)
        {
            script.GetComponent<Camera>().aspect = 16f / 10f;
        }
    }
}