using System;
using System.Xml;

public interface LoadableFromXml {
    
  string getTag();

  string getStringId();

  void initializeFromXml(XmlNode node, string id);

  T initFromLoad<T,L>(XmlNode node, L loader) where T : new();
  //LoadableFromXml initFromLoad<LoadableFromXml,L>(XmlNode node, L loader);
    
}

public class LoadableFromXmlImpl : LoadableFromXml {

    protected string _stringId = "";
    protected string _tag = "";

    //implementation of LoadableFromXml interface
    public virtual string getTag()
    {
        return _tag;
    }

    public virtual string getStringId()
    {
        return _stringId;
    }

    //implementation of LoadableFromXml interface
    //
    //TODO implement default XML loader that takes tag
    // and then applies loader to all xml node elements
    // that had this tag
    public void initializeFromXml(XmlNode node, string id)
    {
        Logger.Log ("LoadableFromXml::initializeFromXml FAKE CALLED"
                    , Logger.Level.ERROR);
        _stringId = id;
    }

    public T initFromLoad<T,L>(XmlNode node, L loader)
        where T: new()
    {
        Logger.Log ("LoadableFromXml::initFromLoad FAKE CALLED"
                    , Logger.Level.ERROR);

        return new T();
    }
}
