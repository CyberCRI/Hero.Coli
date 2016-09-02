using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

//Test class for Vectrosity graph with 2D/3D mode
public class TestVectorLine : MonoBehaviour {

    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;

    public Camera GUICam;

    public VectorLine _fakeVectorline;
    public VectorLine _vectorline;
    
    Vector2 pos2D1 = new Vector2(0, 0);    
    Vector2 pos2D2 = new Vector2(Screen.width-1, Screen.height-1);

    private string cameraName = "Camera"; //NGUI Camera
    //private string cameraName = "Main Camera"; //World Camera      

	// Use this for initialization
	void Start () {
	   Logger.Log("TestVectorLine starts", Logger.Level.ERROR);
       
       
       //VectorManager.useDraw3D = true;
       //VectorLine.SetCanvasCamera (GUICam);
       //draw2D();
       
       
       
       //GUICam = GameObject.Find(cameraName).GetComponent<Camera>();
       //VectorLine.SetCamera3D(GUICam);
	}
	
	// Update is called once per frame
	void Update () {
        
        //Logger.Log("TestVectorLine runs", Logger.Level.ONSCREEN);
        
        if(Input.GetKeyDown(KeyCode.A)) {
            Logger.Log("TestVectorLine draw3D", Logger.Level.ERROR);  
            draw3D();
        }
        
        if(Input.GetKeyDown(KeyCode.Q)) {  
            Logger.Log("TestVectorLine draw2D", Logger.Level.ERROR);
            draw2D();
        }
        
        if(Input.GetKeyDown(KeyCode.W)) {  
            Logger.Log("TestVectorLine reinit", Logger.Level.ERROR);
            reinit();
        }
       
        //if(Input.GetKeyDown(KeyCode.Return)) {
        //    VectorLine.SetLine (Color.red, pos2D1, pos2D2);
        //}
	}
    
    private void draw3D() {            
            reinit();
                   
            point1 = new GameObject();
            point1.name = "point1-global";
            point1.transform.position = generateRandomPoint();
            point2 = new GameObject();
            point2.name = "point2-global";
            point2.transform.position = generateRandomPoint();

            point3 = new GameObject();
            point3.name = "point3-local";
            point3.transform.localPosition = point1.transform.position; 

            point4 = new GameObject();
            point4.name = "point4-local";
            point4.transform.localPosition = point2.transform.position;

            //List<Vector2> fakeLinePoints = new List<Vector2>(){pos2D1, pos2D1};
            List<Vector3> linePoints = new List<Vector3>(){point1.transform.position, point2.transform.position};
            
            //this._fakeVectorline = new VectorLine("Graph_FakeVectorLine", fakeLinePoints, 1.0f, LineType.Continuous, Joins.Weld);
            this._vectorline = new VectorLine("Graph_TestVectorLine", linePoints, 10.0f, LineType.Continuous, Joins.Weld);

            this._vectorline.color = Color.red;
            this._vectorline.layer = LayerMask.NameToLayer ("Non-Physic");
            
            //this._vectorline.Draw3D(); 
            this._vectorline.Draw3DAuto(); 
    }
    
    private void draw2D() {        
            //TODO either hide/show like before, or make 3D work
        
            reinit();
                   
            point1 = new GameObject();
            point1.name = "point1-global";
            point1.transform.position =  pos2D1;
            point2 = new GameObject();
            point2.name = "point2-global";
            point2.transform.position = pos2D2;

            point3 = new GameObject();
            point3.name = "point3-local";
            point3.transform.localPosition = point1.transform.position; 

            point4 = new GameObject();
            point4.name = "point4-local";
            point4.transform.localPosition = point2.transform.position;

            List<Vector2> linePoints = new List<Vector2>(){pos2D1, pos2D2};

            this._vectorline = new VectorLine("Graph_TestVectorLine", linePoints, 1.0f, LineType.Continuous, Joins.Weld);

            this._vectorline.color = Color.green;
            this._vectorline.layer = 0;
            //GUICam = GameObject.Find("Camera").GetComponent<Camera>();
            //VectorLine.SetCamera3D(GUICam);
            this._vectorline.Draw();
            //VectorLine.SetCanvasCamera (GUICam);
    }
    
    private void reinit() {
        Destroy(point1);
        Destroy(point2);
        Destroy(point3);
        Destroy(point4);
        VectorLine.Destroy(ref _fakeVectorline);
        VectorLine.Destroy(ref _vectorline);        
    }
    
    private Vector3 generateRandomPoint () {
        return new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);
    }
}
