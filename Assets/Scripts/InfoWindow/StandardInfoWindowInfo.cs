using UnityEngine;

public class StandardInfoWindowInfo
{
  public string _code;
  public string _texture;
  public string _next;

  public StandardInfoWindowInfo(string code, string texture, string next)
  {
    _code        = code;
    _texture     = texture;
    _next        = next;
  }

  public override string ToString ()
  {
    return string.Format ("[StandardInfoWindowInfo " +
      "_code:"+_code+
      ", _texture:"+_texture+
      ", _next:"+_next+
      "]");
  }
}

