using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*!
\brief This class manages the display of BioBricks under the equiped devices icons
\details The only differences with GenericDisplayedBioBrick is the scale and the background

\sa GenericDisplayedBioBrick
\author Raphael GOUJET
*/

public class TinyBioBrickIcon : GenericDisplayedBioBrick {

  public UISprite icon;
  public UISprite background;


  public static TinyBioBrickIcon Create(
    Transform parentTransform
    ,Vector3 localPosition
    ,string spriteName
    ,BioBrick biobrick
  )
  {
    TinyBioBrickIcon result = (TinyBioBrickIcon)GenericDisplayedBioBrick.Create(
      parentTransform
      , localPosition
      , spriteName
      , biobrick
      , _genericPrefab
      );

    return result;
  }

}
