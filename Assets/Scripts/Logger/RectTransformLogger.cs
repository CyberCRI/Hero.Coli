using UnityEngine;

public class RectTransformLogger : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    void Start() {}

    void Update()
    {   
        Logger.Log(
            "RT[x=" + rectTransform.position.x
            + ", y=" + rectTransform.position.y 
            + ", rect=" + rectTransform.rect
            + ", sizeDelta=" + rectTransform.sizeDelta
            , Logger.Level.ONSCREEN);
    }
}