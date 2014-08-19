using UnityEngine;
using System.Collections;
using System.Xml;

public class SceneManager3 : MonoBehaviour {

	public TextAsset sceneFilePath3;
	public GameObject mine;

	public static bool isReseting;
	public static int mineChanged;

	// Use this for initialization
	void Start () {

		isReseting = false;
		loadMines();
	
	}
	
	// Update is called once per frame
	void Update () {

		resetMines();
	
	}

	private void loadMines()
	{
		Logger.Log("GO ::"+ mine, Logger.Level.WARN);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(sceneFilePath3.text);

		XmlNodeList nodes = xmlDoc.SelectNodes("/Scene3");

		foreach (XmlNode MineId in nodes)
		{
			foreach (XmlNode node in MineId)
			{
				float x = float.Parse(node["x"].InnerText);
				float z = float.Parse(node["z"].InnerText);

				GameObject go = (GameObject) Instantiate(mine, new Vector3(x,0,z),Quaternion.identity);
				Logger.Log("x: "+x+" z: "+z, Logger.Level.WARN);

				go.GetComponent<Mine>().setId(float.Parse(node["id"].InnerText));


			}
		}

	}

	public void resetMines() {
		
		if (GameObject.Find("Perso").GetComponent<Hero>().getIsAlive() == false)
		{
			isReseting = true;

		}
	}

	public void resetSelectedMine(float id, GameObject target)
	{
		float x = target.transform.position.x;
		float z = target.transform.position.z;
	

		Destroy(target);

		GameObject go = (GameObject) Instantiate(mine, new Vector3(x,0,z),Quaternion.identity);
		go.GetComponent<Mine>().setId(id);

		mineChanged -= 1;

		if (isReseting && mineChanged == 0)
			isReseting = false;

		Logger.Log ("Reseted ::"+id, Logger.Level.WARN);

	}
}
