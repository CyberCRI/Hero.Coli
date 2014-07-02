using System;
using System.Xml;
using UnityEngine;

public interface LoadableFromXml {

  //LoadableFromXml(XmlNode node, string id, object loader);

    /* TODO

  //getTag
  //property
  abstract string xmlTag
  {
    get;
  }

  //getStringId
  //property
  public abstract string stringId
  {
      get;
  }
  
  */


  string getTag();

  string getStringId();


  //TODO merge two approaches
    
  //MoleculeSet, ReactionSet, FileLoader
  bool tryInstantiateFromXml(XmlNode node, object loader);

  //ActiveTransport, FickLoader, Medium, XmlLoaderImpl
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
    
    //warning: assumes that node contains correct information
    //implementation of LoadableFromXml interface
    protected virtual void innerInstantiateFromXml(XmlNode node, object loader)
    {
      _stringId = node.Attributes["id"].Value;
    }

    protected bool isIdDataCorrect(XmlNode node)
    {
      return ((null != node) && (null != node.Attributes["id"]) && !string.IsNullOrEmpty(node.Attributes["id"].Value));
    }

    protected virtual bool isDataCorrect(XmlNode node)
    {
      return isIdDataCorrect(node);
    }

    //checks that 'node' contains appropriate id information
    //TODO: check that 'node' contains appropriate additional
    //information for innerInstantiateFromXml
    public virtual bool tryInstantiateFromXml(XmlNode node, object loader)
    {
        Debug.LogError("LoadableFromXml::tryInstantiateFromXml("+Logger.ToString(node)+")");

      if(isDataCorrect(node))
      {
        innerInstantiateFromXml(node, loader);
        return true;
      }
      else
      {
        return false;
      }
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
