using UnityEngine;

public class RespawnPoint: MonoBehaviour {
    private int _colNumber = 0;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            AmbientLighting ambLight = col.GetComponent<AmbientLighting>();
            if (_colNumber == 0)
            {
                ambLight.saveCurrentLighting();
                _colNumber++;
            }
            else
            {
                ambLight.startReset();
            }
        }
    }
}
