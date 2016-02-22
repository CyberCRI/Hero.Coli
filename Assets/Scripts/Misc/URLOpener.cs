using UnityEngine;

public static class URLOpener {
	
	private static string getLocalizedUrl(string urlKey) {
		return Localization.Localize(urlKey);
	}
	
	public static void open(string urlKey, bool newTab = false) {
		string localizedUrl = getLocalizedUrl(urlKey);
		if (!newTab) {
			RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOURL, new CustomData (CustomDataTag.SAMETAB, localizedUrl));
			Application.OpenURL (localizedUrl);
		} else {
			RedMetricsManager.get ().sendEvent (TrackingEvent.GOTOURL, new CustomData (CustomDataTag.NEWTAB, localizedUrl));
			switch (Application.platform) {
				case RuntimePlatform.WebGLPlayer:
				case RuntimePlatform.WindowsWebPlayer:
				case RuntimePlatform.OSXWebPlayer:
					Logger.Log ("URLOpener::open Webplayer/WebGL attempting to open " + localizedUrl, Logger.Level.WEBPLAYER);
					Application.ExternalEval ("window.open('" + localizedUrl + "','_blank')");
					break;
				case RuntimePlatform.WindowsPlayer: 
				case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.OSXEditor:
					Debug.LogError ("URLOpener::open Editor/Standalone attempting to open " + localizedUrl);
					Application.OpenURL (localizedUrl);
					break;
				default:
					Debug.LogError ("URLOpener::open default attempting to open " + localizedUrl);
					Application.OpenURL (localizedUrl);
					break;
			}
		}
	}
}
