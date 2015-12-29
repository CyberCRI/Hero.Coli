using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief Test class for Vectrosity graph with flat line
 \sa PanelInfo
 \sa VectrosityPanel
*/
public class TestFlatLine : VectrosityPanelLine {
	public Color color {get; set;} //!< The line color
	public float graphHeight {get; set;} //!< The line max Y value
	public override VectorLine vectorline {get{return _vectorline;}} //!< The Vectrosity line
	public List<Vector2> pointsList {get{return _pointsList;}} //!< The Vector2 List used by vectrosity to draw the lines
	
	private VectorLine _vectorline;
	private PanelInfos _panelInfos;
	private LinkedList<Vector2> _pointsLinkedList;
	private List<Vector2> _pointsList;
	private List<float> _floatList;
	private int _graphWidth; //!< The line max X value (final)
	private float _ratioW, _ratioH;
	private float _lastVal = 0f;
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
  public TestFlatLine(int graphWidth, float graphHeight, PanelInfos panelInfos, string name = ""){
        this.name = name;
		this._panelInfos = panelInfos;
		this._graphWidth = graphWidth;
		this.graphHeight = graphHeight;
        computeRatios();
        
		this.color = generateAppropriateColor();
        
		this._floatList = new List<float>();
		this._pointsLinkedList = new LinkedList<Vector2>();
		this._pointsList = new List<Vector2>();		
        
		this._vectorline = new VectorLine("GraphNL_"+name, _pointsList, 1.0f, LineType.Continuous, Joins.Weld);
		this._vectorline.color = this.color;
		this._vectorline.layer = _panelInfos.layer;
		
        resize();
		redraw();
	}
	
	/*!
	 * \brief Adds a new point on the graph
	 * \param point the Y value
 	*/
	public override void addPoint(float point){
        Logger.Log("TestFlatLine addPoint("+point+")", Logger.Level.ONSCREEN);
		if(_floatList.Count == _graphWidth)
			_floatList.RemoveAt(0);
		_floatList.Add(point);
		
		shiftLeftArray(point);
	}
	
	public void shiftLeftArray(float point) {
        Logger.Log("TestFlatLine shiftLeftArray", Logger.Level.ONSCREEN);
		/*
		for(int i = 0 ; i < _graphWidth - 1; i++){
			_pointsArray[i] = _pointsArray[i+1];
			_pointsArray[i].x = getX(i);
		}
		
		_pointsArray[_graphWidth - 1] = newPoint(_graphWidth - 1, _floatList[_floatList.Count-1]);
		*/
		LinkedList<Vector2> newList = new LinkedList<Vector2>();
		_pointsLinkedList.RemoveFirst();
		int i = 0;
		foreach(Vector2 v in _pointsLinkedList){
			Vector2 newPt = new Vector2(getX(i), v.y); 
			newList.AddLast(newPt);
			i++;
		}
		//_pointsLinkedList.AddLast(newPoint(_graphWidth - 1, _floatList[_floatList.Count-1]));
		newList.AddLast(newPoint(_graphWidth - 1, point));
		_pointsLinkedList = newList;
        _pointsList = new List<Vector2>(newList);
	}
	
	/*!
	 * \brief Adds a hidden point based on the previous value
 	*/
	public void addPoint(){
        Logger.Log("TestFlatLine addPoint()", Logger.Level.ONSCREEN);
		addPoint(_lastVal);
	}
	
	/*!
	 * \brief Redraws the line
 	*/
	public override void redraw(){
        Logger.Log("TestFlatLine redraw", Logger.Level.ONSCREEN);
		_vectorline.Draw();
	}
	
	/*
	public override void resize(){
        Logger.Log("TestFlatLine resize", Logger.Level.ONSCREEN);
        
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
        if("MOV"==name) Logger.Log("TestFlatLine resize with _panelInfos="+_panelInfos+" in "+name, Logger.Level.ONSCREEN);
        if("MOV"==name) Logger.Log("TestFlatLine resize with _panelInfos="+_panelInfos+" in "+name, Logger.Level.ERROR);
        
        if(null == _panelInfos) {
            if("MOV"==name) Logger.Log("TestFlatLine resize null == _panelInfos in "+name, Logger.Level.ERROR);
            return;
        }
        
        computeRatios();
        
        if(0 == _pointsList.Count) {
		  for(int i = 0; i < _graphWidth ; i++){
		      _pointsList.Add(newPoint(i, 0));
		  }
        }
	}
	
	/*!
	 * \brief Generates the Vector2 point corresponding to the X and Y values
 	*/
	private Vector2 newPoint(int x, float y){
		_lastVal = Mathf.Clamp(y, 0, graphHeight);
		return newPoint(x, true);
	}
	
	/*!
	 * \brief Generates the Vector2 hidden point based on the previous value
 	*/
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
	
	private float getX(int x){
		return        x * _ratioW + getMinX();
	}
	private float getY(){
		return _lastVal * _ratioH + getMinY();
	}
	
	private float getMaxX() {
        if(null != _panelInfos) {
		  return _panelInfos.panelDimensions.x - getMinX();
        } else {
            Debug.LogError("TestFlatLine getMaxX null == _panelInfos");
            return 0;
        }
	}
	
	private float getMinX() {
        if(null != _panelInfos) {
		  return _panelInfos.panelPos.x + _paddingRatio *_panelInfos.padding;
        } else {
            Debug.LogError("TestFlatLine getMinX null == _panelInfos");
            return 0;
        }
	}
	
	private float getMinY() {
        if(null != _panelInfos) {
		  return _panelInfos.panelPos.y + _paddingRatio *_panelInfos.padding;
        } else {
            Debug.LogError("TestFlatLine getMinY null == _panelInfos");
            return 0;
        }
	}
    
    private void computeRatios() {
        if(null == _panelInfos) {
            if("MOV"==name) Debug.LogError("TestFlatLine computeRatios null == _panelInfos in "+name);
            return;
        }
        if(0 == _graphWidth) {
            if("MOV"==name) Debug.LogError("TestFlatLine computeRatios 0 == _graphWidth in "+name);
            return;
        }
        if(0 == graphHeight) {
            if("MOV"==name) Debug.LogError("TestFlatLine computeRatios 0 == graphHeight in "+name);
            return;
        }
        if(0 == _paddingRatio) {
            if("MOV"==name) Debug.LogError("TestFlatLine computeRatios 0 == _paddingRatio in "+name);
        }
        
		_ratioW = (_panelInfos.panelDimensions.x - 2 * _paddingRatio * _panelInfos.padding) / _graphWidth;
		_ratioH = (_panelInfos.panelDimensions.y - 2 * _paddingRatio * _panelInfos.padding) / graphHeight;
        
        Debug.LogError(string.Format("TestFlatLine computeRatios outputs _ratioW={0} and _ratioH={1} "
            +"from _panelInfos={2}, _graphWidth={3}, graphHeight={4}, _paddingRatio={5} in {6}", 
            _ratioW, _ratioH, _panelInfos, _graphWidth, graphHeight, _paddingRatio, name));
    }
    
    public override void doDebugAction() {
        string allValues = "[";
        foreach(Vector2 vec in _pointsList) {
            allValues += ";"+vec.ToString();
        }
        allValues += "]";
        Debug.LogError("TestFlatLine _pointsList="+allValues);
    } 
	
}

