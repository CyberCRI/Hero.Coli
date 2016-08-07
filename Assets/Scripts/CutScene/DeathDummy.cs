using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathDummy : MonoBehaviour {

    private static float _respawnTimeS = 1.5f;
    private static float _disappearingTimeSRatio = 0.9f;
    private static float _disappearingTimeS = _disappearingTimeSRatio * _respawnTimeS;
    private float _popEffectTimeS = 1.0f;
    private static float _baseScale = 145.4339f;
    private static Vector3 _baseScaleVector = new Vector3(_baseScale, _baseScale, _baseScale);
    private static Vector3 _reducedScaleVector = 0.7f * _baseScaleVector;
    private List<GameObject> _flagella = new List<GameObject>();
    
    private bool _locked = false;
    private Vector3 _positionLock;
    [SerializeField]
    private AmpicillinCutScene _ampCutScene;

    private Hashtable _optionsIn = iTween.Hash(
        "scale", _baseScaleVector,
        "time", 0.8f,
        "easetype", iTween.EaseType.easeOutElastic
        );

    private Hashtable _optionsOut = iTween.Hash(
        "scale", _reducedScaleVector,
        "time", _disappearingTimeS,
        "easetype", iTween.EaseType.easeInQuint
        );

    private Hashtable _optionsInAlpha = iTween.Hash(
        "alpha", 1.0f,
        "time", 0.8f,
        "easetype", iTween.EaseType.easeOutElastic
        //"includechildren", false
        );

    private Hashtable _optionsOutAlpha = iTween.Hash(
        "alpha", 0.0f,
        "time", _disappearingTimeS,
        "easetype", iTween.EaseType.easeInQuint
        );

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "DeathZoneDummy")
        {
            if (this.gameObject.name == "3FlagellaPlayerDummy")
            {

            }
            else if (this.gameObject.name == "2FlagellaPlayerDummy")
            {
                StartCoroutine(deathEffectCoroutine());
            }
            
        }
    }

    private void safeFadeTo(Hashtable hash)
    {
        safeFadeTo(gameObject, hash);
    }

    public static void safeFadeTo(GameObject toFade, Hashtable fadeOptions)
    {
        //TODO find most robust method
        //GameObject body = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(). .transform.FindChild("body_perso");
        //GameObject body = gameObject.transform.FindChild("body_perso").gameObject;
        GameObject body = toFade.transform.FindChild("dummy_body_perso").gameObject;
        if (null != body)
        {
            iTween.FadeTo(body, fadeOptions);
        }
        GameObject dna = toFade.transform.FindChild("dummy_dna_perso").gameObject;
        if (null != dna)
        {
            iTween.FadeTo(dna, fadeOptions);
        }
    }

    IEnumerator deathEffectCoroutine()
    {
        iTween.ScaleTo(gameObject, _optionsOut);

        safeFadeTo(_optionsOutAlpha);

        _flagella = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name == "dummy_FBX_flagelPlayer" && child.gameObject.activeSelf)
            {
                _flagella.Add(child.gameObject);
            }
        }

        //1 wait sequence between flagella, pair of eyes disappearances
        //therefore #flagella + #pairs of eyes - 1
        int maxWaitSequences = _flagella.Count;

        //fractional elapsed time
        // 0<elapsed<maxWaitSequences
        float elapsed = 0.0f;

        for (int i = 0; i < _flagella.Count; i++)
        {
            //to make flagella disappear
            float random = UnityEngine.Random.Range(0.0f, 1.0f);
            yield return new WaitForSeconds(random * _respawnTimeS / maxWaitSequences);
            _flagella[i].SetActive(false);
            elapsed += random;
        }

        //to make eyes disappear
        float lastRandom = UnityEngine.Random.Range(0.0f, 1.0f);
        yield return new WaitForSeconds(lastRandom * _respawnTimeS / maxWaitSequences);
        elapsed += lastRandom;
        enableEyes(false);

        _ampCutScene.ResetCamTarget();
        //Destroy(this.gameObject.transform.parent.gameObject);

        yield return new WaitForSeconds((maxWaitSequences - elapsed) * _respawnTimeS / maxWaitSequences);
    }

    private void enableEyes(bool enable)
    {
        foreach (MeshRenderer mr in transform.FindChild("dummy_FBX_eyePlayer").GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = enable;
        }
    }
}
