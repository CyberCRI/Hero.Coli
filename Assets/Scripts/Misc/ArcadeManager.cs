using UnityEngine;
using System.IO.Ports;
using System;

/// <summary>
/// Manages interactions specifically with the connected arduino of our arcade setting
/// Loosely built upon github.com/raphik12/arduino-sandbox
/// </summary>
public class ArcadeManager : MonoBehaviour
{
    [SerializeField]
    private string _player1Port = "COM3", _player2Port = "COM4";
    private const string _portNameStemWin = "COM";
    private const string _portNameStemMac = "/dev/cu.usbmodem1421" /*/dev/tty.usbmodem1421*/;
    private const string _portNameStemLinux = "/dev/ttyACM";
    private SerialPort _port;
    [SerializeField]
    private PLATFORM _platform;
    [SerializeField]
    private string[] ports;
    private bool _initialized = false;
    private static ArcadeManager _instance = null;

    public const string deathAnimation = "B";
    public const string divisionAnimation = "C";

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
                    _instance = (new GameObject("ArduinoManager")).AddComponent<ArcadeManager>();
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
        ports = SerialPort.GetPortNames();
        checkConnection();

        playAnimation("A");
    }

    private static bool isPortOpen()
    {
        return (null != instance._port) && (instance._port.IsOpen);
    }

    private static bool checkConnection()
    {
        if (!isPortOpen())
        {
            Debug.Log("ArcadeManager checkConnection will try to connect");
            instance._port = new SerialPort(instance._player1Port, 9600);
            try
            {
                instance._port.Open();
                Debug.Log("ArcadeManager checkConnection port successfully open");
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
            Debug.Log("ArcadeManager checkConnection port was already open");
            return true;
        }
    }
#endif

    public void playAnimation(string identifier)
    {
#if ARCADE
        if (checkConnection())
        {
            Debug.Log(this.GetType() + " playAnimation(" + identifier + "): port open");
            _port.Write(identifier);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " playAnimation(" + identifier + "): port closed");
        }
#endif
    }
}