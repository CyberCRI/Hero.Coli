using UnityEngine;

public class NanobotsCounter : MonoBehaviour
{
    [SerializeField]
    private UILabel _label;
    private static int _totalNanobotCount;
    private int _currentNanobotCount;

    public static void reset()
    {
        _totalNanobotCount = 0;
    }

    public static void initialize()
    {
        _totalNanobotCount = GameObject.FindGameObjectsWithTag(NanobotsPickUpHandler.nanobotTag).Length;
        // Debug.Log("NanobotsCounter initializeCounter _totalNanobots=" + _totalNanobotCount);
    }

    // Use this for initialization
    void Start()
    {
        updateLabel(0);
    }

    public void updateLabel(int nanobotCount)
    {
        _label.text = nanobotCount.ToString() + "/" + _totalNanobotCount.ToString();
        _currentNanobotCount = nanobotCount;
    }

    public int getNanobotCount()
    {
        return _currentNanobotCount;
    }
}
