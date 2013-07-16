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
	public float padding = 20f; //!< Adds padding to the side of your graph (to use if the panel sprite \shape has borders
	public PanelInfos infos; //!< Will provide the panel information to all the lines drawn \sa PanelInfo
	public List<Line> line {get{return _lines;}} //!< List of the lines being drawn
	
	private List<Line> _lines; 
	private List<float> _values;
	
	// Use this for initialization
	void Start () {
		infos = new PanelInfos();
		refreshInfos();
		
		VectorLine.SetCamera3D(GUICam);		
		
		_lines = new List<Line>();
		_values = new List<float>();
		for(int i = 0; i < 20; i++){
			_lines.Add(new Line(200, 800, infos));
			_values.Add(0f);
		}
		drawLines(true);
	}
	
	// Update is called once per frame
	void Update () {
		bool resize = refreshInfos();
		drawLines(resize);
	}
	
	/*!
	 * \brief Will draw the lines in the list
	 * \param resize If true will resize the lines first
 	*/
	void drawLines(bool resize) {
		int i = 0;
		foreach(Line line in _lines){
			_values[i] += Random.Range(-50, 50);
			_values[i] = Mathf.Clamp(_values[i], 0, 850);
			if(resize) line.resize();
			if(draw)
				line.addPoint(_values[i]);
			else
				line.addPoint();
			line.redraw();
			i++;
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
		if(infos.panelPos != transform.position){
			infos.panelPos = transform.position;
			changed = true;
		}
		
		return changed;
	}
}
