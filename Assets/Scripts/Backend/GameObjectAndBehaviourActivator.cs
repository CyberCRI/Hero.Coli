using UnityEngine;
using UnityEngine.UI;

public class GameObjectAndBehaviourActivator : MonoBehaviour
{
    public GameObject goToActivate;
    public Behaviour behaviourToActivate;
    public Button button;

    void Start()
    {
        if (null != goToActivate)
        {
            name = goToActivate.name;
        }
        else if (null != behaviourToActivate)
        {
            name = behaviourToActivate.name;
        }
        Text buttonText = this.gameObject.GetComponentInChildren<Text>();
        if (null != buttonText)
        {
            buttonText.text = name;
        }
        updateColor();
    }

    public void click()
    {
        Debug.Log(this.GetType() + " " + name + " pressed");
        if (null != goToActivate)
        {
            goToActivate.SetActive(!goToActivate.activeSelf);
        }
        else if (null != behaviourToActivate)
        {
            behaviourToActivate.enabled = !behaviourToActivate.enabled;
        }
        updateColor();
    }

    private void updateColor()
    {
        if (null != goToActivate && null != button)
        {
            button.gameObject.GetComponent<Image>().color = goToActivate.activeSelf ? Color.white : Color.black;
        }
        else if (null != behaviourToActivate && null != button)
        {
            button.gameObject.GetComponent<Image>().color = behaviourToActivate.enabled ? Color.white : Color.black;
        }
    }
}