using UnityEngine;

public class StandardInfoWindowInfo
{
    public string _code;
    public string _texture;
    public string _next;
    public string _cancel;

    public StandardInfoWindowInfo (string code, string texture, string next, string cancel)
    {
        _code = code;
        _texture = texture;
        _next = next;
        _cancel = cancel;
    }

    public override string ToString ()
    {
        return string.Format ("[StandardInfoWindowInfo _code: {0}, _texture: {1}, _next: {2}, _cancel: {3}]", _code, _texture, _next, _cancel);
    }
}

