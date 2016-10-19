using UnityEngine;

public class BubbleAdjuster : MonoBehaviour
{
    [SerializeField]
    private Transform _characterTransform;

    void OnEnable()
    {
        transform.localPosition = _characterTransform.localPosition;
    }

    void Update()
    {
        transform.localPosition = _characterTransform.localPosition;
    }
}