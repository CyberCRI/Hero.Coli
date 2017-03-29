using UnityEngine;

public class LifeIndicator : MonoBehaviour
{
    private Character _character;
    [SerializeField]
    private UILabel _lifeValueLabel;

    // Use this for initialization
    void Start()
    {
        //TODO trigger this after resize
        _character = Character.get();
    }

    // Update is called once per frame
    void Update()
    {
        if (_character != null)
        {
            _lifeValueLabel.text = _character.getDisplayedLife();
        }
    }
}
