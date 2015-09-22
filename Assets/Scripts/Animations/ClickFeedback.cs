using UnityEngine;
using System.Collections;

public class ClickFeedback : MonoBehaviour {

    private float duration = 0f;
    public ParticleEmitter emitter;

    // Use this for initialization
    void Start () {
        duration = emitter.maxEnergy;
        StartCoroutine(selfDestruct());
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(this.gameObject);
    }
}
