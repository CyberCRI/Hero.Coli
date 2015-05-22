using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphVariableList : MonoBehaviour {

  public WholeCell         wholeCell;
	public UILabel           namesLabel;
  public UILabel           valuesLabel;
	public bool              displayAll;
  public GameObject        unfoldingVariableList;
    
  public string            debugName;

  private int               pixelsPerMoleculeLine = 15;
      
  public UILabel           topLabels;
  public UILabel           topValues;
  public Vector3           topLabelsShift;
  public Vector3           topValuesShift;
  public Vector3           currentHeight;

  private LinkedList<DisplayedVariable> _displayedVariables = new LinkedList<DisplayedVariable>();
  private int                           _displayedListVariablesCount = 0;
  private LinkedList<DisplayedVariable> _toRemove = new LinkedList<DisplayedVariable>();
  private Vector3 _initialScale;

  void Awake()
  {
    currentHeight = Vector3.zero;
    unfoldingVariableList.transform.localScale = new Vector3(unfoldingVariableList.transform.localScale.x, 20, unfoldingVariableList.transform.localScale.z);
    _initialScale = unfoldingVariableList.transform.localScale;
  }
   
  void Start() {
  }

  private void resetVariableList()
  {
    foreach(DisplayedVariable variable in _displayedVariables)
    {
      variable.reset();
    }
  }

  private void removeUnusedVariables()
  {
    _toRemove.Clear();
    foreach(DisplayedVariable variable in _displayedVariables)
    {
      if(!variable.isUpdated())
      {
        _toRemove.AddLast(variable);
      }
    }
    foreach(DisplayedVariable variable in _toRemove)
    {
      _displayedVariables.Remove(variable);
      _displayedListVariablesCount--;
    }
  }

  //TODO iTween this
  void setUnfoldingListBackgroundScale()
  {
    currentHeight = Vector3.up * (pixelsPerMoleculeLine * _displayedListVariablesCount /*+ pixelsPerDeviceLine * _equipedDevices.Count*/);
    unfoldingVariableList.transform.localScale = _initialScale + currentHeight;
  }

  Vector3 getNewPosition(int index = -1) {
    Vector3 res =
            new Vector3(
                0.0f,
                -_displayedListVariablesCount*pixelsPerMoleculeLine,
                -0.1f
                );
    
    return res;
  }

	// Update is called once per frame
	void Update()
  {
        
    int previousListedCount = _displayedListVariablesCount;
    int previousTotalCount = _displayedVariables.Count;

    resetVariableList();

    List<WholeCellVariable> variables = wholeCell._displayedVariables;

		foreach(WholeCellVariable variable in variables) {
      string realName = variable._realName;
      string codeName = variable._codeName;
			float value = variable._value;
      if(displayAll || (0 != value))
      {
        DisplayedVariable found = LinkedListExtensions.Find(
                    _displayedVariables
                    , m => m.getCodeName() == codeName
                    , false
                    , " GraphVariableList::Update()"
                    );
        if(null != found)
        {
          found.update(value);
        }
        else
        //variable is not displayed yet
        {
          DisplayedVariable created = new DisplayedVariable(codeName, realName, value);
          _displayedListVariablesCount++;
          _displayedVariables.AddLast(created);
        }
      }
		}

    removeUnusedVariables();

		string namesToDisplay = "";
    string valuesToDisplay = "";

		foreach(DisplayedVariable variable in _displayedVariables) {
            namesToDisplay+=variable.getRealName()+":\n";
            valuesToDisplay+=variable.getVal()+"\n";
		}
		if(!string.IsNullOrEmpty(namesToDisplay)) {
			namesToDisplay.Remove(namesToDisplay.Length-1, 1);
      valuesToDisplay.Remove(valuesToDisplay.Length-1, 1);
		}
    namesLabel.text = namesToDisplay;
    valuesLabel.text = valuesToDisplay;

    setUnfoldingListBackgroundScale();
	}
}
