using UnityEngine;
using System.Collections;
/*!
 \brief This behaviour class manages the line drawing on a basic 2D shape
 \sa VectrosityPanel
 \sa Line
*/
public class PanelInfos
{
	public Vector3 panelPos {get;set;} //!< The panel position in world
	public Vector3 panelDimensions {get;set;} //!< The panel box collider size
	public float padding {get;set;} //!< The graph padding
	public int layer {get;set;} //!< The NGUI/Graph layer
}
