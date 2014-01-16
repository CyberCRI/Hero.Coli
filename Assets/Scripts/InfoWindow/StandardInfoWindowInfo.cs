using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StandardInfoWindowInfo
{
  public string _title;
  public string _subtitle;
  public string _texture;
  public string _explanation;
  public string _bottom;
  //TODO onNext;

  public StandardInfoWindowInfo(string title, string subtitle, string texture, string explanation, string bottom)
  {
    _title       = title;
    _subtitle    = subtitle;
    _texture     = texture;
    _explanation = explanation;
    _bottom      = bottom;
    //TODO onNext;
  }
}

