using UnityEngine;

public class FlagellaSetter : MonoBehaviour
{
    [SerializeField]
    private GameObject centralFlagellum;
    [SerializeField]
    private GameObject leftFlagellum;
    [SerializeField]
    private GameObject rightFlagellum;
    [SerializeField]
    private GameObject sideFlagellum;

    protected int _flagellaCount = 0;
    public int flagellaCount
    {
        get
        {
            return _flagellaCount;
        }
    }

    public void setFlagellaCount(int count)
    {
        _flagellaCount = count;
        leftFlagellum.SetActive(count >= 2);
        centralFlagellum.SetActive(count == 1 || count >= 3);
        rightFlagellum.SetActive(count >= 2);
        sideFlagellum.SetActive(count == 4);
    }
}