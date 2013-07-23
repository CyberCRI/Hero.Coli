using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class Curve
{

  public const int _maxPoints = 200;
  private Molecule _mol;
  private string _label;
  private LinkedList<Vector2> _points;
  private VectorLine _line;
  private Vector2[] _pts;

  private LineType[] _linesTypes;
//   private VectorLine[] _lines;

  private float _minY;
  private float _maxY;
  private Color _color;
  private Camera _vectroCam;
  public bool _isEnabled;
  private Vector2 _pos;

  public Curve(Molecule mol, Vector2 pos, Camera VectroCam = null)
  {
    _mol = mol;
    _label = mol.getName();
    _points = new LinkedList<Vector2>();
    _pts = new Vector2[_maxPoints];
    _minY = 0;
    _maxY = 0;
    _color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
    _line = new VectorLine(mol.getName(), _pts, _color, null, 2.0f, LineType.Continuous, Joins.Weld);
    _isEnabled = true;
    _vectroCam = VectroCam;
//     _pos = pos;

//     _lines = new VectorLine[_maxPoints];
//     _linesTypes = new _linesTypes[_maxPoints - 1];

    VectorManager.useDraw3D = true;
    if (_vectroCam != null)
      Vectrosity.VectorLine.SetCamera(_vectroCam);
    else
      Debug.Log("No Camera set for the Graph Window.");
  }

  public float getLastY() { if (_points != null && _points.Last != null) return _points.Last.Value.y; return 0;}
  public float getMinX() { if (_points.Count > 0) return _points.First.Value.x; return 0;}
  public float getMaxX() { if (_points.Count > 0) return _points.Last.Value.x; return 0;}
  public float getMaxY() { return _maxY; }
  public float getMinY() { return _minY; }
  public Color getColor() { return _color; }
  public string getLabel() { return _label; }
  public bool isEnabled() { return _isEnabled; }
  public void changeState(bool state)
  {
    if (state == false)
      {
        if (_line != null)
          _line.active = false;
      }
    else
      _line.active = true;
    _isEnabled = state;
  }
  public LinkedList<Vector2> getPointsList() { return _points; }

  public void updateMinMax()
  {
    float max = _points.First.Value.y;
    float min = _points.First.Value.y;

    foreach (Vector2 v in _points)
      {
        if (v.y > max)
          max = v.y;
        if (v.y < min)
          min = v.y;
      }
    _maxY = max;
    _minY = min;
  }
  
  public void removeFirstPoint()
  {
    if (_maxY == _points.First.Value.y || _minY == _points.First.Value.y)
      {
        _points.RemoveFirst();
        updateMinMax();
      }
    else
      _points.RemoveFirst();
  }

  public void addPoint(Vector2 pt)
  {
    if (_points.Count >= _maxPoints)
      removeFirstPoint();
//     Debug.Log("add: "+pt.x+" pt.y="+pt.y);
    _points.AddLast(pt);
  }

  public void addPoint()
  {
    if (_mol == null)
      {
        Debug.Log("No molecule define for this Curve");
        return ;
      }
    Vector2 p = new Vector2((float)Time.timeSinceLevelLoad * 200f, _mol.getConcentration());
    addPoint(p);
  }


  public void updatePts()
  {
    int i = 0;

    if (_isEnabled == false)
      return;

    foreach (Vector2 pt in _points)
      {
        Vector2 tmpPt = new Vector2();
        tmpPt.y = pt.y * 3f;
        tmpPt.x = pt.x;
        tmpPt.x -= getMinX();
//         if (i == 0)
//           Debug.Log(tmpPt);
        _pts[i] = tmpPt;
        i++;
      }    
    _line.drawStart = 0;
    _line.drawEnd = _points.Count - 1;
    _line.Draw();
  }

//   public Vector2[] getPts()
//   {
//     updatePts();
//     return _pts;
//   }

}