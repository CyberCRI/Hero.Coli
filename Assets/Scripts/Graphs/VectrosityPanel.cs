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

  public ReactionEngine _reactionEngine;
  public int _mediumId;
	
  private List<Line> _lines = new List<Line>(); 
  private ArrayList _molecules;
  private bool _paused = false;
	
  public void setPause(bool paused) {
	_paused = paused;
  }
	
  // Use this for initialization
  void Start () {
	infos = new PanelInfos();
	refreshInfos();
	
	VectorLine.SetCamera3D(GUICam);
	
	_lines = new List<Line>();

    if (_reactionEngine == null)
      return ;

    LinkedList<Medium> mediums = _reactionEngine.getMediumList();
    if (mediums == null)
      return ;

    Medium medium = ReactionEngine.getMediumFromId(_mediumId, mediums);
    if (medium == null)
      {
        Debug.Log("Can't find the given medium (" + _mediumId + ")");
        return ;
      }

    _molecules = medium.getMolecules();
    if (_molecules == null)
      return ;

    foreach (Molecule m in _molecules)
      _lines.Add(new Line(width, height, infos, m.getName()));

	drawLines(true);
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
    foreach(Line line in _lines){
      Molecule m = ReactionEngine.getMoleculeFromName(line.name, _molecules);
      if(resize) line.resize();
	  if(!_paused) {
        if (m != null)
          line.addPoint(m.getConcentration());
        else
          line.addPoint();
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
  	if(infos.panelDimensions != collider.bounds.size){
  		infos.panelDimensions = collider.bounds.size;
  		changed = true;
  	}
  	if(infos.panelPos != collider.bounds.center){
  		infos.panelPos = new Vector3(
  			collider.bounds.center.x - infos.panelDimensions.x/2,
  			collider.bounds.center.y - infos.panelDimensions.y/2,
  			collider.bounds.center.z);
  		changed = true;
  	}
  	
  	return changed;
  }
}
