using UnityEngine;
using System.Collections;

public class MediumInfoWindow : MonoBehaviour {

  private ReactionEngine _reactionEngine;
  public int _mediumId;

  private GraphWindow _graphWindow;
  private MolCheckBoxList _checkBoxes;
  private UILabel _title;
  private Medium _medium;
  
  public bool setMedium(Medium m)
  {
    if (m != null && _graphWindow != null)
      {
        _medium = m;
        _graphWindow.setMedium(_medium);
        _checkBoxes = gameObject.GetComponentInChildren<MolCheckBoxList>();
        if (_checkBoxes != null && _medium != null)
          _checkBoxes.setMedium(_medium);
        UILabel[] labelTab = gameObject.GetComponentsInChildren<UILabel>();
        foreach (UILabel l in labelTab)
          if (l.name == "Title")
            l.text = m.getName();
        return true;
      }
    return false;
  }

  public void changeMoleculeState(string name, bool state)
  {
    if (_graphWindow == null)
      return;
    _graphWindow.changeMoleculeState(name, state);
  }

  void Start () {
    _reactionEngine = ReactionEngine.get ();
    _graphWindow = gameObject.GetComponentInChildren<GraphWindow>();
    if (setMedium(ReactionEngine.getMediumFromId(_mediumId, _reactionEngine.getMediumList())) == false)
      Logger.Log("Failed to load medium curves", Logger.Level.ERROR);
  }

  void Update () {
  }
}
