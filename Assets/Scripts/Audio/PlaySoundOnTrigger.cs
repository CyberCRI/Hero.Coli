using UnityEngine;
using System.Collections;

public class PlaySoundOnTrigger : MonoBehaviour
{
    /// <summary>
    /// The sound that will be played if the player enters the trigger
    /// </summary>
    [Tooltip("The sound that will be played if the player enters the trigger")]
    public PlayableUISound sound;
    [SerializeField]
    private string _arcadeAnimation = "A";

    public enum SoundType
    {
        DnaPickup,
        NanobotPickup,
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Character.playerTag)
        {
            sound.Play();
            ArcadeManager.instance.playAnimation(_arcadeAnimation);
        }
    }
}
