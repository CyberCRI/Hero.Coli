using UnityEngine;

public class DisplayedBioBrick : GenericDisplayedBioBrick {
	
	public UILocalize                 _localize;
	public LastHoveredInfoManager     _lastHoveredInfoManager;
	
	protected new static string prefabURI      = "GUI/screen3/BioBricks/DisplayedBioBrickPrefab";
    public static UnityEngine.Object prefab    = null;
	
	
	public static new DisplayedBioBrick Create(
		Transform parentTransform
		,Vector3 localPosition
		,string spriteName
		,BioBrick biobrick
		,Object externalPrefab = null
		)
	{
		
		string usedSpriteName = ((spriteName!=null)&&(spriteName!=""))?spriteName:getSpriteName(biobrick);
		string nullSpriteName = ((spriteName!=null)&&(spriteName!=""))?"":"(null)=>"+usedSpriteName;
		
		if (null == prefab) prefab = Resources.Load(prefabURI);
		Object prefabToUse = (externalPrefab==null)?prefab:externalPrefab;
		
		Logger.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
		           + ", localPosition="+localPosition
		           + ", spriteName="+spriteName+nullSpriteName
		           + ", biobrick="+biobrick
		           , Logger.Level.DEBUG
		           );
		
		DisplayedBioBrick result = (DisplayedBioBrick)DisplayedElement.Create(
			parentTransform
			,localPosition
			,usedSpriteName
			,prefabToUse
			);
		
		Initialize(result, biobrick);
		
		return result;
	}
	
	protected static void Initialize(
		DisplayedBioBrick biobrickScript
		,BioBrick biobrick
		) {

		// Logger.Log("DisplayedBioBrick::Initialize("+biobrickScript+", "+biobrick+") starts", Logger.Level.TRACE);
		GenericDisplayedBioBrick.Initialize(biobrickScript, biobrick);
		biobrickScript._localize.key = GameplayNames.biobrickPrefix+biobrick.getName();
		Logger.Log("DisplayedBioBrick::Initialize ends with biobrickScript._lastHoveredInfoManager="+biobrickScript._lastHoveredInfoManager, Logger.Level.TRACE);

	}
	
	public new static string getSpriteName(BioBrick brick) {
		return brick.getName();
	}

  protected new string getDebugInfos() {
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+", key="+_localize.key+" time="+Time.realtimeSinceStartup;
  }

	protected override void OnPress(bool isPressed) {
		Logger.Log("DisplayedBioBrick::OnPress _id="+_id+", isPressed="+isPressed, Logger.Level.INFO);
  }
}
