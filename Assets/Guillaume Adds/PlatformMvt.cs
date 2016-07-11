using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformMvt : MonoBehaviour {
	
	//Tu peux remplacer les Vector3 par Transform, créer des Empty Objects que tu places aux positions que tu veux, et les glisser dans la liste.
	public List<GameObject> points = new List<GameObject>();
	private int currentDestination = 0;
	public float speed = 0.1f;
	[HideInInspector]
	public float saveSpeed;
	public float pause;
    public bool loop = true;
	
	[HideInInspector]
	public Vector3 difMove;
	Vector3 savePos;
	Vector3 newPos;
	
	// Use this for initialization
	void Start () {
		if(points.Count > 0)
        {
			StartCoroutine(Movement(pause,loop));
        }
		
		savePos = this.transform.position;
	}

    void OnEnable()
    {
        StartCoroutine(Movement(pause, loop));
    }

    void OnDisable()
    {
        //StopAllCoroutines();
    }
	
	void Update()
	{
		newPos = this.transform.position;
		difMove = newPos - savePos;
		savePos = this.transform.position;
	}
	
	IEnumerator Movement (float pause, bool loop)
	{
		while (transform.position != points[currentDestination].transform.position)
		{
			transform.position = Vector3.MoveTowards(transform.position, points[currentDestination].transform.position, speed*Time.deltaTime);
			
			yield return true;
		}

		currentDestination++;
		if (currentDestination > points.Count -1)
		{
            if (loop == true)
            {
                currentDestination = 0;
            }
            else currentDestination--;
		}
		
		yield return new WaitForSeconds (pause);
		StartCoroutine(Movement(pause,loop));
		
		yield return true;
	}

    public void restart()
    {
        currentDestination -= 0;
        //StartCoroutine(Movement(pause, loop));
    }
	
}
