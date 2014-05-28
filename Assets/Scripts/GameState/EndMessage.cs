using UnityEngine;
using System.Collections;

public class EndMessage : MonoBehaviour {

  public UILabel subtitleLabel;
  private string _prefix = "Your time is ";
  private string _suffix = ", congratulations!";

  public void setTimeInMessage(string time) {
    subtitleLabel.text = _prefix+time+_suffix;
  }
}
