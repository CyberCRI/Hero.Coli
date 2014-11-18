using UnityEngine;
using System.Collections;
using System.Xml;

public class MineManager : MonoBehaviour {

	//public TextAsset sceneFilePath3;
	public GameObject mine;

	public static bool isReseting;
	public static int mineChanged;

	public Hero hero;

	// Use this for initialization
	void Start () {
		isReseting = false;
	}

  private Hero safeGetPerso()
  {
      if(null == hero)
      {
        hero = GameObject.Find("Perso").GetComponent<Hero>();
      }
      return hero;
  }
	
	// Update is called once per frame
	void Update () {

		resetMines();
	
  	}

	public void resetMines() {		
    if (safeGetPerso().getIsAlive() == false)
		{
			isReseting = true;
		}
	}

	//public void resetSelectedMine(float id, GameObject target)
  public void resetSelectedMine(GameObject target)
	{
		float x = target.transform.position.x;
		float z = target.transform.position.z;
	
		iTween.Stop(target, true);
		Destroy(target);

		GameObject go = (GameObject) Instantiate(mine, new Vector3(x,0,z),Quaternion.identity);
		//go.GetComponent<Mine>().setId(id);

		mineChanged -= 1;

		if (isReseting && mineChanged == 0)
    {
			isReseting = false;
    }
  }
}
