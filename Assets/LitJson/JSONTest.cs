using UnityEngine;
using System.Collections;

public class JSONTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		JSON_Object jo1 = new JSON_Object();
		jo1.put("count", 1);
		jo1.increment("count");
		Debug.Log("Should print 2: " + jo1.getInt("count"));
		
		JSON_Object jo2 = new JSON_Object("{\"quote\": \"He said, \\\"I like JSON\\\"\"}");
		Debug.Log("Should print an escaped quote: " + jo2.toString());
		
		JSONArray ja1 = new JSONArray();
		ja1.put(jo1);
		ja1.put(jo2);
		
		Debug.Log("Should print count 2, and an escaped quote: " + ja1.toString());
		
		JSON_Object jo3 = ja1.getJSONObject(1);
		Debug.Log("Should print the same escaped quote: " + jo3.toString());
		
		string twitterTest = "{\"errors\": [{\"message\": \"The Twitter REST API v1 is no longer active. Please migrate to API v1.1. https://dev.twitter.com/docs/api/1.1/overview.\", \"code\": 68}]}";
		JSON_Object jo4 = new JSON_Object(twitterTest);
		JSONArray ja2 = jo4.getJSONArray("errors");
		string message = ja2.getJSONObject(0).getString("message");
		int code = ja2.getJSONObject(0).getInt("code");
		Debug.Log(message + " code: " + code);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
