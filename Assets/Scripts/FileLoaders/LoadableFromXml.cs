using System;
using System.Xml;

public interface LoadableFromXml {
    
  string getTag();

  string getStringId();


  //TODO merge two approaches

  //MoleculeSet, ReactionSet, FileLoader
  void initializeFromXml(XmlNode node, string id);

  //ActiveTransportLoader, FickLoader, MediumLoader, XmlLoaderImpl
  void initFromLoad(XmlNode node, object loader);
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
    public virtual void initializeFromXml(XmlNode node, string id)
    {
        Logger.Log ("LoadableFromXmlImpl::initializeFromXml NOT IMPLEMENTED "+ToString()
                    , Logger.Level.ERROR);
        _stringId = id;
    }

    public virtual void initFromLoad(XmlNode node, object loader)
    {
        Logger.Log ("LoadableFromXmlImpl::initFromLoad NOT IMPLEMENTED "+ToString ()
                    , Logger.Level.ERROR);
    }

  public override string ToString ()
  {
    return string.Format ("[LoadableFromXmlImpl "
                              +"id:"+_stringId+";"
                              +"tag:"+_tag
                              +"]");
  }
}
