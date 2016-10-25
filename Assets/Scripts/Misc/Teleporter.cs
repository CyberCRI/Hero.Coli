using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private Transform[] _teleportPoints;
    [SerializeField]
    private GameObject[] _zones;

    public void teleport(int index)
    {
        if(index <= _teleportPoints.Length)
        {
            foreach(GameObject zone in _zones)
            {
                zone.SetActive(true);
            }
            CellControl.get(this.GetType().ToString()).teleport(_teleportPoints[index].position);
        }
    }
}