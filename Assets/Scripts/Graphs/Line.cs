using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief This behaviour class manages the line drawing on a basic 2D shape
 \author Yann LEFLOUR
 \mail yleflour@gmail.com
 \sa PanelInfo
 \sa VectrosityPanel
*/
public class Line{
	public Color color {get; set;} //!< The line color
	public string name {get; set;} //!< The line name
	public float graphHeight {get; set;} //!< The line max Y value
	public VectorLine vectorline {get{return _vectorline;}} //!< The Vectrosity line
	public Vector3[] pointsArray {get{return _pointsArray;}} //!< The Vector3 array used by vectrosity to draw the lines
	
	private VectorLine _vectorline;
	private PanelInfos _panelInfos;
	private Vector3[] _pointsArray;
	private List<float> _pointsList;
	private int _graphWidth; //!< The line max X value (final)
	private float _ratioW, _ratioH;
	private float _lastVal = 0f;
	
	/*!
	 * \brief Constructor
	 * \param graphHeight Max Y value
	 * \param graphWidth Max number of values on the X axis (cannot be modified)
	 * \param panelinfos contains the panel Transform values \sa PanelInfos
 	*/
  public Line(int graphWidth, float graphHeight, PanelInfos panelInfos, string name = ""){
    this.name = name;
		this._panelInfos = panelInfos;
		this._graphWidth = graphWidth;
		this.graphHeight = graphHeight;
		
		this._pointsList = new List<float>();
		this._pointsArray = new Vector3[_graphWidth];
		
		this.color = new Color(Random.Range(0.2f, 1f), Random.Range(0.2f, 1f), Random.Range(0.2f, 1f));
		
		this._vectorline = new VectorLine("Graph", _pointsArray, this.color, null, 1.0f, LineType.Continuous, Joins.Weld);
		this._vectorline.layer = _panelInfos.layer;
		
		for(int i = 0; i < _graphWidth; i++)
			_pointsList.Add(0);
		
		resize();
		redraw();
	}
	
	/*!
	 * \brief Adds a new point on the graph
	 * \param point the Y value
 	*/
	public void addPoint(float point){
		if(_pointsList.Count == _graphWidth)
			_pointsList.RemoveAt(0);
		_pointsList.Add(point);
		
		for(int i = 0 ; i < _pointsList.Count - 1; i++){
			_pointsArray[i] = _pointsArray[i+1];
			_pointsArray[i].x = i * _ratioW + _panelInfos.panelPos.x + 0.001f*_panelInfos.padding;
		}
		_pointsArray[_pointsList.Count - 1] = newPoint(_pointsList.Count - 1, point);
	}
	
	/*!
	 * \brief Adds a hidden point based on the previous value
 	*/
	public void addPoint(){
		if(_pointsList.Count == _graphWidth)
			_pointsList.RemoveAt(0);
		_pointsList.Add(_lastVal);
		
		for(int i = 0 ; i < _pointsList.Count - 1; i++){
			_pointsArray[i] = _pointsArray[i+1];
			_pointsArray[i].x = i * _ratioW + _panelInfos.panelPos.x + 0.001f*_panelInfos.padding;
		}
		_pointsArray[_pointsList.Count - 1] = newPoint(_pointsList.Count - 1);
	}
	
	/*!
	 * \brief Redraws the line
 	*/
	public void redraw(){
		_vectorline.Draw3D();
	}
	
	/*!
	 * \brief Resizes the graph based on the panel Transform proprieties
	 * \sa PanelInfos
 	*/
	public void resize(){
		_ratioW = 1f / _graphWidth * (_panelInfos.panelDimensions.x - 0.002f*_panelInfos.padding);
		_ratioH = 1f / graphHeight * (_panelInfos.panelDimensions.y - 0.002f*_panelInfos.padding);
		
		//Known values
		int i = 0;
		foreach(float val in _pointsList){
			_pointsArray[i] = newPoint(i, val);
			i++;
		}
		
		//Unknown values
		for(; i < _graphWidth; i++){
			_pointsArray[i] = newPoint(i);
		}
	}
	
	/*!
	 * \brief Generates the Vector3 point corresponding to the X and Y values
 	*/
	private Vector3 newPoint(int x, float y){
		_lastVal = Mathf.Clamp(y, 0, graphHeight);
		return new Vector3(
			x * _ratioW + _panelInfos.panelPos.x + 0.001f*_panelInfos.padding,
			_lastVal * _ratioH + _panelInfos.panelPos.y + 0.001f*_panelInfos.padding,
			_panelInfos.panelPos.z + (y > graphHeight || y < 0 ? 0.01f : -0.01f)
		);
	}
	
	/*!
	 * \brief Generates the Vector3 hidden point based on the previous value
 	*/
	private Vector3 newPoint(int x){
		return new Vector3(
			x * _ratioW + _panelInfos.panelPos.x + 0.001f*_panelInfos.padding,
			_lastVal * _ratioH + _panelInfos.panelPos.y + 0.001f*_panelInfos.padding,
			_panelInfos.panelPos.z + 0.01f		
		);
	}
}

