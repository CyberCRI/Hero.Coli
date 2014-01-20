using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipInfo
{
  public string _code;
  public string _title;
  public string _subtitle;
  public string _texture;
  public string _explanation;
  public string _bottom;

  public TooltipInfo(string code, string title, string subtitle, string texture, string explanation, string bottom)
  {
    _code        = code;
    _title       = title;
    _subtitle    = subtitle;
    _texture     = texture;
    _explanation = explanation;
    _bottom      = bottom;
  }

  public override string ToString ()
  {
    return string.Format ("[TooltipInfo " +
      "_code:"+_code+
      ", _title:"+_title+
      ", _subtitle:"+_subtitle+
      ", _texture:"+_texture+
      ", _explanation:"+_explanation+
      ", _bottom:"+_bottom+
      "]");
  }
}

