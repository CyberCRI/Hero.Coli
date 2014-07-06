using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

/*!
\brief This class loads all the allostery reactions
\details A allostery's reaction's declaration should respect this syntax :

    <allostery>
      <name>inhibitLacI</name>
      <effector>IPTG</effector>
      <EnergyCost>0.3</EnergyCost>
      <K>0.1</K>
      <n>2</n>
      <protein>LacI</protein>
      <products>LacI*</products>
    </allostery>

\sa Allostery

*/
public class AllosteryLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);

  /*!
\brief This function load and parse a string and give it to the given setter
\param value The string to parse and load
\param setter The delegate setter
\return Return true is success false otherwise
  */
  private bool loadAllosteryString(string value, StrSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty name field");
        return false;
      }
    setter(value);
    return true;    
  }

  /*!
\brief This function load and parse a string and give it to the given setter
\param value The string to parse and load
\param setter The delegate setter
\return Return true is success false otherwise
  */
  private bool loadAllosteryFloat(string value, FloatSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty productionMax field");
        return false;
      }
    setter(float.Parse(value.Replace(",", ".")));
    return true;    
  }
}