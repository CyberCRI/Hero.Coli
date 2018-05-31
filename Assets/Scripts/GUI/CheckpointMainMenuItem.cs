#define TUTORIAL3

using UnityEngine;
using System.Collections;
/// <summary>
/// When clicked, starts the game at a specific checkpoint
/// </summary>
public class CheckpointMainMenuItem : MainMenuItem
{
    [SerializeField]
    private UILabel _label;
    [SerializeField]
    private Color _selectedColor;
    [SerializeField]
    private Color _activeColor;
    [SerializeField]
    private Color _inactiveColor;

    [SerializeField]
    private UITexture _backgroundTexture;
    [SerializeField]
    private Material _selectedMaterial;
    [SerializeField]
    private Material _activeMaterial;
    [SerializeField]
    private Material _inactiveMaterial;

    private const float _selectedAlpha = 1.0f;
    private const float _activeAlpha = 1.0f;
    private const float _inactiveAlpha = 1.0f;
    private Vector3 _unselectedScale = Vector3.one / hoverExpandingFactor;
    private Vector3 _selectedScale = Vector3.one;
    private Vector3 _amplitude = 0.1f * Vector3.one;
    private const float _timeFactorUnselected = 2;
    private const float _timeFactorSelected = 5;
    private float _timeFactor = _timeFactorUnselected;

    private bool _isActive;

    [Tooltip("The index of the checkpoint. This index determines where the player is spawned")]
    [SerializeField]
    private int _checkpointIndex;
    /// <summary>
    /// Gets the index of the checkpoint. This index determines where the player is spawned
    /// </summary>
    /// <value>The index of the check point.</value>
    public int checkpointIndex
    {
        get
        {
            return _checkpointIndex;
        }
    }

    [Tooltip("The gamemap the checkpoint is linked to.")]
    [SerializeField]
#if !TUTORIAL3
    private GameConfiguration.GameMap _gameMap = GameConfiguration.GameMap.TUTORIAL1;
#else
    private GameConfiguration.GameMap _gameMap = GameConfiguration.GameMap.TUTORIAL3;
#endif
    /// <summary>
    /// The gamemap the checkpoint is linked to.
    /// </summary>
    /// <value>The game map.</value>
    public GameConfiguration.GameMap gameMap
    {
        get
        {
            return _gameMap;
        }
    }
    /// <summary>
    /// Called when the user clicks on this instant. Leaves the main menu and teleports the player to the indexed checkPoint
    /// </summary>
    public override void click()
    {
        if (_isActive)
        {
            base.click();
            GameStateController.get().loadWithCheckpoint(_checkpointIndex, _gameMap);
        }
    }

    public void activate(bool activate)
    {
        // Debug.Log(this.GetType() + " activate(" + activate + ") " + name);
        _isActive = activate;

        setDisplay();
    }

    private void setDisplay()
    {
        // Debug.Log(this.GetType() + " updateTexture " + name);
        _label.alpha = _isActive ? (_isSelected ? _selectedAlpha : _activeAlpha) : _inactiveAlpha;
        _label.color = _isActive ? (_isSelected ? _selectedColor : _activeColor) : _inactiveColor;
        _backgroundTexture.material = _isActive ? (_isSelected ? _selectedMaterial : _activeMaterial) : _inactiveMaterial;
    }

    void OnEnable()
    {
        // Debug.Log(this.GetType() + " OnEnable " + name);
        setDisplay();
    }

    public override void select()
    {
        base.select();
        _timeFactor = _timeFactorSelected;
        setDisplay();
    }

    public override void deselect()
    {
        base.deselect();
        _timeFactor = _timeFactorUnselected;
        setDisplay();
    }

    protected override void Update()
    {
        base.Update();

        // _backgroundTexture.transform.localScale = _isSelected ? _selectedScale : _unselectedScale;
        // _backgroundTexture.transform.localScale += _amplitude * Mathf.Sin(timeFactor * Time.unscaledTime); 

        if (_isActive)
        {
            transform.localScale = _isSelected ? _selectedScale : _unselectedScale;
            transform.localScale += _amplitude * Mathf.Sin(_timeFactor * Time.unscaledTime);
        }
    }
}
