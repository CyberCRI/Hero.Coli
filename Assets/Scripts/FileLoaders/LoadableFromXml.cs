using System;
using System.Xml;
using UnityEngine;
using System.Collections;
using System.Globalization;

public interface LoadableFromXml
{

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

public class LoadableFromXmlImpl : LoadableFromXml
{

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
        if (isDataCorrect(node))
        {
            innerInstantiateFromXml(node);
            return true;
        }
        else
        {
            return false;
        }
    }

    protected delegate void FloatSetter(float dst);
    protected delegate void  StrSetter(string dst);
    /*!
    \brief This function load and parse a string and give it to the given setter
    \param value The string to parse and load
    \param setter The delegate setter
    \return Return true is success false otherwise
      */
    protected bool loadFloat(string value, FloatSetter setter)
    {
        if (String.IsNullOrEmpty(value))
        {
            Debug.LogError(this.GetType() + " loadFloat empty productionMax field");
            return false;
        }

        // fails on French-localized Linux
        // setter(float.Parse(value.Replace(",", ".")));

        // try
        // {
        //   setter(float.Parse(value.Replace(",", ".")));
        // }
        // catch (FormatException e)
        // {
        //   setter(float.Parse(value.Replace(".", ",")));
        // }

        setter(parseFloat(value));
        return true;
    }

    /*!
    \brief Checks the validity of a string to get parsed as float and returns the float
    \param value the string to check
    \return Returns the float if valid or 0 otherwise
    */
    protected float checkFloat(string value)
    {
        if (String.IsNullOrEmpty(value))
        {
            // Debug.LogWarning(this.GetType() + " Error: Empty field");
            return 0f;
        }
        return parseFloat(value);
    }

    protected float parseFloat(string value)
    {
      return float.Parse(value, CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return "[LoadableFromXmlImpl "
                  + "id:" + _stringId + ";"
                  + "tag:" + _tag
                  + "]";
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
    protected override void innerInstantiateFromXml(XmlNode node)
    {
        // Debug.Log(this.GetType() + " innerInstantiateFromXml(" + Logger.ToString (node) + ")"
        //     + " with elementCollection=" + Logger.ToString<T> ("T", elementCollection)
        //         );

        otherInitialize(node);

        elementCollection = new ArrayList();

        foreach (XmlNode eltNode in node)
        {
            if (XMLTags.COMMENT != eltNode.Name)
            {
                T elt = construct(eltNode);
                if (elt.tryInstantiateFromXml(eltNode))
                {
                    elementCollection.Add(elt);
                }
                else
                {
                    Debug.LogWarning(this.GetType() + " innerInstantiateFromXml could not load elt from " + Logger.ToString(eltNode));
                }
            }
            else
            {
                // Debug.Log(this.GetType() + " CompoundLoadableFromXmlImpl.innerInstantiateFromXml found comment");
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
