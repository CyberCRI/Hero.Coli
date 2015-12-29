using UnityEngine;
using Vectrosity;

class TestGraph : MonoBehaviour { 
    void Start () {
        VectorLine.SetLine (Color.green, new Vector2(0, 0), new Vector2(Screen.width-1, Screen.height-1));
    }
}