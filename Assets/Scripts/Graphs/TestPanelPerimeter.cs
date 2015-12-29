using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief Test class for Vectrosity graph with graph perimeter drawn with vector lines
 \sa PanelInfo
 \sa VectrosityPanel
*/
public class TestPanelPerimeter : VectrosityPanelLine {
	public Color color {get; set;} //!< The line color
	public override VectorLine vectorline {get{return _vectorline;}} //!< The Vectrosity line
	//public Vector2[] pointsArray {get{return _pointsArray;}} //!< The Vector2 array used by vectrosity to draw the lines
	public List<Vector2> pointsList {get{return _pointsList;}} //!< The Vector2 List used by vectrosity to draw the lines
	public VectorLine _vectorline;
	public PanelInfos _panelInfos;
	//private Vector2[] _pointsArray;
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // POINTS FED TO THE VECTROSITY GRAPH ///////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //list of points given to Vectrosity
	public List<Vector2> _pointsList;
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
	public float graphHeight {get; set;} //!< The line max Y value
    public int _graphWidth; //!< The line max X value (final)
	public float _ratioW, _ratioH;
	public float _lastVal = 0f;
	public float _paddingRatio = 0.001f;
    GameObject point1;
    GameObject point2;        
    List<Vector2> linePointsPerimeter;    
    
    public TPPMode tppmode = TPPMode.RANDOM;
    
    public enum TPPMode
    {
        RANDOM,
        PERIMETER
    }

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
  public TestPanelPerimeter(int graphWidth, float graphHeight, PanelInfos panelInfos, string name = ""){
      Debug.LogError(string.Format("TestPanelPerimeter({0}, {1}, {2}, {3})", graphWidth, graphHeight, panelInfos, name));
      //TODO: graphHeight = max(maxYValue, minGraphHeight) to avoid huge zooms
        this.name = name;
        //Debug.LogError("created TestPanelPerimeter "+name);
		this._panelInfos = panelInfos;
		this._graphWidth = 1;
		this.graphHeight = 1f;
        computeRatios();
		
		this._pointsList = new List<Vector2>();
		//resize();
        
		this.color = generateAppropriateColor();
		
		//this._vectorline = new VectorLine("Graph", _pointsArray, this.color, null, 1.0f, LineType.Continuous, Joins.Weld);
		
        //this._vectorline = new VectorLine("Graph_"+name, _pointsList, 1.0f, LineType.Continuous, Joins.Weld);
        
        Vector2 vec00 = newPoint(0, 0);
        Vector2 vec01 = newPoint(0, 1);
        Vector2 vec11 = newPoint(1, 1);
        Vector2 vec10 = newPoint(1, 0);
        linePointsPerimeter = new List<Vector2>(){vec00, vec01, vec11, vec10};    
    
        generatePointsAndDraw();        
        
        //VectorLine.SetCamera3D(GUICam);
        //VectorLine.SetCanvasCamera (GUICam);
        //VectorLine.SetLine (Color.red, new Vector2(0, 0), new Vector2(Screen.width-1, Screen.height-1));
		
        
        //TODO
        //redraw();
	}
    
    
    private Vector2 generateRandomPoint () {
        return new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
    }
	
	/*!
	 * \brief Adds a new point on the graph
	 * \param point the Y value
 	*/
	public override void addPoint(float point) {
        if("MOV"==name) Logger.Log("TestPanelPerimeter addPoint("+point+") "+name, Logger.Level.ONSCREEN);
	}
	
	public void shiftLeftArray() {
	}
	
	/*!
	 * \brief Adds a hidden point based on the previous value
 	*/
	public void addPoint(){		
        if("MOV"==name) Logger.Log("TestPanelPerimeter addPoint "+name, Logger.Level.ONSCREEN);
	}
	
	/*!
	 * \brief Redraws the line
 	*/
	public override void redraw(){
        if("MOV"==name) Logger.Log("TestPanelPerimeter redraw "+name, Logger.Level.ONSCREEN);
		_vectorline.Draw();
	}
	
	/*!
	 * \brief Resizes the graph based on the panel Transform properties
	 * \sa PanelInfos
 	*/
	public override void resize(){
        if("MOV"==name) Logger.Log("TestPanelPerimeter resize with _panelInfos="+_panelInfos+" in "+name, Logger.Level.ONSCREEN);
        if("MOV"==name) Logger.Log("TestPanelPerimeter resize with _panelInfos="+_panelInfos+" in "+name, Logger.Level.ERROR);
        
        if(null == _panelInfos) {
            if("MOV"==name) Logger.Log("TestPanelPerimeter resize null == _panelInfos in "+name, Logger.Level.ERROR);
            return;
        }
		
        //_pointsList.Clear();
        
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
            Debug.LogError("TestPanelPerimeter getMaxX null == _panelInfos");
            return 0;
        }
	}
	
	private float getMinX() {
        if(null != _panelInfos) {
		  return _panelInfos.panelPos.x + _paddingRatio *_panelInfos.padding;
        } else {
            Debug.LogError("TestPanelPerimeter getMinX null == _panelInfos");
            return 0;
        }
	}
	
	private float getMinY() {
        if(null != _panelInfos) {
		  return _panelInfos.panelPos.y + _paddingRatio *_panelInfos.padding;
        } else {
            Debug.LogError("TestPanelPerimeter getMinY null == _panelInfos");
            return 0;
        }
	}
    
    private void computeRatios() {
        if(null == _panelInfos) {
            if("MOV"==name) Debug.LogError("TestPanelPerimeter computeRatios null == _panelInfos in "+name);
            return;
        }
        if(0 == _graphWidth) {
            if("MOV"==name) Debug.LogError("TestPanelPerimeter computeRatios 0 == _graphWidth in "+name);
            return;
        }
        if(0 == graphHeight) {
            if("MOV"==name) Debug.LogError("TestPanelPerimeter computeRatios 0 == graphHeight in "+name);
            return;
        }
        if(0 == _paddingRatio) {
            if("MOV"==name) Debug.LogError("TestPanelPerimeter computeRatios 0 == _paddingRatio in "+name);
        }
        
		_ratioW = (_panelInfos.panelDimensions.x - 2 * _paddingRatio * _panelInfos.padding) / _graphWidth;
		_ratioH = (_panelInfos.panelDimensions.y - 2 * _paddingRatio * _panelInfos.padding) / graphHeight;
        
        Debug.LogError(string.Format("TestPanelPerimeter computeRatios outputs _ratioW={0} and _ratioH={1} "
            +"from _panelInfos={2}, _graphWidth={3}, graphHeight={4}, _paddingRatio={5} in {6}", 
            _ratioW, _ratioH, _panelInfos, _graphWidth, graphHeight, _paddingRatio, name));
    }
    
    private void generatePointsAndDraw() {
        Debug.LogError("TestPanelPerimeter generatePointsAndDraw with tppmode="+tppmode+" in "+name);
        if(TPPMode.RANDOM == tppmode) {
            point1 = new GameObject();
            point1.name = "point1-global";
            Vector2 position1 = generateRandomPoint(); 
            point1.transform.position = position1;
            
            point2 = new GameObject();
            point2.name = "point2-global";
            Vector2 position2 = generateRandomPoint();
            point2.transform.position = position2;
            
            List<Vector2> linePoints = new List<Vector2>(){position1, position2};    
            this._vectorline = new VectorLine("Graph_"+name, linePoints, 1.0f, LineType.Continuous, Joins.Weld);    
            this._vectorline.color = this.color;
            this._vectorline.layer = _panelInfos.layer;
            this._vectorline.Draw();
            
        } else if(TPPMode.PERIMETER == tppmode) {   
            string debug = "linePointsPerimeter=[";
            foreach(Vector2 point in linePointsPerimeter) {
                debug += point+";";
            }
            debug += "]";
            Debug.LogError("generatePointsAndDraw: "+debug);
            
            this._vectorline = new VectorLine("Graph_"+name, linePointsPerimeter, 1.0f, LineType.Continuous, Joins.Weld);
            this._vectorline.color = this.color;
            this._vectorline.layer = _panelInfos.layer;
            this._vectorline.Draw();
        }
    }
    
    public override void doDebugAction() {
        Object.Destroy(point1);
        Object.Destroy(point2);
        VectorLine.Destroy(ref this._vectorline);
        
        if(TPPMode.RANDOM == tppmode) {
            tppmode = TPPMode.PERIMETER;
        } else {
            tppmode = TPPMode.RANDOM;
        }        
        generatePointsAndDraw();
    } 
	
}

