using UnityEngine;

public class LifeIndicator : MonoBehaviour
{
    private Hero _hero;
    [SerializeField]
    private UILabel _lifeValueLabel;

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
            _lifeValueLabel.text = _hero.getDisplayedLife();
        }
    }
}
