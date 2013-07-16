using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph : MonoBehaviour
{
  public Vector3 _pos;
  public float _width;
  public float _height;
  public Camera cam;

  private Material _material;
  private LinkedList<Curve> _curves;
  public const int _maxPoints = 200;

  public Graph()
  {
    _curves = new LinkedList<Curve>();
  }

  private static Material createLineMaterial()
  {
    Material material = new Material("Shader \"Lines/Colored Blended\" {" +
                                     "SubShader { Pass { " +
                                     "    Blend SrcAlpha OneMinusSrcAlpha " +
                                     "    ZWrite Off Cull Off Fog { Mode Off } " +
                                     "    BindChannels {" +
                                     "      Bind \"vertex\", vertex Bind \"color\", color }" +
                                     "} } }");
    material.hideFlags = HideFlags.HideAndDontSave;
    material.shader.hideFlags = HideFlags.HideAndDontSave;    

    return material;
  }

  public void DrawLine(Vector3 p1, Vector3 p2, Color c, bool isOrtho)
  {
    Material material = createLineMaterial();
    material.SetPass(0);
    GL.PushMatrix();
    if (isOrtho)
      GL.LoadOrtho();
    GL.Begin(GL.LINES);
    GL.Color(c);
    GL.Vertex3(p1.x, p1.y, p1.z);
    GL.Vertex3(p2.x, p2.y, p2.z);
    GL.End();
    GL.PopMatrix();
  }

  public static void DrawSquare(Vector3 pos, Vector3 pos2, Color c, bool isOrtho)
  {
    Material material = createLineMaterial();
    material.SetPass(0);
    GL.PushMatrix();
    if (isOrtho)
      GL.LoadOrtho();
    GL.Begin(GL.QUADS);
    GL.Color(c);
    GL.Vertex3(pos.x, pos.y, pos.z);
    GL.Vertex3(pos2.x, pos.y, pos.z);
    GL.Vertex3(pos2.x, pos2.y, pos.z);
    GL.Vertex3(pos.x, pos2.y, pos.z);
    GL.End();
    GL.PopMatrix();
  }

  public void addCurve(Curve curve)
  {
    _curves.AddLast(curve);
  }

  private void drawAllPoints(LinkedList<Vector2> points, Color c)
  {
    Vector3 p1 = default(Vector3);
    Vector3 p2 = default(Vector3);
    int i = 0;

    foreach (Vector2 p in points)
      {
        if (i % 2 == 1)
          {
            p1 = new Vector3(p.x * 10f, p.y/10f, 0f);
            DrawLine(p2, p1, c, false);
          }
        else
          {
            p2 = new Vector3(p.x * 10f, p.y/10f, 0f);
            if (i != 0)
              DrawLine(p1, p2, c, false);
          }
        i++;
      }
  }

  

  public void Start()
  {
  }

  public void Update()
  {
  }

  public void OnPostRender()
  {
//     Vector3 corner1 = new Vector3(_pos.x,0.001f,0);
    Vector3 corner2 = new Vector3(_pos.x + _width, _pos.y + _height,0);

    DrawSquare(_pos, corner2, new Color(0.5f,0.5f,0.5f,0.5f), true);
    foreach (Curve c in _curves)
      {
        drawAllPoints(c.getPointsList(), c.getColor());
  //       Debug.Log("draw a curve");
      }
  //   Debug.Log("End");

//         Vector3 p1 = new Vector3();
//         Vector3 p2 = new Vector3();

//         p1.x = 0;
//         p2.x = 1;
//         p1.y = 0;
//         p2.y = 3;
//         p1.z = 0f;
//         p2.z = 0f;

//         _material = createLineMaterial();
//         DrawSquare(p1, p2, _material, new Color(0.5f,0.5f,0.5f,0.5f));
    // //     DrawPoint(p1, _material, new Color(0f,1f,0f,1f));
  }

}