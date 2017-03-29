using UnityEngine;

public class EnergyIndicator : MonoBehaviour
{
    private Character _character;
    [SerializeField]
    private UILabel _energyValueLabel;

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
            _energyValueLabel.text = _character.getDisplayedEnergy();
        }
    }
}
