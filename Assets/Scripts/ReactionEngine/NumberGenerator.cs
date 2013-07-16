using System;
using UnityEngine;

class NumberGenerator
{

  public delegate float repartitionFunc(float x, float esp, float ecartType);

  private float _min;
  private float _max;
  private float _step;
  private repartitionFunc _func;
  private float[] _tab;
  private int _sizeTab;
  private float _localMax;

  public static float uniform(float x, float esp, float ecartType)
  {
    return 1f;
  }

  public static float normale(float x, float esperance, float ecartType)
  {
    return (float)(1f / ecartType * Math.Sqrt(2f*Math.PI)) * (float)Math.Exp((-1f/2f) * Math.Pow((x - esperance) / ecartType, 2f));
  }


  public NumberGenerator(repartitionFunc func, float min, float max, float step = 0.1f)
  {
    init(func, min, max, step);
  }

  public void init(repartitionFunc func, float min, float max, float step = 0.1f)
  {
    _func = func;
    _max = max;
    _min = min;
    _step = step;
    _tab = new float[(int)((max - min) / step) + 3];
    _sizeTab = (int)((max - min) / step) + 3;
    
    float last = 0f;
    float i = min;
    int j = 0;
    while (i < max)
      {
        _tab[j] = func(i, 0f, 0.01f) + last;
//         Debug.Log(j);
//         Debug.Log(_tab[j]);
        last = _tab[j];
        j++;
        i += step;
      }
    _localMax = last;
//     Debug.Log(j + " != " + _sizeTab + " i = " + i);
//     _tab[j] = last;
  }

  public float getNumber()
  {
    float nb = UnityEngine.Random.Range(_min, _localMax);

    int i = 0;
//     Debug.Log("Start");
//     Debug.Log(nb);
    while (i < _sizeTab && _tab[i] <= nb)
      {
//         Debug.Log(_tab[i] + " <= " + nb);
//         Debug.Log(i);
        i++;
      }
//     Debug.Log("End");
    if (i == 0)
      return 0f;
//     Debug.Log("return : " + ((float)(i - 1) * _step + _min));
    return (float)(i - 1) * _step + _min;
  } 
}