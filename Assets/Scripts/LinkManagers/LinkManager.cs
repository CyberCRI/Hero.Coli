using UnityEngine;

/*DESCRIPTION
 * This class creates the links between the Interface's Scene, classes and GameObject and the others
 * */

public abstract class LinkManager : MonoBehaviour {
    
    public GameObject root;

    public abstract void initialize ();
    public abstract void finishInitialize ();
    
    public virtual void activateAllChildren(bool active)
    {
        for(int index = 0 ; index < root.transform.childCount ; index++)
        {
            root.transform.GetChild(index).gameObject.SetActive(active);
        }        
    }
    
}

