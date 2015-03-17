using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief This behaviour class manages the line drawing on a basic 2D shape
 \author Yann LEFLOUR
 \mail yleflour@gmail.com
 \sa PanelInfo
 \sa Line
*/
public class VectrosityPanel : MonoBehaviour {
	
  public Camera GUICam; //!< The Isometric camera which will display the layer
  public bool draw = true; //!< Toggles drawing of the lines
  public float padding; //!< Adds padding to the side of your graph (to use if the panel sprite \shape has borders
  public PanelInfos infos; //!< Will provide the panel information to all the lines drawn \sa PanelInfo
  public List<Line> line {get{return _lines;}} //!< List of the lines being drawn
  public string identifier;
  
  private int width = 200;
  private float height = 800;

  private ReactionEngine _reactionEngine;
  private LinkedList<Medium> _mediums;
  public int _mediumId;
	
  private List<Line> _lines = new List<Line>(); 
  private ArrayList _molecules;
  private bool _paused = false;
	
  public void setPause(bool paused) {
	  _paused = paused;
  }

  public int getMediumId()
  {
    return _mediumId;
  }

  private bool safeLazyInit()
  {
    if(null==_reactionEngine)
      _reactionEngine = ReactionEngine.get();

    if(_reactionEngine != null)
    {
      if(null==_mediums)
      {
        _mediums = _reactionEngine.getMediumList();
      }
      if(null==_mediums)
      {
        Logger.Log ("VectrosityPanel::safeLazyInit failed to get mediums", Logger.Level.WARN);
        return false;
      }
    }
    else
    {
      Logger.Log ("VectrosityPanel::safeLazyInit failed to get ReactionEngine", Logger.Level.WARN);
      return false;
    }

    return true;
  }

  public void setMedium(int mediumId)
  {

    if(!safeLazyInit())
      return;

    _mediumId = mediumId;

    Medium medium = ReactionEngine.getMediumFromId(_mediumId, _mediums);
    if (medium == null)
    {
      Debug.Log("Can't find the given medium (" + _mediumId + ")");
      return ;
    }
  
    _molecules = medium.getMolecules();
    if (_molecules == null)
      return ;

    Line line;
    foreach (Molecule m in _molecules)
    {
      line = _lines.Find(l => l.name == m.getName());
      if(null == line)
      {
        _lines.Add(new Line(width, height, infos, m.getName()));
      }
    }
  
    drawLines(true);
  }
	
  // Use this for initialization
  void Start () {
  	infos = new PanelInfos();
  	refreshInfos();
  	
  	VectorLine.SetCamera3D(GUICam);
  
    safeLazyInit();

    _lines = new List<Line>();

    setMedium(_mediumId);
  }
	
  // Update is called once per frame
  void Update () {
	  bool resize = refreshInfos();
	  drawLines(resize);
  }
	
  void OnDisable() {
    Logger.Log("VectrosityPanel::OnDisable "+identifier, Logger.Level.TRACE);
	  foreach(Line line in _lines) {
	    line.vectorline.active = false;
	  }
  }
	
  void OnEnable() {
    Logger.Log("VectrosityPanel::OnEnable "+identifier, Logger.Level.TRACE);
	  foreach(Line line in _lines) {
	    line.vectorline.active = true;
	  }
  }
  
  /*!
   * \brief Will draw the lines in the list
   * \param resize If true will resize the lines first
  */
  void drawLines(bool resize) {
    if (_molecules == null)
      return ;
    foreach(Line line in _lines)
    {
      Molecule m = ReactionEngine.getMoleculeFromName(line.name, _molecules);
      if(resize)
        line.resize();
      if(!_paused) {
        if (m != null)
          line.addPoint(m.getConcentration());
        else
          line.addPoint(0f);
      }
      line.redraw();
    }
  }
  
  /*!
   * \brief Refreshes the infos of the panel 
   * \return True if the panel information were modified
   * \sa PanelInfo
  */
  public bool refreshInfos(){
  	bool changed = false;
  	if(infos.layer != gameObject.layer){
  		infos.layer = gameObject.layer;
  		changed = true;
  	}
  	if(infos.padding != padding){
  		infos.padding = padding;
  		changed = true;
  	}
  	if(infos.panelDimensions != GetComponent<Collider>().bounds.size){
  		infos.panelDimensions = GetComponent<Collider>().bounds.size;
  		changed = true;
  	}
		
  	if(
		infos.panelPos.x != GetComponent<Collider>().bounds.center.x - infos.panelDimensions.x/2 
	 || infos.panelPos.y != GetComponent<Collider>().bounds.center.y - infos.panelDimensions.y/2 
	 || infos.panelPos.z != GetComponent<Collider>().bounds.center.z
	  ){
  		infos.panelPos = new Vector3(
  			GetComponent<Collider>().bounds.center.x - infos.panelDimensions.x/2,
  			GetComponent<Collider>().bounds.center.y - infos.panelDimensions.y/2,
  			GetComponent<Collider>().bounds.center.z);
  		changed = true;
  	}
  	
  	return changed;
  }
}
