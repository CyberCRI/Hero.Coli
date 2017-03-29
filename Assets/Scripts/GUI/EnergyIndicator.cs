using UnityEngine;

public class EnergyIndicator : MonoBehaviour
{
    private Hero _hero;
    [SerializeField]
    private UILabel _energyValueLabel;

    // Use this for initialization
    void Start()
    {
        //TODO trigger this after resize
        _hero = Hero.get();
    }

    // Update is called once per frame
    void Update()
    {
        if (_hero != null)
        {
            _energyValueLabel.text = _hero.getDisplayedEnergy();
        }
    }
}
