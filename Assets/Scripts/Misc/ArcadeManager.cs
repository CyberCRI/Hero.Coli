using UnityEngine;
using System.Collections.Generic;
using System.IO.Ports;
using System;

/// <summary>
/// Manages interactions specifically with the connected arduino of our arcade setting
/// Loosely built upon github.com/raphik12/arduino-sandbox
/// </summary>
/// 

public enum ArcadeAnimation
{
	none,                                       // warning: set on Prefab, auto-shifted when an animation is added before
	gui_default,                                // warning: set on Prefab, auto-shifted when an animation is added before
	bacterium_move,
	pickup_dna,
	pickup_nanobot,
	bacterium_death,
	bacterium_respawn,
	gui_brick_add,                              // warning: set on Prefab, auto-shifted when an animation is added before
	gui_brick_remove,                           // warning: set on Prefab, auto-shifted when an animation is added before
	// gui_menu_navigate_n,
	// gui_pop_up_appear,
	// gui_default_click,
	gui_create_device,
	gui_assemble_device,
	// bacterium_death_mine,
	// bacterium_death_enemy,
	bacterium_hurt_antibiotics,
	// bacterium_death_antibiotics,
	// scenery_bubbles,
	// bacterium_idle,
	// bacterium_illuminate_normal,
	// bacterium_illuminate_darkness,
	bacterium_gfp_start,
	bacterium_gfp_end,
	bacterium_rfp_start,
	bacterium_rfp_end,
	// scenery_door_open,
	// scenery_door_close,
	// bacterium_death_energy,
	bacterium_hurt_energy,
	// bacterium_death_crushed,
	// bacterium_bubbles,
	// bacterium_death_suicide,
	// gui_device_remove,
	// scenery_light_off,
	// scenery_light_on,
	// scenery_rock_push,
	// music_tutorial,
	bacterium_divide,
	// bacterium_dialog,
	// bacterium_loses_energy,
	gui_tactile_start,
	gui_tactile_end,
	bacterium_speed_start,
	bacterium_speed_end,
	bacterium_hurt_antibiotics_start,
	bacterium_hurt_antibiotics_end,
	bacterium_hurt_energy_start,
	bacterium_hurt_energy_end,
}

#if ARCADE
public class ArcadeManager : MonoBehaviour
{
    private string _player1Port = "/dev/ttyACM0";
    private string _player2Port = "COM4";
    // private const string _portNameStemWin = "COM3", "COM4";
    // private const string _portNameStemMac = "/dev/cu.usbmodem1421", "/dev/tty.usbmodem1411";
    // private const string _portNameStemLinux = "/dev/ttyACM0";
    private SerialPort _port;
    [SerializeField]
    private PLATFORM _platform;
    [SerializeField]
    private bool _initialized = false;
    private static ArcadeManager _instance = null;
	private Queue<ArcadeAnimation> _animationQueue = new Queue<ArcadeAnimation>();

    private Dictionary<ArcadeAnimation, string> _animations = new Dictionary<ArcadeAnimation, string> {
        {ArcadeAnimation.none, ""},
        {ArcadeAnimation.gui_default, "F"},
        {ArcadeAnimation.bacterium_move, "A"}, // for sped-up movement
        {ArcadeAnimation.pickup_dna, "B"},
        {ArcadeAnimation.pickup_nanobot, "C"},
        {ArcadeAnimation.bacterium_death, "D"},
        {ArcadeAnimation.bacterium_respawn, "E"},
        {ArcadeAnimation.gui_brick_add, "F"},
        {ArcadeAnimation.gui_brick_remove, "F"},
        // {ArcadeAnimation.gui_menu_navigate_n, "H"},
        // {ArcadeAnimation.gui_pop_up_appear, "I"},
        // {ArcadeAnimation.gui_default_click, "J"},
        {ArcadeAnimation.gui_create_device, "K"},
        {ArcadeAnimation.gui_assemble_device, "L"},
        // {ArcadeAnimation.bacterium_death_mine, "M"},
        // {ArcadeAnimation.bacterium_death_enemy, "N"},
        {ArcadeAnimation.bacterium_hurt_antibiotics, "O"},
        // {ArcadeAnimation.bacterium_death_antibiotics, "P"},
        // {ArcadeAnimation.scenery_bubbles, "Q"},
        // {ArcadeAnimation.bacterium_idle, "R"},
        // {ArcadeAnimation.bacterium_illuminate_normal, "S"},
        {ArcadeAnimation.bacterium_rfp_start, "R"},
        {ArcadeAnimation.bacterium_rfp_end, "r"},
        {ArcadeAnimation.bacterium_gfp_start, "S"},
        {ArcadeAnimation.bacterium_gfp_end, "s"},
        // {ArcadeAnimation.bacterium_illuminate_darkness, "T"},
        // {ArcadeAnimation.scenery_door_open, "U"},
        // {ArcadeAnimation.scenery_door_close, "V"},
        // {ArcadeAnimation.bacterium_death_energy, "W"},
        {ArcadeAnimation.bacterium_hurt_energy, "X"},
        // {ArcadeAnimation.bacterium_death_crushed, "Y"},
        // {ArcadeAnimation.bacterium_bubbles, "Z"},
        // {ArcadeAnimation.bacterium_death_suicide, "a"},
        // {ArcadeAnimation.gui_device_remove, "b"},
        // {ArcadeAnimation.scenery_light_off, "c"},
        // {ArcadeAnimation.scenery_light_on, "d"},
        // {ArcadeAnimation.scenery_rock_push, "e"},
        // {ArcadeAnimation.music_tutorial, "f"},
        {ArcadeAnimation.bacterium_divide, "g"},
        // {ArcadeAnimation.bacterium_dialog, "h"},
        // {ArcadeAnimation.bacterium_loses_energy, "i"},
        {ArcadeAnimation.gui_tactile_start, "J"},
        {ArcadeAnimation.gui_tactile_end, "j"},
        {ArcadeAnimation.bacterium_speed_start, "A"},
        {ArcadeAnimation.bacterium_speed_end, "a"},
        {ArcadeAnimation.bacterium_hurt_antibiotics_start, "O"},
        {ArcadeAnimation.bacterium_hurt_antibiotics_end, "o"},
        {ArcadeAnimation.bacterium_hurt_energy_start, "X"},
        {ArcadeAnimation.bacterium_hurt_energy_end, "x"},
    };

    private string getCode(ArcadeAnimation animation)
    {
        string code = "A";
        _animations.TryGetValue(animation, out code);
        return code;
    }

    private enum PLATFORM
    {
        WIN,
        MAC,
        LINUX
    }

    public static ArcadeManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (ArcadeManager)FindObjectOfType<ArcadeManager>();
                if (_instance == null)
                {
                    // Create gameObject and add component
                    _instance = (new GameObject("ArcadeManager")).AddComponent<ArcadeManager>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            instance.Init();
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Init()
    {
        if (!_initialized)
        {
            _initialized = true;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        checkConnection();
    }

    private static bool isPortOpen()
    {
        return (null != instance._port) && (instance._port.IsOpen);
    }

    private static bool checkConnection()
    {
        if (!isPortOpen())
        {
            // Debug.Log("ArcadeManager checkConnection will try to connect");
            instance._port = new SerialPort(instance._player1Port, 9600);
            try
            {
                instance._port.Open();
				instance._port.WriteTimeout = 50;
                // Debug.Log("ArcadeManager checkConnection port successfully open");
            }
            catch (Exception e)
            {
                Debug.LogError("ArcadeManager checkConnection opening the stream failed: " + e);
                return false;
            }

            return isPortOpen();
        }
        else
        {
            // Debug.Log("ArcadeManager checkConnection port was already open");
            return true;
        }
    }

    public void switchPort(string newPort)
    {
        // Debug.Log(this.GetType() + " switchPort(" + newPort + ")");
        if(_port.IsOpen)
        {
            _port.Close();
        }
        _player1Port = newPort;
        checkConnection();
    }

    public void playAnimation(ArcadeAnimation animation)
    {
        // Debug.Log(this.GetType() + " playAnimation(" + animation + ")");
		_animationQueue.Enqueue(animation); 
    }

	private string getQueueContent()
	{
		string res = "";

		foreach (var animation in _animationQueue) {
			res += getCode (animation);
		}

		return res;
	}

   	private void write(string identifier)
	{
        if (!string.IsNullOrEmpty(identifier))
        {
            if (checkConnection())
            {
                // Debug.Log(this.GetType() + " playAnimation(" + identifier + "): port open");
				try {
					_port.Write(identifier);
				}
				catch (Exception e)
				{
					Debug.LogError ("identifier = " + identifier);
					Debug.LogError ("stack content = " + getQueueContent());
					Debug.LogError(e);
				}
            }
            else
            {
                Debug.LogWarning(this.GetType() + " playAnimation(" + identifier + "): port closed");
            }
        }
        else
        {
            // Debug.Log(this.GetType() + " playAnimation(" + identifier + "): parameter string was null");
        }
    }

	void Update()
	{
		if (_animationQueue.Count > 0)
			write(getCode(_animationQueue.Dequeue()));
	}
}
#endif 