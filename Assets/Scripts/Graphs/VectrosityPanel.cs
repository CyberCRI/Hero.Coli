using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief This behaviour class manages the line drawing on a basic 2D shape
 \sa PanelInfo
 \sa VectrosityPanelLine
*/
public class VectrosityPanel : MonoBehaviour {
	
  public Camera GUICam; //!< The Isometric camera which will display the layer
  public bool draw = true; //!< Toggles drawing of the lines
  public float padding; //!< Adds padding to the side of your graph (to use if the panel sprite \shape has borders
  public PanelInfos infos; //!< Will provide the panel information to all the lines drawn \sa PanelInfo
  public List<VectrosityPanelLine> line {get{return _lines;}} //!< List of the lines being drawn
  public string identifier;
  
  public int width = 200;
  public float height = 800;

  public ReactionEngine _reactionEngine;
  public LinkedList<Medium> _mediums;
  public int _mediumId;
	
  public List<VectrosityPanelLine> _lines = new List<VectrosityPanelLine>(); 
  public int lineCount;
  public ArrayList _molecules;
  public bool _paused = false;
	
  private bool areLinesNull = false;
    
  public void setPause(bool paused) {
	  _paused = paused;
  }

  public int getMediumId()
  {
    return _mediumId;
  }

  private bool safeLazyInit()
  {
    if(null==_reactionEngine) {
        _reactionEngine = ReactionEngine.get();
    }

    if(_reactionEngine != null) {
        if(null==_mediums) {
            _mediums = _reactionEngine.getMediumList();
        }
        if(null==_mediums) {
            Logger.Log ("VectrosityPanel::safeLazyInit failed to get mediums", Logger.Level.WARN);
            return false;
        }
    } else {
        Logger.Log ("VectrosityPanel::safeLazyInit failed to get ReactionEngine", Logger.Level.WARN);
        return false;
    }

    return true;
  }

  public void setMedium(int mediumId)
  {
    //Debug.LogError("setMedium("+mediumId+")");
    
    if(!safeLazyInit())
        return;

    _mediumId = mediumId;

    Medium medium = ReactionEngine.getMediumFromId(_mediumId, _mediums);
    if (medium == null)
    {
        Debug.LogError("VectrosityPanel Can't find the given medium (" + _mediumId + ")");
        return ;
    }
  
    _molecules = medium.getMolecules();
    if (_molecules == null) {
        Debug.LogError("VectrosityPanel Can't find molecules in medium (" + _mediumId + ")");
        return ;
    }

    VectrosityPanelLine line;
    foreach (Molecule m in _molecules)
    {
        //string targetLineName = getLineName(m);
        if("MOV" == m.getName()) {
            //Debug.LogError("VectrosityPanel found MOV");
            line = _lines.Find(l => m.getName() == l.name);
            if(null == line)
            {
                _lines.Add(new TestNewLine(width, height, infos, _mediumId+"."+m.getName()));
                //Debug.LogError("VectrosityPanel added MOV line");
            }
        }
    }
  
    drawLines(true);
    
    lineCount = _lines.Count;
  }
  
  
  
    
    /*
    private string getLineName(Molecule m) {
        return _mediumId.ToString()+"."+m.getName();
    }
    */
	
  // Use this for initialization
  void Start () {
    if(_mediumId == 1) {
        //TODO remove
        //gameObject.AddComponent<TestVectorLine>();
    } else {
        Destroy(this);
        return;
    }
    
    infos = new PanelInfos();
    //refreshInfos();
    setInfos();
    safeLazyInit();

    _lines = new List<VectrosityPanelLine>();

    setMedium(_mediumId);
  }
	
  // Update is called once per frame
  void Update () {
    if(_mediumId == 1) {
        bool resize = refreshInfos();
        drawLines(resize);
    }
    
    if(Input.GetKeyDown(KeyCode.Z)) {
        if(_mediumId == 1) {
        /*
        UnityEngine.Bounds panelBounds = GetComponent<Collider>().bounds;
        Debug.LogError("VectrosityPanel Z with #_lines="+_lines.Count
        +"; bounds.center="+panelBounds.center
        +"; bounds.size="+panelBounds.size);
        //*/
            foreach(VectrosityPanelLine line in _lines) {
                //Debug.LogError("VectrosityPanel called doDebugAction on "+line.name);
                line.doDebugAction();
            }
        }
    }
    
    if(Input.GetKeyDown(KeyCode.E)) {
        if(_mediumId == 1) {
            if(areLinesNull) {
                Debug.LogError("VectrosityPanel toggling lines with areLinesNull="+areLinesNull+": creating lines");
                foreach(VectrosityPanelLine line in _lines) {
                    line.initializeVectorLine();
                    Debug.LogError("VectrosityPanel initialized line "+line.name);
                }
                areLinesNull = false;
            } else {
                Debug.LogError("VectrosityPanel toggling lines with areLinesNull="+areLinesNull+": destroying lines");
                foreach(VectrosityPanelLine line in _lines) {
                    line.destroyLine();
                    Debug.LogError("VectrosityPanel destroyed line "+line.name);
                }
                areLinesNull = true;
            }
        }
    }    
  } 
    
  void OnDisable() {
    Logger.Log("VectrosityPanel::OnDisable "+identifier, Logger.Level.TRACE);
	  foreach(VectrosityPanelLine line in _lines) {
	    line.setActive(false);
	  }
  }
	
  void OnEnable() {
    Logger.Log("VectrosityPanel::OnEnable "+identifier, Logger.Level.TRACE);
	  foreach(VectrosityPanelLine line in _lines) {
	    line.setActive(true);
	  }
  }
  
  /*!
   * \brief Will draw the lines in the list
   * \param resize If true will resize the lines first
  */
  private void drawLines(bool resize) {
    Logger.Log("VectrosityPanel drawLines", Logger.Level.ONSCREEN);
    if (_mediumId != 1 || _molecules == null)
        return ;
    foreach(VectrosityPanelLine line in _lines)
    {
        Logger.Log("VectrosityPanel drawLines line.name="+line.name, Logger.Level.ONSCREEN);
        if("1.MOV" == line.name) {        
            //TODO add molecule in VectrosityPanelLine/Line instead of using same name for line and molecule?
            Molecule m = ReactionEngine.getMoleculeFromName("MOV", _molecules);
            //if(resize)
                //line.resize();
            if(!_paused) {
                if (m != null) {
                    float cc = m.getConcentration();
                    Logger.Log("VectrosityPanel drawLines adds cc="+cc, Logger.Level.ONSCREEN);
                    line.addPoint(cc);
                } else {
                    Logger.Log("VectrosityPanel drawLines adds 0", Logger.Level.ONSCREEN);
                    line.addPoint(0f);
                }
            }
            line.redraw();
        }
    }
  }
  
  
  private void setInfos() {
        Vector2 vectrosityPanelSize = new Vector2(transform.localScale.x, transform.localScale.y);
        //Vector2 vectrosityPanelSize = new Vector2(233f, 152f);
        
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 boxSize = boxCollider.size;
        Vector2 graphPanelRatio = new Vector2(boxSize.x, boxSize.y);
        //Vector2 graphPanelRatio = new Vector2(0.88f, 0.6f);        
        
        infos.panelDimensions = new Vector3(vectrosityPanelSize.x*graphPanelRatio.x, vectrosityPanelSize.y*graphPanelRatio.y, 0);        
        //?infos.panelDimensions = new Vector2(140, 354);
        
        //screen size: 1280x720
        //infos.panelPos = new Vector3(640, 354, 0);        
        //infos.panelPos = new Vector3(1280, 714, 0); //top-right corner
        //infos.panelPos = new Vector3(1280-233/2, 714-152/2, 0); //center
        //infos.panelPos = new Vector3(1280-233, 714-152, 0); //bottom-left corner of VectrosityPanel itself
        float x = Screen.width - vectrosityPanelSize.x + ((1f-graphPanelRatio.x)/2f)*vectrosityPanelSize.x;
        float y = Screen.height - 6f - vectrosityPanelSize.y + ((1f-graphPanelRatio.y)/2f)*vectrosityPanelSize.y;
        infos.panelPos = new Vector3(x, y, 0); 
        
        infos.padding = padding;
        
        infos.layer = gameObject.layer;
        
        /*
        Debug.LogError("setInfos "+infos);
        Debug.LogError("setInfos boxSize="+boxSize);
        Debug.LogError("setInfos go.t.localScale="+gameObject.transform.localScale);
        Debug.LogError("setInfos t.localScale="+transform.localScale);
        //*/
  }
  
  /*!
   * \brief Refreshes the infos of the panel 
   * \return True if the panel information were modified
   * \sa PanelInfo
  */
  private bool refreshInfos(){
  	bool changed = false;
      
      /*
  	if(infos.layer != gameObject.layer){
  		infos.layer = gameObject.layer;
  		changed = true;
  	}
  	if(infos.padding != padding){
  		infos.padding = padding;
  		changed = true;
  	}
      
    UnityEngine.Bounds panelBounds = GetComponent<Collider>().bounds;      
      
  	if(infos.panelDimensions != panelBounds.size){
        infos.panelDimensions = panelBounds.size;
        changed = true;
  	}
		
    UISprite sprite = this.gameObject.GetComponent<UISprite>();
    
    
    Logger.Log("infos.panelPos="+infos.panelPos, Logger.Level.ONSCREEN);
    Logger.Log("infos.panelDimensions="+infos.panelDimensions, Logger.Level.ONSCREEN);
    Logger.Log("panelBounds.center="+panelBounds.center, Logger.Level.ONSCREEN);
    Logger.Log("panelBounds.size="+panelBounds.size, Logger.Level.ONSCREEN);
    Logger.Log("go.name="+this.gameObject.name, Logger.Level.ONSCREEN);
    Logger.Log("go.position="+this.gameObject.transform.position, Logger.Level.ONSCREEN);
    //Logger.Log("go.scale="+this.gameObject.transform., Logger.Level.ONSCREEN);
    //Logger.Log("sprite.dimensions=["+sprite.mainTexture.width+";"+sprite.mainTexture.height+"]", Logger.Level.ONSCREEN);
    //Logger.Log("sprite.relativeSize="+sprite.relativeSize, Logger.Level.ONSCREEN);
        
  	if(
		infos.panelPos.x != panelBounds.center.x - infos.panelDimensions.x/2 
	 || infos.panelPos.y != panelBounds.center.y - infos.panelDimensions.y/2 
	 || infos.panelPos.z != panelBounds.center.z
	  ){
  		infos.panelPos = new Vector3(
  			panelBounds.center.x - infos.panelDimensions.x/2,
  			panelBounds.center.y - infos.panelDimensions.y/2,
  			panelBounds.center.z);
  		changed = true;
  	}
  	*/
  	return changed;
  }
}
