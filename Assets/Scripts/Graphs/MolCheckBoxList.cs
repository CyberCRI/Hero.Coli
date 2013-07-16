using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MolCheckBoxList : MonoBehaviour {

  public Transform _checkBoxPrefab;
  private Medium _medium;
  LinkedList<Transform> _checkBoxes;

  MolCheckBoxList()
  {
    _checkBoxes = new LinkedList<Transform>();
  }

  public void setMedium(Medium med)
  {
    _medium = med;
    UpdateCheckBoxes();
  }

  private void UpdateCheckBoxes()
  {
    int i = 1;
    if (_medium == null)
      return ;
    ArrayList molecules = _medium.getMolecules();
    if (molecules == null)
      return ;
    foreach (Molecule mol in molecules)
      {
        Vector3 pos = new Vector3(gameObject.transform.position.x - 0.14f, gameObject.transform.position.y + i * 0.05f - 0.07f, gameObject.transform.position.z);

        Transform CB = Instantiate(_checkBoxPrefab, pos, gameObject.transform.rotation) as Transform;
        CB.parent = gameObject.transform;
        CB.name = mol.getName() + "CheckBox";
        UILabel l = CB.Find("Label").GetComponent<UILabel>();
        l.text = mol.getName();
        i++;
      }
  }

  public void changeMoleculeState(string name, bool state)
  {
    MediumInfoWindow MIW = gameObject.transform.parent.GetComponent<MediumInfoWindow>();
    if (MIW == null)
      return;
    MIW.changeMoleculeState(name, state);
  }

	// Use this for initialization
  void Start () {
  }
  
  // Update is called once per frame
  void Update () {
    
  }
  
  void OnGUI()
  {
  }
}
