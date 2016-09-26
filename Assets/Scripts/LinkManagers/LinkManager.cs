using UnityEngine;

/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
 * */

public abstract class LinkManager : MonoBehaviour
{
    public GameObject root;
    protected abstract int getLMIndex();

    protected void Start ()
    {
         GameStateController.get().setSceneLoaded(getLMIndex());
    }

    public virtual void initialize()
    {
        Debug.Log(this.GetType() + " initialize");
    }

    public virtual void finishInitialize()
    {
        Debug.Log(this.GetType() + " finishInitialize");
    }

    public virtual void activateAllChildren(bool active)
    {
        if (null != root)
        {
            for (int index = 0; index < root.transform.childCount; index++)
            {
                root.transform.GetChild(index).gameObject.SetActive(active);
            }
        }
        else
        {
            Debug.Log(this.GetType() + " has null root");
        }
    }

}

