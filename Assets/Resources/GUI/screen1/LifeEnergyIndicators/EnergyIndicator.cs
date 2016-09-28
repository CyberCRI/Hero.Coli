using UnityEngine;

public class EnergyIndicator : MonoBehaviour
{
	[HideInInspector]
    public Hero hero;
    private const float maxEnergy = 100f;
    [SerializeField]
    private UILabel _energyValueLabel;
    private int _energyValue;

    // Use this for initialization
    void Start()
    {
        //TODO trigger this after resize
        hero = Hero.get();
    }

    // Update is called once per frame
    void Update()
    {
        if (hero != null)
        {
            _energyValue = Mathf.CeilToInt(hero.getEnergy() * maxEnergy);
            _energyValueLabel.text = _energyValue.ToString();
        }
    }
}
