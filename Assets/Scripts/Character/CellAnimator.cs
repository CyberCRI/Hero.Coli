using UnityEngine;
using System.Collections;

public abstract class CellAnimator : MonoBehaviour {
    [SerializeField]
    protected GameObject _body;
    [SerializeField]
    protected GameObject _dna;    
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

    public void safeFadeTo(Hashtable fadeOptions)
    {
        if (null != _body)
        {
            iTween.FadeTo(_body, fadeOptions);
        }
        if (null != _dna)
        {
            iTween.FadeTo(_dna, fadeOptions);
        }
    }
}