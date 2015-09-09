using UnityEngine;
using System.Collections;
using LitJson;

public class SerializationTest : MonoBehaviour {

	// Use this for initialization
	void Start () {


		CustomData customData = new CustomData();
		customData.Add("customKey1", "customValue1");
		customData.Add("customKey2", "customValue2");

		int[] coordinates = { 1, 2, 3 };

		TrackingEventData data = new TrackingEventDataWithoutIDs(
			TrackingEvent.START,
			customData,
			"map1.level1.section1",
			coordinates
			);

		//*
		JsonWriter writer = new JsonWriter();
		writer.PrettyPrint = true;
		
		JsonMapper.ToJson(data,writer);
		
		string json = writer.ToString();
		Debug.Log("obj="+data);
		Debug.Log("serialized="+json);

		TrackingEventData deserializedData = JsonMapper.ToObject<TrackingEventData>(json);
		/*/
		
		// If you don't need a JsonWriter, use this.
		string json = JsonMapper.ToJson(exampleClass);
		
		//*/
		
		Debug.Log("deserialized:"+deserializedData);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
