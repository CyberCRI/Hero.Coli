// #define DEV
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
    public ArrayList _molecules;
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
                Logger.Log("VectrosityPanel::safeLazyInit failed to get mediums", Logger.Level.WARN);
                return false;
            }
        }
        else
        {
            Logger.Log("VectrosityPanel::safeLazyInit failed to get ReactionEngine", Logger.Level.WARN);
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
            Logger.Log("VectrosityPanel Can't find the given medium (" + _mediumId + ")", Logger.Level.ERROR);
            return;
        }

        _molecules = medium.getMolecules();
        if (_molecules == null)
        {
            Logger.Log("VectrosityPanel Can't find molecules in medium (" + _mediumId + ")", Logger.Level.ERROR);
            return;
        }

        Line line;
        foreach (Molecule m in _molecules)
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
                    Logger.Log("VectrosityPanel toggling lines with areLinesNull=" + areLinesNull + ": creating lines", Logger.Level.ERROR);
                    foreach (Line line in _lines)
                    {
                        line.initializeVectorLine();
                        Logger.Log("VectrosityPanel initialized line " + line.name, Logger.Level.ERROR);
                    }
                    areLinesNull = false;
                }
                else
                {
                    Logger.Log("VectrosityPanel toggling lines with areLinesNull=" + areLinesNull + ": destroying lines", Logger.Level.ERROR);
                    foreach (Line line in _lines)
                    {
                        line.destroyLine();
                        Logger.Log("VectrosityPanel destroyed line " + line.name, Logger.Level.ERROR);
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
        // Logger.Log("VectrosityPanel::OnDisable " + identifier, Logger.Level.TRACE);
        GUITransitioner.showGraphs(false, GUITransitioner.GRAPH_HIDER.VECTROSITYPANEL);
    }

    void OnEnable()
    {
        // Logger.Log("VectrosityPanel::OnEnable " + identifier, Logger.Level.TRACE);
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
