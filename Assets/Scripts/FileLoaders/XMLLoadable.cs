using System;
using System.Xml;

public interface XMLLoadable {
    
  string getTag();

  string getId();

  void initializeFromXML(XmlNode node, string id);
    
}

public class XMLLoadableImpl : XMLLoadable {

    private string _id = "";
    private string _tag = "";

    //implementation of XMLLoadable interface
    public string getTag()
    {
        return _tag;
    }

    public string getId()
    {
        return _id;
    }

    //implementation of XMLLoadable interface
    //
    //TODO implement default XML loader that takes tag
    // and then applies loader to all xml node elements
    // that had this tag
    public void initializeFromXML(XmlNode node, string id)
    {
        _id = id;
    }
    
}
