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
      , prefab
      );
    //result.transform.localScale = new Vector3(0.4f, 0.4f, 1.0f);
    Vector3 pos = result.icon.transform.localPosition;
    result.icon.transform.localPosition = new Vector3(pos.x, pos.y, -0.2f);
    pos = result.background.transform.localPosition;
    result.background.transform.localPosition = new Vector3(pos.x, pos.y, -0.1f);

    return result;
  }

  /*
  void Update()
  {
    Vector3 pos = icon.transform.localPosition;
    icon.transform.localPosition = new Vector3(pos.x, pos.y, -0.2f);

    pos = background.transform.localPosition;
    background.transform.localPosition = new Vector3(pos.x, pos.y, -0.1f);
  }
  */
}
