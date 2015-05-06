using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedMetricsManager
{

    public static IEnumerator GET (string url, System.Action<WWW> callback)
    {
        Logger.Log("GET", Logger.Level.INFO);
        WWW www = new WWW (url);
        return waitForWWW (www, callback);
    }

    public static IEnumerator POST (string url, Dictionary<string,string> post, System.Action<WWW> callback)
    {
        Logger.Log("POST", Logger.Level.INFO);
        WWWForm form = new WWWForm ();
        foreach (KeyValuePair<string,string> post_arg in post) {
            form.AddField (post_arg.Key, post_arg.Value);
        }
            
        WWW www = new WWW (url, form);
        return waitForWWW (www, callback);
    }
        
    public static IEnumerator POST (string url, byte[] post, Dictionary<string, string> headers, System.Action<WWW> callback)
    {
        Logger.Log ("POST url:"+url, Logger.Level.INFO);
        WWW www = new WWW (url, post, headers);
        return waitForWWW (www, callback);
    }
        
    private static IEnumerator waitForWWW (WWW www, System.Action<WWW> callback)
    {
        Logger.Log ("waitForWWW", Logger.Level.INFO);
        float elapsedTime = 0.0f;
            
        if(null == www)
        {
            Logger.Log("waitForWWW: null www", Logger.Level.ERROR);
            yield return null;
        }

        while (!www.isDone) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 30.0f)
            {
                Logger.Log("waitForWWW: TimeOut!", Logger.Level.ERROR);
                break;
            }
            yield return null;
        }
            
        if (!www.isDone || !string.IsNullOrEmpty (www.error)) {
            string errmsg = string.IsNullOrEmpty (www.error) ? "timeout" : www.error;
            Logger.Log("RedMetricsManager::waitForWWW Error: Load Failed: " + errmsg, Logger.Level.ERROR);
            callback (null);    // Pass null result.
            yield break;
        }
            
        Logger.Log("waitForWWW: www good to ship!", Logger.Level.ERROR);
        callback (www); // Pass retrieved result.
    }
}
