using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphWindow : MonoBehaviour {


  public Camera _VectrosityCam;
  private LinkedList<Curve> _curves;
  private ArrayList _molecules;

  GraphWindow()
  {
    _curves = new LinkedList<Curve>();
  }

  public void setMedium(Medium medium)
  {
    _molecules = medium.getMolecules();
    if (_molecules == null)
      return ;
    foreach (Molecule mol in _molecules)
      _curves.AddLast(new Curve(mol, gameObject.GetComponent<Transform>().localPosition, _VectrosityCam));
  }

  void addCurve(Curve c)
  {
    if (c != null)
      _curves.AddLast(c);
  }

  void removeCurve(string name)
  {
    foreach (Curve c in _curves)
      if (c.getLabel() == name)
        _curves.Remove(c);
  }

  public void changeMoleculeState(string name, bool state)
  {
//     Debug.Log(name + state);
    foreach (Curve c in _curves)
      if (c.getLabel() == name)
        {
          c.changeState(state);
        }
  }

  void Start ()
  {
  }

  void Update ()
  {
    LinkedListNode<Curve> node = _curves.First;
    foreach (Molecule mol in _molecules)
      {
        Vector2 p = new Vector2((float)Time.timeSinceLevelLoad*100f, mol.getConcentration());
//         Debug.Log(ps);
//         p.x -=  ps.x;
//         p.y *= 30f;
//         p.x -= ps.x;
//         p.y +=  ps.y + (ps.y * 0.01f);
//         p.y = ps.y;
        node.Value.addPoint(p);
        node = node.Next;
      }

    if (_curves == null)
      return;
    foreach (Curve c in _curves)
      c.updatePts();
  }
  
  void OnGUI()
  {
    // Vector3 size = gameObject.GetComponent<Transform>().size;
    
  }
}
