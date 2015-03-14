using System;
using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface LoadableFromXml {

  //LoadableFromXml(XmlNode node, string id);

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
    
  //MoleculeSet, ReactionSet, FileLoader
  //ActiveTransport, FickLoader, Medium, XmlLoaderImpl
  bool tryInstantiateFromXml(XmlNode node);

  //TODO Allostery, EnzymeReaction, InstantReaction, Promoter  
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
    protected virtual void innerInstantiateFromXml(XmlNode node)
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
    public virtual bool tryInstantiateFromXml(XmlNode node)
    {
      if(isDataCorrect(node))
      {
        innerInstantiateFromXml(node);
        return true;
      }
      else
      {
        return false;
      }
    }

  public override string ToString ()
  {
    return string.Format ("[LoadableFromXmlImpl "
                              +"id:"+_stringId+";"
                              +"tag:"+_tag
                              +"]");
  }
}



public abstract class CompoundLoadableFromXmlImpl<T> : LoadableFromXmlImpl 
  where T : LoadableFromXml
{
    
    protected ArrayList elementCollection;

    protected virtual void otherInitialize(XmlNode node)
    {
      _stringId = node.Attributes["id"].Value;
    }

    protected abstract T construct(XmlNode node);

    //warning: assumes that node contains correct information
    protected override void innerInstantiateFromXml (XmlNode node)
    {
        Logger.Log ("CompoundLoadableFromXmlImpl::innerInstantiateFromXml(" + Logger.ToString (node) + ")"
            + " with elementCollection=" + Logger.ToString<T> ("T", elementCollection)
                , Logger.Level.DEBUG);
    
        otherInitialize (node);
            
        elementCollection = new ArrayList ();
    
        foreach (XmlNode eltNode in node) {
            if (XMLTags.COMMENT != eltNode.Name) {
                T elt = construct (eltNode);
                if (elt.tryInstantiateFromXml (eltNode)) {
                    elementCollection.Add (elt);
                } else {
                    Logger.Log ("CompoundLoadableFromXmlImpl.innerInstantiateFromXml could not load elt from " + Logger.ToString (eltNode), Logger.Level.WARN);
                }
            } else {
                Logger.Log ("CompoundLoadableFromXmlImpl.innerInstantiateFromXml found comment", Logger.Level.DEBUG);
            }
        }
    }
}

public class CompoundLoadableFromXmlImplWithNew<T> : CompoundLoadableFromXmlImpl<T>
    where T : LoadableFromXml, new()
{
    protected override T construct(XmlNode node)
    {
      return new T();
    }
}
