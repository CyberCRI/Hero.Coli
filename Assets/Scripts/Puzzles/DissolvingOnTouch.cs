using UnityEngine;
using System.Collections;

public class DissolvingOnTouch : MonoBehaviour {

    public float _disappearingTimeS;
    public float _destroyingTimeS;

    void OnCollisionEnter(Collision col) {
        if (col.collider){
            if(col.gameObject.GetComponent<CellControl>()){
                StartCoroutine(dissolveCoroutine());
            }
        }
    }

    IEnumerator dissolveCoroutine() {

        Hashtable _optionsOutAlpha = iTween.Hash(
            "alpha", 0.0f,
            "time",_disappearingTimeS,
            "easetype", iTween.EaseType.easeInQuint
            );

        foreach(MeshRenderer r in this.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            iTween.FadeTo(r.gameObject, _optionsOutAlpha);
        }

        yield return new WaitForSeconds(_destroyingTimeS);
        Destroy(this.gameObject);
    }
}
