using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TinyBioBrickIcon : GenericDisplayedBioBrick {
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
    result.transform.localScale = new Vector3(0.4f, 0.4f, 1.0f);
    return result;
  }
}
