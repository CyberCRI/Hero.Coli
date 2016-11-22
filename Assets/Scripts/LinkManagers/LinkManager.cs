using UnityEngine;

/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
 * */

public abstract class LinkManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _activationRoot;
    [SerializeField]
    private GameObject[] _activationArray;
    protected abstract int getLMIndex();

    protected void Start ()
    {
         GameStateController.get().setSceneLoaded(getLMIndex());
    }

    public virtual void initialize()
    {
        // Debug.Log(this.GetType() + " initialize");
    }

    public virtual void finishInitialize()
    {
        // Debug.Log(this.GetType() + " finishInitialize");
    }

    protected void activateAllChildren(bool active)
    {
        if (null != _activationRoot)
        {
            for (int index = 0; index < _activationRoot.transform.childCount; index++)
            {
                _activationRoot.transform.GetChild(index).gameObject.SetActive(active);
            }
        }
        else
        {
            // Debug.Log(this.GetType() + " has null root");
        }
    }

    protected void activateAllInArray(bool active)
    {
        if (null != _activationArray)
        {
            foreach (GameObject obj in _activationArray)
            {
                obj.SetActive(active);
            }
        }
    }

}

