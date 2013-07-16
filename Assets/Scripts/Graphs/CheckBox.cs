using UnityEngine;
using System.Collections;

public class CheckBox : MonoBehaviour {

  // Use this for initialization
  void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
    
  }
  
  void OnActivate(bool state)
  {
    MolCheckBoxList p = gameObject.transform.parent.GetComponent<MolCheckBoxList>();
    if (p == null)
      return;
    p.changeMoleculeState(gameObject.GetComponentInChildren<UILabel>().text, state);
  }
}
