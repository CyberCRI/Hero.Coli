using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief Test class for Vectrosity graph with random lines
 \sa PanelInfo
 \sa VectrosityPanel
*/
public class TestRandomLine : VectrosityPanelLine {
	public Color color {get; set;} //!< The line color
	public float graphHeight {get; set;} //!< The line max Y value
	public override VectorLine vectorline {get{return _vectorline;}} //!< The Vectrosity line
	//public Vector3[] pointsArray {get{return _pointsArray;}} //!< The Vector3 array used by vectrosity to draw the lines
	public List<Vector2> pointsList {get{return _pointsList;}} //!< The Vector2 List used by vectrosity to draw the lines
	
	public VectorLine _vectorline;
	public PanelInfos _panelInfos;
	//private Vector3[] _pointsArray;
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // POINTS FED TO THE VECTROSITY GRAPH ///////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //list of points given to Vectrosity
	public List<Vector2> _pointsList;
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
	public int _graphWidth; //!< The line max X value (final)
	public float _ratioW, _ratioH;
	public float _lastVal = 0f;
	public float _paddingRatio = 0.001f;
    
    Vector2 pos2D1 = new Vector2(0, 0);    
    Vector2 pos2D2 = new Vector2(Screen.width-1, Screen.height-1);

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
  public TestRandomLine(int graphWidth, float graphHeight, PanelInfos panelInfos, string name = ""){
        this.name = name;
        //Debug.LogError("created TestRandomLine "+name);
		this._panelInfos = panelInfos;
		this._graphWidth = graphWidth;
		this.graphHeight = graphHeight;
		
		this._pointsList = new List<Vector2>();
		//resize();
        
		this.color = generateAppropriateColor();
		
		//this._vectorline = new VectorLine("Graph", _pointsArray, this.color, null, 1.0f, LineType.Continuous, Joins.Weld);
		
        //this._vectorline = new VectorLine("Graph_"+name, _pointsList, 1.0f, LineType.Continuous, Joins.Weld);
        
        GameObject point1 = new GameObject();
        point1.name = "point1-global";
        Vector2 position1 = generateRandomPoint(); 
        point1.transform.position = position1;
        
        GameObject point2 = new GameObject();
        point2.name = "point2-global";
        Vector2 position2 = generateRandomPoint();
        point2.transform.position = position2;
        
        GameObject point3 = new GameObject();
        point3.name = "point3-local";
        point3.transform.localPosition = point1.transform.position; 
        
        GameObject point4 = new GameObject();
        point4.name = "point4-local";
        point4.transform.localPosition = point2.transform.position;
        
        List<Vector2> linePoints = new List<Vector2>(){position1, position2};        
        this._vectorline = new VectorLine("Graph_"+name, linePoints, 1.0f, LineType.Continuous, Joins.Weld);        
		this._vectorline.color = this.color;
		this._vectorline.layer = _panelInfos.layer;
        this._vectorline.Draw();
        
        
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
        if("MOV"==name) Logger.Log("TestRandomLine addPoint("+point+") "+name, Logger.Level.ONSCREEN);
	}
	
	public void shiftLeftArray() {
	}
	
	/*!
	 * \brief Adds a hidden point based on the previous value
 	*/
	public void addPoint(){		
        if("MOV"==name) Logger.Log("TestRandomLine addPoint "+name, Logger.Level.ONSCREEN);
	}
	
	/*!
	 * \brief Redraws the line
 	*/
	public override void redraw(){
        if("MOV"==name) Logger.Log("TestRandomLine redraw "+name, Logger.Level.ONSCREEN);
		_vectorline.Draw();
	}
	
	/*!
	 * \brief Resizes the graph based on the panel Transform properties
	 * \sa PanelInfos
 	*/
	public override void resize(){
        if("MOV"==name) Logger.Log("TestRandomLine resize with _panelInfos="+_panelInfos+" in "+name, Logger.Level.ONSCREEN);
        if("MOV"==name) Logger.Log("TestRandomLine resize with _panelInfos="+_panelInfos+" in "+name, Logger.Level.ERROR);
        
        if(null == _panelInfos) {
            if("MOV"==name) Logger.Log("TestRandomLine resize null == _panelInfos in "+name, Logger.Level.ERROR);
            return;
        }
        
		_ratioW = (_panelInfos.panelDimensions.x - 2 * _paddingRatio * _panelInfos.padding) / _graphWidth;
		_ratioH = (_panelInfos.panelDimensions.y - 2 * _paddingRatio * _panelInfos.padding) / graphHeight;
		
        //_pointsList.Clear();
        
        if(0 == _pointsList.Count) {
		  for(int i = 0; i < _graphWidth ; i++){
		      _pointsList.Add(newPoint(i, 0));
		  }
        }
	}
	
	/*!
	 * \brief Generates the Vector3 point corresponding to the X and Y values
 	*/
	private Vector3 newPoint(int x, float y){
		_lastVal = Mathf.Clamp(y, 0, graphHeight);
		return newPoint(x, true);
	}
	
	/*!
	 * \brief Generates the Vector3 hidden point based on the previous value
 	*/
	private Vector3 newPoint(int x, bool visible){
		if(visible) {
			return new Vector3(
				getX(x),
				getY(),
				_panelInfos.panelPos.z - 0.01f
			);
		} else {
			return new Vector3(
				getMaxX(),
				getMinY (),
				_panelInfos.panelPos.z + 1.0f
			);
		}
		
	}
	
	private float getX(int x){
		return x * _ratioW + _panelInfos.panelPos.x + _paddingRatio *_panelInfos.padding;
	}
	private float getY(){
		return _lastVal * _ratioH + getMinY();
	}
	
	private float getMaxX() {
		return _panelInfos.panelDimensions.x - _paddingRatio * _panelInfos.padding + _panelInfos.panelPos.x;
	}
	
	private float getMinY() {
		return _panelInfos.panelPos.y + _paddingRatio *_panelInfos.padding;
	}
    
    public override void doDebugAction() {
        
    } 
	
}

