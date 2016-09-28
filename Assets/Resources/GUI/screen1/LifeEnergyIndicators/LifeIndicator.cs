using UnityEngine;

public class LifeIndicator : MonoBehaviour
{
	[HideInInspector]
    public Hero hero;
    private const float maxLife = 100f;
    [SerializeField]
    private UILabel _lifeValueLabel;
    private int _lifeValue;

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
            _lifeValue = Mathf.CeilToInt(hero.getLife() * maxLife);
            _lifeValueLabel.text = _lifeValue.ToString();
        }
    }
}
