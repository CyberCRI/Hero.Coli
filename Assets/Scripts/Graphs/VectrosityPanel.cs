#define DEV
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*!
 \brief This behaviour class manages the line drawing on a basic 2D shape
 \sa PanelInfo
 \sa Line
*/
public class VectrosityPanel : MonoBehaviour
{
    public Camera GUICam; //!< The Isometric camera which will display the layer
    public bool draw = true; //!< Toggles drawing of the lines
    public RectTransform graphPlaceHolder;
    public string identifier;

    private const int width = 200;
    private const float height = 200;

    public ReactionEngine _reactionEngine;
    public LinkedList<Medium> _mediums;
    public int _mediumId;

    public List<Line> _lines = new List<Line>();
    public int lineCount;
	public Dictionary<string, Molecule> _molecules;
    public bool _paused = false;

    private bool areLinesNull = false;

    public void setPause(bool paused)
    {
        _paused = paused;
    }

    public int getMediumId()
    {
        return _mediumId;
    }

    private bool safeLazyInit()
    {
        if (null == _reactionEngine)
        {
            _reactionEngine = ReactionEngine.get();
        }

        if (_reactionEngine != null)
        {
            if (null == _mediums)
            {
                _mediums = _reactionEngine.getMediumList();
            }
            if (null == _mediums)
            {
                Debug.LogWarning(this.GetType() + " safeLazyInit failed to get mediums");
                return false;
            }
        }
        else
        {
            Debug.LogWarning(this.GetType() + " safeLazyInit failed to get ReactionEngine");
            return false;
        }

        return true;
    }

    public void setMedium(int mediumId)
    {
        if (!safeLazyInit())
            return;

        _mediumId = mediumId;

        Medium medium = ReactionEngine.getMediumFromId(_mediumId, _mediums);
        if (medium == null)
        {
            Debug.LogError(this.GetType() + " Can't find the given medium (" + _mediumId + ")");
            return;
        }

        _molecules = medium.getMolecules();
        if (_molecules == null)
        {
            Debug.LogError(this.GetType() + " Can't find molecules in medium (" + _mediumId + ")");
            return;
        }

        Line line;
        foreach (Molecule m in _molecules.Values)
        {
            line = _lines.Find(l => m.getName() == l.moleculeName);
            if (null == line)
            {
                string moleculeName = m.getName();
                _lines.Add(new Line(width, height, _mediumId, m.getName(), graphPlaceHolder));
            }
        }

        drawLines(true);

        lineCount = _lines.Count;
    }

    // Use this for initialization
    void Start()
    {

        safeLazyInit();

        _lines = new List<Line>();

        setMedium(_mediumId);
    }

    // Update is called once per frame
    void Update()
    {
        //bool resize = refreshInfos();
        //drawLines(resize);

#if DEV
        if(Input.GetKeyDown(KeyCode.W)) {
          draw = !draw;
        }
#endif

        if (draw)
        {
            //bool resize = refreshInfos();
            //drawLines(resize);
            drawLines(false);
        }

#if DEV
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_mediumId == 1)
            {
                foreach (Line line in _lines)
                {
                    line.doDebugAction();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_mediumId == 1)
            {
                if (areLinesNull)
                {
                    // Debug.Log(this.GetType() + " toggling lines with areLinesNull=" + areLinesNull + ": creating lines");
                    foreach (Line line in _lines)
                    {
                        line.initializeVectorLine();
                        // Debug.Log(this.GetType() + " initialized line " + line.name);
                    }
                    areLinesNull = false;
                }
                else
                {
                    // Debug.Log(this.GetType() + " toggling lines with areLinesNull=" + areLinesNull + ": destroying lines");
                    foreach (Line line in _lines)
                    {
                        line.destroyLine();
                        // Debug.Log(this.GetType() + " destroyed line " + line.name);
                    }
                    areLinesNull = true;
                }
            }
        }
#endif

    }

    // can only be called directly by GUITransitioner
    // use GUITransitioner.showGraphs instead
    public void show(bool show)
    {
        foreach (Line line in _lines)
        {
            line.setActive(show);
        }
    }

    void OnDisable()
    {
        // Debug.Log(this.GetType() + " OnDisable " + identifier);
        GUITransitioner.showGraphs(false, GUITransitioner.GRAPH_HIDER.VECTROSITYPANEL);
    }

    void OnEnable()
    {
        // Debug.Log(this.GetType() + " OnEnable " + identifier);
        GUITransitioner.showGraphs(true, GUITransitioner.GRAPH_HIDER.VECTROSITYPANEL);
    }

    /*!
     * \brief Will draw the lines in the list
     * \param resize If true will resize the lines first
    */
    private void drawLines(bool resize)
    {
        if (_molecules == null)
            return;
        foreach (Line line in _lines)
        {
            Molecule m = ReactionEngine.getMoleculeFromName(line.moleculeName, _molecules);

            //TODO dynamic resize
            //if(resize)
            //line.resize();

            if (!_paused)
            {
                if (m != null)
                {
                    line.addPoint(m.getConcentration());
                }
                else
                {
                    line.addPoint(0f);
                }
            }
            line.redraw();
        }
    }
}
