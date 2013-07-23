using System;
using UnityEngine;

/*!
  \brief This class gives discretes randoms numbers based on a distribution function
 */
public class NumberGenerator
{

  public delegate float repartitionFunc(float x, float esp, float ecartType);           //!< The delegate of a repartition function

  private float _min;                           //!< The minimum
  private float _step;                          //!< The step
  private float[] _tab;                         //!< The table of number used to generate number
  private int _sizeTab;                         //!< The size of the table
  private float _localMax;                      //!< The maximum in the tab

  /*!
    \brief A basic uniform distribution function
    \return Return always 1;
   */
  public static float uniform(float x, float esp, float ecartType)
  {
    return 1f;
  }

  /*!
    \brief A normal distribution function
    \param x x parameter
    \param esperance The esperance
    \param ecartType The ecartType
   */
  public static float normale(float x, float esperance, float ecartType)
  {
    return (float)(1f / ecartType * Math.Sqrt(2f*Math.PI)) * (float)Math.Exp((-1f/2f) * Math.Pow((x - esperance) / ecartType, 2f));
  }

  //! Constructor
  public NumberGenerator(repartitionFunc func, float min, float max, float step = 0.1f)
  {
    init(func, min, max, step);
  }

  /*!
    \brief Initialize the generator
    \param func The repartition function
    \param min The minimum value
    \param max The maximum value
    \param step The step
   */
  public void init(repartitionFunc func, float min, float max, float step = 0.1f)
  {
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
        last = _tab[j];
        j++;
        i += step;
      }
    _localMax = last;
  }

  //! return a random number
  public float getNumber()
  {
    float nb = UnityEngine.Random.Range(0f, _localMax);

    int i = 0;
    while (i < _sizeTab && _tab[i] <= nb)
      i++;
    if (i == 0)
        return 0f;
    return (float)(i) * _step + _min;
  } 
}