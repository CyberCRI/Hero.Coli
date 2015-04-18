using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedMetricsManager
{

    public static IEnumerator GET (string url, System.Action<WWW> callback)
    {
        Debug.Log ("GET");
        WWW www = new WWW (url);
        yield return www;
        waitForWWW (www, callback);
    }
        
    public static IEnumerator POST (string url, Dictionary<string,string> post, System.Action<WWW> callback)
    {
        Debug.Log ("POST");
        WWWForm form = new WWWForm ();
        foreach (KeyValuePair<string,string> post_arg in post) {
            form.AddField (post_arg.Key, post_arg.Value);
        }
            
        WWW www = new WWW (url, form);
        return waitForWWW (www, callback);
    }
        
    public static IEnumerator POST (string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
    {
        Debug.Log ("POST");
        WWW www = new WWW (url, post, headers);
        return waitForWWW (www, callback);
    }
        
    private static IEnumerator waitForWWW (WWW www, System.Action<WWW> callback)
    {
        Debug.Log ("waitForWWW");
        float elapsedTime = 0.0f;
            
        while (!www.isDone) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f)
                break;
            yield return null;
        }
            
        if (!www.isDone || !string.IsNullOrEmpty (www.error)) {
            string errmsg = string.IsNullOrEmpty (www.error) ? "timeout" : www.error;
            Debug.LogError ("Load Failed: " + errmsg);
            callback (null);    // Pass null result.
            yield break;
        }
            
        callback (www); // Pass retrieved result.
    }
}
