// #define DEV

using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
/*!
 \brief Class for Vectrosity graph to draw refreshed data on graph
 \sa PanelInfo
 \sa VectrosityPanel
*/
public class Line
{
    public string name; //!< The line name
    public string moleculeName; //!< The name of the molecule whose concentration is represented by this VectrosityPanelLine

    public Color color { get; set; } //!< The line color
    public float graphHeight { get; set; } //!< The line max Y value
    public VectorLine vectorline { get { return _vectorline; } } //!< The Vectrosity line
    public List<Vector2> pointsList { get { return _pointsList; } } //!< The Vector2 List used by vectrosity to draw the lines

    protected VectorLine _vectorline;
    private RectTransform _graphPlaceHolder;

    protected List<Vector2> _pointsList; // list of points fed to VectorLine
    protected List<float> _floatList; // list of values

    protected int lastNonZeroValueIndex; //index of last non-zero value in _floatList

    protected int _graphWidth; //!< The line max X value (final)
    protected float _ratioW, _ratioH;
    //private float _lastVal = 0f;
    protected const float _paddingRatio = 0.001f;
    private const string _graphNamePrefix = "GraphNL_";

    private static Color orange = new Color(1f, 0.5f, 0);
    private static Color darkYellow = new Color(0.5f, 0.5f, 0);
    private static Color purple = new Color(0.5f, 0f, 0.5f);
    private static Color darkRed = new Color(0f, 0.5f, 0);
    private static Color darkGreen = new Color(0f, 0.5f, 0);
    private static Color darkBlue = new Color(0f, 0.5f, 0);
    private static Color grey1 = new Color(0.1f, 0.1f, 0.1f);
    private static Color grey2 = new Color(0.2f, 0.2f, 0.2f);
    private static Color grey3 = new Color(0.3f, 0.3f, 0.3f);
    private static Color grey4 = new Color(0.4f, 0.4f, 0.4f);

    // white -> signalling molecules
    // grey -> complexes
    // magenta -> ampicillin because of its representation in the game
    // green -> anti ampicillin molecule
    // yellow -> FLhDC (speed)
    // fluorescent proteins get their own color
    private static Dictionary<string, Color> moleculeColors = new Dictionary<string, Color>()
    {
        {"AMPI*", Color.black},
        {"AMPI", Color.magenta},
        {"AMPR", darkGreen},
        {"ARAC", grey3},
        {"ATC", grey4},
        {"FLUO1", Color.green},
        {"FLUO2", Color.red},
        {"FLUO3", Color.yellow},
        {"FLUO4", orange},
        {"FLUO5", Color.cyan},
        {"FLUO6", Color.blue},
        {"IPTG", purple},
        {"MOV", darkYellow},
        {"REPR1", darkRed},
        {"REPR2", darkBlue},
        {"REPR3", grey1},
        {"REPR4", grey2},
        {"REPR1*", Color.grey},
        {"REPR2*", Color.white}
  };

    public string generateLineName(int _mediumId, string _moleculeName)
    {
        return _mediumId + "." + _moleculeName;
    }

    /*!
	 * \brief Constructor
	 * \param graphHeight Max Y value
	 * \param graphWidth Max number of values on the X axis (cannot be modified)
 	*/
    public Line(int graphWidth, float _graphHeight, int _mediumId, string _moleculeName, RectTransform graphPlaceHolder)
    {
        this.name = generateLineName(_mediumId, _moleculeName);
        this.moleculeName = _moleculeName;

        this._graphWidth = graphWidth;
        this.lastNonZeroValueIndex = graphWidth;
        this.graphHeight = _graphHeight;
        this._graphPlaceHolder = graphPlaceHolder;

        initializeColor();

        this._floatList = new List<float>();
        this._pointsList = new List<Vector2>();

        initializeVectorLine();
    }

    public void resize()
    {
        computeRatios();
    }

    void initializeColor()
    {
        Color newColor;

        this.color = moleculeColors.TryGetValue(moleculeName, out newColor) ? newColor : Color.black;
    }

    public void initializeVectorLine()
    {
        this._vectorline = new VectorLine(_graphNamePrefix + name, _pointsList, 1.0f, LineType.Continuous, Joins.Weld);
        this._vectorline.color = this.color;
        this._vectorline.layer = _graphPlaceHolder.gameObject.layer;
        resize();
        redraw();
        this._vectorline.rectTransform.position = _graphPlaceHolder.position;
    }

    /*!
	 * \brief Adds a new point on the graph
	 * \param point the Y value
 	*/
    public void addPoint(float point)
    {
        if (0 != point)
        {
            lastNonZeroValueIndex = 0;
        }
        else if (lastNonZeroValueIndex >= _graphWidth && _floatList.Count > 0)
        {
            _floatList.Clear();
            _pointsList.Clear();
            return;
        }
        else if (0 == point && 0 == _floatList.Count)
        {
            return;
        }

        if (_floatList.Count == _graphWidth)
        {
            _floatList.RemoveAt(0);
        }
        else if (0 != point && 0 == _floatList.Count)
        {
            _floatList.Add(getY(0f));
        }

        _floatList.Add(getY(point));

        shiftLeft();
    }

    protected virtual void shiftLeft()
    {
        List<Vector2> newList = new List<Vector2>();

        int i = _graphWidth - _floatList.Count;
        foreach (float f in _floatList)
        {
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
    public void redraw()
    {
        if (null != _vectorline)
        {
            _vectorline.Draw();
        }
    }

    private float getX(int x)
    {
        return x * _ratioW;
    }
    private float getY(float y)
    {
#if DEV
        if (Random.value > 0.5f)
        {
            return 0;
        }
        else
        {
            return graphHeight * _ratioH;
        }
#else
        return Mathf.Clamp(y, 0, graphHeight) * _ratioH;
#endif
    }

    private void computeRatios()
    {
        if (null != _graphPlaceHolder)
        {
            _ratioW = _graphPlaceHolder.rect.width / _graphWidth;
            _ratioH = _graphPlaceHolder.rect.height / graphHeight;
        }
    }

    public void doDebugAction()
    {
#if DEV
        string allValues = "[";
        foreach (Vector2 vec in _pointsList)
        {
            allValues += ";" + vec.ToString();
        }
        allValues += "]";
        Debug.Log(this.GetType() + " Line _pointsList=" + allValues);

        allValues = "[";
        foreach (float flt in _floatList)
        {
            allValues += ";" + flt.ToString();
        }
        allValues += "]";
        Debug.Log(this.GetType() + " Line _floatList=" + allValues);
#endif
    }

    public void setActive(bool isActive)
    {
        if (null != _vectorline)
        {
            _vectorline.active = isActive;
        }
    }

    public void destroyLine()
    {
        VectorLine.Destroy(ref _vectorline);
    }
}

