using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief Class for Vectrosity graph to draw refreshed data on graph
 \sa PanelInfo
 \sa VectrosityPanel
*/
public class Line : VectrosityPanelLine {
	public Color color {get; set;} //!< The line color
	public float graphHeight {get; set;} //!< The line max Y value
	public override VectorLine vectorline {get{return _vectorline;}} //!< The Vectrosity line
	public List<Vector2> pointsList {get{return _pointsList;}} //!< The Vector2 List used by vectrosity to draw the lines
	
	private VectorLine _vectorline;
	private PanelInfos _panelInfos;
    
	private List<Vector2> _pointsList; // list of points fed to VectorLine
	private List<float> _floatList; // list of values
    
    private int lastNonZeroValueIndex; //index of last non-zero value in _floatList
    
	private int _graphWidth; //!< The line max X value (final)
	private float _ratioW, _ratioH;
	//private float _lastVal = 0f;
	private float _paddingRatio = 0.001f;

  private Color generateColor()
  {
    return new Color(Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), Random.Range(0.0f, 1f));
  }

  private bool isAppropriate(Color color)
  {
    float max = 0.8f;
    float min = 0.1f;
    return (!((color.r > max) && (color.g > max) && (color.b > max)))
      && (!((color.r < min) && (color.g < min) && (color.b < min)));
  }

  private Color generateAppropriateColor()
  {
    Color color;
    do
    {
      color = generateColor();
    } while (!isAppropriate(color));

    return color;
  }

	/*!
	 * \brief Constructor
	 * \param graphHeight Max Y value
	 * \param graphWidth Max number of values on the X axis (cannot be modified)
	 * \param panelinfos contains the panel Transform values \sa PanelInfos
 	*/
  public Line(int graphWidth, float _graphHeight, PanelInfos panelInfos, int _mediumId, string _moleculeName){
        this.name = generateLineName(_mediumId, _moleculeName);
        this.moleculeName = _moleculeName;
		this._panelInfos = panelInfos;
		this._graphWidth = graphWidth;
        this.lastNonZeroValueIndex = graphWidth;
		this.graphHeight = _graphHeight;
        computeRatios();
        
		this.color = generateAppropriateColor();
        
		this._floatList = new List<float>();
		this._pointsList = new List<Vector2>();		
        
		initializeVectorLine();
        
        resize();
		redraw();
	}
    
    public override void initializeVectorLine() {
        this._vectorline = new VectorLine("GraphNL_"+name, _pointsList, 1.0f, LineType.Continuous, Joins.Weld);
		this._vectorline.color = this.color;
		this._vectorline.layer = _panelInfos.layer;
		
        //resize();
		redraw();
    }
	
	/*!
	 * \brief Adds a new point on the graph
	 * \param point the Y value
 	*/
	public override void addPoint(float point){
        if(0 != point) {
            lastNonZeroValueIndex = 0;
        } else if(lastNonZeroValueIndex >= _graphWidth && _floatList.Count > 0) {
            _floatList.Clear();
            _pointsList.Clear();
            return;
        } else if (0 == point && 0 == _floatList.Count) {
            return;
        }
        
		if(_floatList.Count == _graphWidth) {
			_floatList.RemoveAt(0);
        } else if (0 != point && 0 == _floatList.Count) {
            _floatList.Add(getY(0f));
        }
        
		_floatList.Add(getY(point));
		
		shiftLeft();
	}
	
	private void shiftLeft() {
		List<Vector2> newList = new List<Vector2>();
        
		int i = _graphWidth - _floatList.Count;
		foreach(float f in _floatList){
			Vector2 newPt = new Vector2(getX(i), f); 
			newList.Add(newPt);
			i++;
		}
		        
        _pointsList.Clear();
        _pointsList.AddRange(newList);
        lastNonZeroValueIndex++;
	}
	
	/*!
	 * \brief Redraws the line
 	*/
	public override void redraw(){
        if(null != _vectorline) {
		  _vectorline.Draw();
        }
	}
	
	/*
	public override void resize(){
        Logger.Log("Line resize", Logger.Level.ONSCREEN);
        
		computeRatios();
		
		//Unknown values
		int i = 0;
		int firstRange = _graphWidth - _floatList.Count;
		_pointsLinkedList.Clear();
		
		for(; i < firstRange ; i++){
			_pointsLinkedList.AddLast(newPoint(i, false));
		}
				
		//Known values
		i = _graphWidth - _floatList.Count;
		foreach(float val in _floatList){
			_pointsLinkedList.AddLast(newPoint(i, val));
			i++;
		}
	}
    */
    
	/*!
	 * \brief Resizes the graph based on the panel Transform properties
	 * \sa PanelInfos
 	*/
	public override void resize(){
        if(null == _panelInfos) {
            return;
        }        
        computeRatios();     
	}
	
    /*
	/ *!
	 * \brief Generates the Vector2 point corresponding to the X and Y values
 	* /
	private Vector2 newPoint(int x, float y){
		_lastVal = Mathf.Clamp(y, 0, graphHeight);
		return newPoint(x, true);
	}
	
	/ *!
	 * \brief Generates the Vector2 hidden point based on the previous value
 	* /
	private Vector2 newPoint(int x, bool visible){
		if(visible) {
			return new Vector2(
				getX(x),
				getY()
			);
		} else {
			return new Vector2(
				getMaxX(),
				getMinY ()
			);
		}
		
	}
    
	private float getY(){
		return _lastVal * _ratioH + getMinY();
	}
    
    /*
	/ *!
	 * \brief Adds a hidden point based on the previous value
 	* /
	private void addPoint(){
		addPoint(_lastVal);
	}
    */
	
	private float getX(int x){
		return        x * _ratioW + getMinX();
	}
	private float getY(float y){
		return Mathf.Clamp(y, 0, graphHeight) * _ratioH + getMinY();
	}
	
    /*
	private float getMaxX() {
        if(null != _panelInfos) {
		  return _panelInfos.panelDimensions.x - getMinX();
        } else {
            Logger.Log("Line getMaxX null == _panelInfos", Logger.Level.ERROR);
            return 0;
        }
	}
    */
	
	private float getMinX() {
        if(null != _panelInfos) {
		  return _panelInfos.panelPos.x + _paddingRatio *_panelInfos.padding;
        } else {
            Logger.Log("Line getMinX null == _panelInfos", Logger.Level.ERROR);
            return 0;
        }
	}
	
	private float getMinY() {
        if(null != _panelInfos) {
		  return _panelInfos.panelPos.y + _paddingRatio *_panelInfos.padding;
        } else {
            Logger.Log("Line getMinY null == _panelInfos", Logger.Level.ERROR);
            return 0;
        }
	}
    
    private void computeRatios() {
        if(null == _panelInfos) {
            Logger.Log("Line computeRatios null == _panelInfos in "+name, Logger.Level.ERROR);
            return;
        }
        if(0 == _graphWidth) {
            Logger.Log("Line computeRatios 0 == _graphWidth in "+name, Logger.Level.ERROR);
            return;
        }
        if(0 == graphHeight) {
            Logger.Log("Line computeRatios 0 == graphHeight in "+name, Logger.Level.ERROR);
            return;
        }
        if(0 == _paddingRatio) {
            Logger.Log("Line computeRatios 0 == _paddingRatio in "+name, Logger.Level.ERROR);
        }
        
		_ratioW = (_panelInfos.panelDimensions.x - 2 * _paddingRatio * _panelInfos.padding) / _graphWidth;
		_ratioH = (_panelInfos.panelDimensions.y - 2 * _paddingRatio * _panelInfos.padding) / graphHeight;
    }
    
    public override void doDebugAction() {
        string allValues = "[";
        foreach(Vector2 vec in _pointsList) {
            allValues += ";"+vec.ToString();
        }
        allValues += "]";
        Logger.Log("Line _pointsList="+allValues, Logger.Level.ERROR);
        
        allValues = "[";
        foreach(float flt in _floatList) {
            allValues += ";"+flt.ToString();
        }
        allValues += "]";
        Logger.Log("Line _floatList="+allValues, Logger.Level.ERROR);
    } 
	
    public override void setActive(bool isActive) {
        if(null != _vectorline) {
            _vectorline.active = isActive;
        }
    }
    
    public override void destroyLine() {
        VectorLine.Destroy(ref _vectorline);
    }
}

