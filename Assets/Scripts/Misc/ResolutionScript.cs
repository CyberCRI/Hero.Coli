using UnityEngine;

public class ResolutionScript : MonoBehaviour
{
    private static int initialWidth = 0;
    private static float initialAspectRatio = 0f;

    public enum RESOLUTION
    {
        NONE,
        NATIVERESOLUTION,
        WIDTH640 = 640,
        WIDTH800 = 800,
        WIDTH1024 = 1024,
        WIDTH1280 = 1280,
        WIDTH1600 = 1600,
        WIDTH1920 = 1920,
        WIDTH2560 = 2560
    }

    public enum ASPECTRATIO
    {
        NONE,
        NATIVEASPECTRATIO,
        RATIO43,
        RATIO1610,
        RATIO169
    }

    public static float getInitialWidth()
    {
        return initialWidth;
    }
    public static float getInitialaspectRatio()
    {
        return initialAspectRatio;
    }

    public void setResolution(RESOLUTION resolution = RESOLUTION.NONE, ASPECTRATIO aspectRatio = ASPECTRATIO.NONE)
    {
        if (resolution != RESOLUTION.NONE)
        {
            // width changes, aspectRatio is fixed
            // Debug.Log(this.GetType() + " setResolution resolution=" + resolution + "=" + ((int)resolution).ToString());
            int width = resolution == RESOLUTION.NATIVERESOLUTION ? initialWidth : (int)resolution;
            int height = Mathf.RoundToInt(width / this.GetComponent<Camera>().aspect);
            Screen.SetResolution(width, height, true);
        }
        else if (aspectRatio != ASPECTRATIO.NONE)
        {
            // width is fixed, aspectRatio changes
            float ratio = initialAspectRatio;
            switch (aspectRatio)
            {
                case ASPECTRATIO.NATIVEASPECTRATIO:
                    break;
                case ASPECTRATIO.RATIO43:
                    ratio = 4f / 3f;
                    break;
                case ASPECTRATIO.RATIO1610:
                    ratio = 1.6f;
                    break;
                case ASPECTRATIO.RATIO169:
                    ratio = 16f / 9f;
                    break;
                default:
                    break;
            }
            this.GetComponent<Camera>().aspect = ratio;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Debug.Log("ScreenResolution=" + Screen.width + "x" + Screen.height);
        initialWidth = 0 == initialWidth ? Screen.width : initialWidth;

        Camera thisCamera = this.GetComponent<Camera>();
        // Debug.Log("CameraResolution=" + thisCamera.pixelWidth + "x" + thisCamera.pixelHeight);

        initialAspectRatio = 0 == initialAspectRatio ? thisCamera.aspect : initialAspectRatio;

#if UNITY_ANDROID && !UNITY_EDITOR
		// Screen.SetResolution(800, 450, true);
		// this.GetComponent<Camera>().aspect = 16f / 9f;
		// Screen.SetResolution(1024, 640, true);
		Screen.SetResolution(1200, 750, true);
		this.GetComponent<Camera>().aspect = 16f / 10f;

#elif UNITY_WEBPLAYER && !UNITY_EDITOR
        Screen.SetResolution(Screen.width, Screen.height, false);

#else
        Screen.SetResolution(1920, 1080, true);
        this.GetComponent<Camera>().aspect = 16f / 9f;
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
