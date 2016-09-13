//#define QUICKTEST
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformMvt : MonoBehaviour {
	
	public List<GameObject> points = new List<GameObject>();
	public int currentDestination = 0;
#if QUICKTEST
	public float speed = 1f;
#else
	public float speed = 0.1f;
#endif
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
			StartCoroutine(move(pause,loop));
        }
		
		savePos = this.transform.position;
	}

    void OnEnable()
    {
        StartCoroutine(move(pause, loop));
        restart();
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
	
	IEnumerator move (float pause, bool loop)
	{
		while (Vector3.Distance(transform.position, points[currentDestination].transform.position) >= 0.002f)
		{
			transform.position = Vector3.MoveTowards(transform.position, points[currentDestination].transform.position, speed*Time.deltaTime);

            yield return true;
		}

		currentDestination++;
		if (currentDestination >= points.Count)
		{
            if (loop == true)
            {
                currentDestination = 0;
            }
            else currentDestination--;
		}
		
		yield return new WaitForSeconds (pause);
		StartCoroutine(move(pause,loop));
		
		yield return true;
	}

    public void restart()
    {
        currentDestination -= 0;
        //StartCoroutine(Movement(pause, loop));
    }

    public Vector3 getCurrentDestination()
    {
        return points[currentDestination].transform.position;
    }

    public void setCurrentDestination(int index)
    {
        currentDestination = index;
    }
	
}
