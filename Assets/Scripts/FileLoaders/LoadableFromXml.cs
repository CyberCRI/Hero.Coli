using System;
using System.Xml;

public interface LoadableFromXml {
    
  string getTag();

  string getId();

  void initializeFromXml(XmlNode node, string id);
    
}

public class LoadableFromXmlImpl : LoadableFromXml {

    private string _id = "";
    private string _tag = "";

    //implementation of LoadableFromXml interface
    public string getTag()
    {
        return _tag;
    }

    public string getId()
    {
        return _id;
    }

    //implementation of LoadableFromXml interface
    //
    //TODO implement default XML loader that takes tag
    // and then applies loader to all xml node elements
    // that had this tag
    public void initializeFromXml(XmlNode node, string id)
    {
        _id = id;
    }
    
}
