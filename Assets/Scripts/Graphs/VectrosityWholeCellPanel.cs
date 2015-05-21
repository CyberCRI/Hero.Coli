using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

/*!
 \brief This behaviour class manages the line drawing on a basic 2D shape from WholeCell sim data
 \sa PanelInfo
 \sa Line
*/
public class VectrosityWholeCellPanel : MonoBehaviour
{
  
    public Camera GUICam; //!< The Isometric camera which will display the layer
    public bool draw = true; //!< Toggles drawing of the lines
    public float padding; //!< Adds padding to the side of your graph (to use if the panel sprite \shape has borders
    public PanelInfos infos; //!< Will provide the panel information to all the lines drawn \sa PanelInfo
    public List<Line> line { get { return _lines; } } //!< List of the lines being drawn
    public string identifier;
    private int width = 200;
    private float height = 100;

    //private ReactionEngine _reactionEngine;
    public WholeCell wholeCell;
    //private LinkedList<Medium> _mediums;
    //public int _mediumId;
  
    private List<Line> _lines = new List<Line> (); 
    //TODO replace _molecules by _variables
    //private ArrayList _molecules;
    private List<WholeCellVariable> _variables;
    private bool _paused = false;
  
    public void setPause (bool paused)
    {
        _paused = paused;
    }

    public void setWholeCellSimulator ()
    {
        _variables = wholeCell._variables;

        Line line;

        foreach (WholeCellVariable v in _variables) {
            line = _lines.Find (l => l.name == v._codeName);
            if (null == line) {
                _lines.Add (new Line (width, height, infos, v._codeName));
            }
        }
        //*/
  
        drawLines (true);
    }
  
    // Use this for initialization
    void Start ()
    {
        infos = new PanelInfos ();
        refreshInfos ();
    
        VectorLine.SetCamera3D (GUICam);

        _lines = new List<Line> ();

        setWholeCellSimulator ();
    }
  
    // Update is called once per frame
    void Update ()
    {
        bool resize = refreshInfos () || (wholeCell.graphHeight != height);
        drawLines (resize);
    }
  
    void OnDisable ()
    {
        Logger.Log ("VectrosityWholeCellPanel::OnDisable " + identifier, Logger.Level.TRACE);
        foreach (Line line in _lines) {
            line.vectorline.active = false;
        }
    }
  
    void OnEnable ()
    {
        Logger.Log ("VectrosityWholeCellPanel::OnEnable " + identifier, Logger.Level.TRACE);
        foreach (Line line in _lines) {
            line.vectorline.active = true;
        }
    }
  
    /*!
   * \brief Will draw the lines in the list
   * \param resize If true will resize the lines first
  */
    void drawLines (bool resize)
    {
        //if (_molecules == null)
        //  return ;

        if(resize)
        {
            height = wholeCell.graphHeight;
        }

        foreach (Line line in _lines) {
            //Molecule m = ReactionEngine.getMoleculeFromName(line.name, _molecules);
            WholeCellVariable v = _variables.Find (var => var._codeName == line.name);
            if (resize) {
                line.graphHeight = height;
                line.resize ();
            }
            if (!_paused) {
                if (v != null)
                    line.addPoint (v._value);
                else
                    line.addPoint (0f);
            }
            line.redraw ();
        }
    }
  
    /*!
   * \brief Refreshes the infos of the panel 
   * \return True if the panel information were modified
   * \sa PanelInfo
  */
    public bool refreshInfos ()
    {
        bool changed = false;
        if (infos.layer != gameObject.layer) {
            infos.layer = gameObject.layer;
            changed = true;
        }
        if (infos.padding != padding) {
            infos.padding = padding;
            changed = true;
        }
        if (infos.panelDimensions != GetComponent<Collider> ().bounds.size) {
            infos.panelDimensions = GetComponent<Collider> ().bounds.size;
            changed = true;
        }
    
        if (
    infos.panelPos.x != GetComponent<Collider> ().bounds.center.x - infos.panelDimensions.x / 2 
            || infos.panelPos.y != GetComponent<Collider> ().bounds.center.y - infos.panelDimensions.y / 2 
            || infos.panelPos.z != GetComponent<Collider> ().bounds.center.z
    ) {
            infos.panelPos = new Vector3 (
        GetComponent<Collider> ().bounds.center.x - infos.panelDimensions.x / 2,
        GetComponent<Collider> ().bounds.center.y - infos.panelDimensions.y / 2,
        GetComponent<Collider> ().bounds.center.z);
            changed = true;
        }
    
        return changed;
    }
}
