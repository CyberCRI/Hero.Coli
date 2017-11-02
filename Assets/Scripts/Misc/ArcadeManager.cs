using UnityEngine;
using System.Collections.Generic;
#if ARCADE
using System.IO.Ports;
#endif
using System;

/// <summary>
/// Manages interactions specifically with the connected arduino of our arcade setting
/// Loosely built upon github.com/raphik12/arduino-sandbox
/// </summary>
public class ArcadeManager : MonoBehaviour
{
    private string _player1Port = "/dev/ttyACM0";
    private string _player2Port = "COM4";
    // private const string _portNameStemWin = "COM3", "COM4";
    // private const string _portNameStemMac = "/dev/cu.usbmodem1421", "/dev/tty.usbmodem1411";
    // private const string _portNameStemLinux = "/dev/ttyACM0";
#if ARCADE
    private SerialPort _port;
#endif
    [SerializeField]
    private PLATFORM _platform;
    [SerializeField]
    private bool _initialized = false;
    private static ArcadeManager _instance = null;

    public enum Animation
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

    private Dictionary<Animation, string> _animations = new Dictionary<Animation, string> {
        {Animation.none, ""},
        {Animation.gui_default, "F"},
        {Animation.bacterium_move, "A"}, // for sped-up movement
        {Animation.pickup_dna, "B"},
        {Animation.pickup_nanobot, "C"},
        {Animation.bacterium_death, "D"},
        {Animation.bacterium_respawn, "E"},
        {Animation.gui_brick_add, "F"},
        {Animation.gui_brick_remove, "F"},
        // {Animation.gui_menu_navigate_n, "H"},
        // {Animation.gui_pop_up_appear, "I"},
        // {Animation.gui_default_click, "J"},
        {Animation.gui_create_device, "K"},
        {Animation.gui_assemble_device, "L"},
        // {Animation.bacterium_death_mine, "M"},
        // {Animation.bacterium_death_enemy, "N"},
        {Animation.bacterium_hurt_antibiotics, "O"},
        // {Animation.bacterium_death_antibiotics, "P"},
        // {Animation.scenery_bubbles, "Q"},
        // {Animation.bacterium_idle, "R"},
        // {Animation.bacterium_illuminate_normal, "S"},
        {Animation.bacterium_rfp_start, "R"},
        {Animation.bacterium_rfp_end, "r"},
        {Animation.bacterium_gfp_start, "S"},
        {Animation.bacterium_gfp_end, "s"},
        // {Animation.bacterium_illuminate_darkness, "T"},
        // {Animation.scenery_door_open, "U"},
        // {Animation.scenery_door_close, "V"},
        // {Animation.bacterium_death_energy, "W"},
        {Animation.bacterium_hurt_energy, "X"},
        // {Animation.bacterium_death_crushed, "Y"},
        // {Animation.bacterium_bubbles, "Z"},
        // {Animation.bacterium_death_suicide, "a"},
        // {Animation.gui_device_remove, "b"},
        // {Animation.scenery_light_off, "c"},
        // {Animation.scenery_light_on, "d"},
        // {Animation.scenery_rock_push, "e"},
        // {Animation.music_tutorial, "f"},
        {Animation.bacterium_divide, "g"},
        // {Animation.bacterium_dialog, "h"},
        // {Animation.bacterium_loses_energy, "i"},
        {Animation.gui_tactile_start, "J"},
        {Animation.gui_tactile_end, "j"},
        {Animation.bacterium_speed_start, "A"},
        {Animation.bacterium_speed_end, "a"},
        {Animation.bacterium_hurt_antibiotics_start, "O"},
        {Animation.bacterium_hurt_antibiotics_end, "o"},
        {Animation.bacterium_hurt_energy_start, "X"},
        {Animation.bacterium_hurt_energy_end, "x"},
    };

    private string getCode(Animation animation)
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

#if ARCADE
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
				instance._port.WriteTimeout = 500;
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
#endif

    public void playAnimation(Animation animation)
    {
        // Debug.Log(this.GetType() + " playAnimation(" + animation + ")");
        playAnimation(getCode(animation));
    }

    private void playAnimation(string identifier)
    {
#if ARCADE
        if (!string.IsNullOrEmpty(identifier))
        {
            if (checkConnection())
            {
                // Debug.Log(this.GetType() + " playAnimation(" + identifier + "): port open");
                _port.Write(identifier);
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

#endif
    }
}